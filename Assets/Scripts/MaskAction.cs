using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaskAction : MonoBehaviour
{
    private Vector2 direction = Vector2.down;
    [SerializeField] private float rotSpeed = 10f;
    [SerializeField] private Transform[] childs;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Transform switchTarget;

    public PlayerMovement Player => player;

    private void Awake()
    {
        childs = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();

        SetChildActive(false);
    }

    public void GetSwitchTarget(Transform target)
    {
        if (switchTarget == target) return;

        switchTarget = target;
    }

    public void GetPlayer(PlayerMovement player)
    {
        if (this.player == player) return;

        this.player = player;
    }

    void SetChildActive(bool v)
    {
        foreach (Transform t in childs)
        {
            t.gameObject.SetActive(v);
        }
    }

    void SwitchPosition()
    {
        Vector2 temp = player.transform.position;
        player.transform.position = switchTarget.position;
        switchTarget.position = temp;

        // player activate
        GameManager.Inst.Controller.ChangeControlTarget(player);
        player.enabled = true;
        player.SetOppositeDirection();
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

            GameManager.Inst.CameraController.CamDampMove(switchTarget);

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
