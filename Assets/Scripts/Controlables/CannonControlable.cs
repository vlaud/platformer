using TMPro;
using UnityEngine;

public class CannonControlable : Controlable, IObjectAction
{
    [SerializeField] private Transform launcher;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform cannonBall;
    [SerializeField] Vector2 rotLimit = new Vector2(-70f, 70f);
    [SerializeField] float rot;
    [SerializeField] float shootPower = 30f;
    [SerializeField] PlayerMovement player;

    [Header("메시지")]
    [SerializeField] private Transform _messages;
    [SerializeField] private TMP_Text showText;

    public Transform Launcher => launcher;
    public Transform ShootPos => shootPos;
    public Transform CannonBall => cannonBall;

    private void Awake()
    {
        rot = launcher.localRotation.eulerAngles.z;
    }

    public override void Interact()
    {

    }

    public override void Jump()
    {
        Shoot();
    }

    public override void Move(Vector2 input)
    {
        float rotDir = rot <= 0f ? 1f : -1f;
        player.transform.localScale = new Vector2(rotDir, 1);

        rot -= input.x;
        rot = Mathf.Clamp(rot, rotLimit.x, rotLimit.y);
        launcher.localRotation = Quaternion.Euler(0f, 0f, rot);
    }

    public override void Rotate(Vector2 input)
    {

    }

    public void Shoot()
    {
        cannonBall.position = shootPos.position;
        cannonBall.SetParent(null);

        player?.SetRigidbody(true);
        player?.ChangeState(PlayerMovement.PlayerState.Flying);
        GameManager.Inst.Controller.ChangeControlTarget(null);
        GameManager.Inst.CameraController.SetCamTarget(GameManager.Inst.Player.transform);

        if (cannonBall.GetComponent<Rigidbody2D>() != null)
            cannonBall.GetComponent<Rigidbody2D>().AddForce(shootPos.up * shootPower, ForceMode2D.Impulse);

        cannonBall = null;
        player = null;
    }

    public void SetCannonBall(Transform cannonball)
    {
        cannonBall = cannonball;

        if (cannonball.GetComponent<PlayerMovement>() != null)
            player = cannonball.GetComponent<PlayerMovement>();
    }

    public void GetTextObject(Transform target, TMP_Text showText)
    {
        _messages = target;
        this.showText = showText;
    }

    public void SetMessages()
    {
        if (showText == null) return;
        showText.text = "GetIn";
    }

    public void SetTextAppear(bool v)
    {
        showText.gameObject.SetActive(v);
    }

    public void SetTextPosition(Vector3 position)
    {
        _messages.position = position;
    }
}
