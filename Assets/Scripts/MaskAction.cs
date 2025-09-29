using System.Collections;
using System.Linq;
using UnityEngine;

public class MaskAction : MonoBehaviour
{
    private Vector2 direction = Vector2.down;
    [SerializeField] private float rotSpeed = 10f;
    [SerializeField] private Transform[] childs;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private SwitchableObjects switchTarget;

    private void Awake()
    {
        childs = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();

        SetChildActive(false);
    }

    public void GetSwitchTargets(PlayerMovement player, SwitchableObjects target)
    {
        switchTarget = target;
        this.player = player;
    }

    void SetChildActive(bool v)
    {
        foreach (Transform t in childs)
        {
            t.gameObject.SetActive(v);
        }
    }

    /// <summary>
    /// 몸체 전환 함수
    /// </summary>
    void SwitchPosition()
    {
        // TODO: 몸체 바꿀 때, controlable을 바꾸는 식으로 수정
        // switch position
        (player.transform.position, switchTarget.transform.position)
         = (switchTarget.transform.position, player.transform.position);

        // switch ObjectType
        (player.type, switchTarget.type) = (switchTarget.type, player.type);


        // player activate
        GameManager.Inst.Controller.ChangeControlTarget(player);
        player.enabled = true;
        player.SetOppositeDirection();

        // switch sprite
        (player.transform.GetComponent<SpriteRenderer>().sprite, switchTarget.transform.GetComponent<SpriteRenderer>().sprite)
         = (switchTarget.transform.GetComponent<SpriteRenderer>().sprite, player.transform.GetComponent<SpriteRenderer>().sprite);

    }

    IEnumerator RotatingToPosition(Vector3 dir)
    {
        float Angle = Vector3.Angle(transform.up, dir);
        float rotDir = -1f;

        SetChildActive(true);
        GameManager.Inst.CameraController.SetCamTarget(null);
        GameManager.Inst.PauseGame();

        player.InActivatePlayer();

        while (Angle > Mathf.Epsilon)
        {
            Debug.Log("rott");

            float delta = rotSpeed * GameManager.Inst.GameUnscaledDeltaTime;

            if (delta > Angle)
            {
                delta = Angle;
            }

            Angle -= delta;

            transform.Rotate(Vector3.forward * delta * rotDir, Space.World);

            GameManager.Inst.CameraController.CamDampMove(switchTarget.transform);

            yield return null;
        }

        GameManager.Inst.StartGame();
        GameManager.Inst.CameraController.SetCamTarget(player.transform);
        SwitchPosition();
        SetChildActive(false);
        transform.rotation = Quaternion.identity;
    }

    public void RotateToDown(Vector3? startDir = null, Vector3? dir = null)
    {
        if (startDir != null)
        {
            float Angle = Vector3.Angle(transform.up, (Vector3)startDir);
            transform.Rotate(Vector3.forward * Angle, Space.World);
        }
        if (dir != null)
        {
            StartCoroutine(RotatingToPosition((Vector3)dir));
            return;
        }
        Debug.Log("null");
        StartCoroutine(RotatingToPosition(direction));
    }
}
