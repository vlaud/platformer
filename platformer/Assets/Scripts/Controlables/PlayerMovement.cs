using System.Collections;
using UnityEngine;

public class PlayerMovement : Controlable
{
    public enum PlayerState
    {
        Moving, Flying
    }

    [Header("움직임, 점프")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower = 300f;
    [SerializeField] private int jumpLimit = 2;
    [SerializeField] private int jumpTimes;
    [SerializeField] private LayerMask groundMask;
    private Rigidbody2D body;
    private Ground _ground;
    [Header("물리")]
    [SerializeField] private float desireX;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("포탈")]
    [SerializeField] private Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] private float scaleSpeed = 3f;
    [SerializeField] private float scaleRotSpeed = 1000f;

    public Rigidbody2D PlayerBody => body;
    public BoxCollider2D BoxCollider => boxCollider;

    public LayerMask GroundMask => groundMask;

    public PlayerState State = PlayerState.Moving;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        jumpTimes = jumpLimit;
    }

    private void Update()
    {
        if (body.velocity.y <= 0)
        {
            if (GameManager.Inst.Controller.controlTarget == null)
                GameManager.Inst.Controller.ChangeControlTarget(this);

            if (IsGrounded())
            {
                coyoteTimeCounter = coyoteTime;
                jumpTimes = 0;
            }
            else
            {
                if (coyoteTimeCounter > 0f) coyoteTimeCounter -= Time.deltaTime;
                if (coyoteTimeCounter < 0f)
                {
                    jumpTimes++;
                    coyoteTimeCounter = 0f;
                }
            }
        }

        jumpBufferCounter -= Time.deltaTime;

        if(jumpBufferCounter > 0f)
            JumpAction();
    }

    public void ChangeState(PlayerState s)
    {
        if (State == s) return;
        State = s;

        switch (State)
        {
            case PlayerState.Moving:
                SetVelocity(Vector2.zero);
                desireX = 0f;
                break;
            case PlayerState.Flying:
                break;
        }
    }

    public override void Move(Vector2 input)
    {
        if (input.x < 0f)
            transform.localScale = new Vector2(-1f, 1);
        else if (input.x > 0f)
            transform.localScale = Vector2.one;

        switch (State)
        {
            case PlayerState.Moving:
                SetVelocity(new Vector2(input.x * (speed - _ground.Friction), body.velocity.y));
                break;
            case PlayerState.Flying:
                desireX = Mathf.Lerp(desireX, input.x, speed * Time.deltaTime);
                float x = desireX + body.velocity.x;
                x = Mathf.Clamp(x, -speed, speed);
                SetVelocity(new Vector2(x, body.velocity.y));
                break;
        }
    }

    public override void Jump()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    void JumpAction()
    {
        if (jumpTimes < jumpLimit)
        {
            coyoteTimeCounter = 0f;
            SetVelocity(new Vector2(body.velocity.x, 0f));

            body.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpTimes++;

            jumpBufferCounter = 0f;
        }
    }

    bool IsGrounded()
    {
        Vector3 bottom = boxCollider.bounds.center - new Vector3(0.0f, boxCollider.bounds.extents.y - boxSize.y / 2, 0.0f);
        RaycastHit2D raycastHit = Physics2D.BoxCast(bottom, boxSize, 0f, Vector2.down, 0.1f, groundMask);

        return raycastHit.collider != null;
    }

    public void SetVelocity(Vector2 velocity)
    {
        body.velocity = velocity;
    }

    void SetGravity(float amount)
    {
        body.gravityScale = amount;
    }

    void ToCannon(Collision2D collision)
    {
        CannonControlable target = collision.gameObject.GetComponent<CannonControlable>();
        SetRigidbody(false);
        target.SetCannonBall(transform);
        transform.SetParent(target.Launcher);
        GameManager.Inst.Controller.ChangeControlTarget(target);
    }

    public void ToGate(GateAction gate)
    {
        StartCoroutine(ToGateCoroutine(gate));
    }

    public void OutGate()
    {
        StartCoroutine(OutGateCoroutine());
    }

    IEnumerator ToGateCoroutine(GateAction gate)
    {
        SetVelocity(Vector2.zero);
        SetGravity(0f);
        GameManager.Inst.Controller.ChangeControlTarget(gate);
        transform.localScale = Vector2.one;

        while (Vector3.Distance(transform.position, gate.transform.position) > 0.1f)
        {
            yield return null;
            transform.position += (gate.transform.position - transform.position).normalized * Time.deltaTime * 3f;
        }

        yield return new WaitForSeconds(0.5f);

        while (transform.localScale.y > 0.1f)
        {
            yield return null;

            transform.localScale -= scaleChange * Time.deltaTime * scaleSpeed;
            transform.Rotate(Vector3.forward * Time.deltaTime * -scaleRotSpeed, Space.World);
        }

        transform.position = gate.ConnectedGate.transform.position;
    }

    IEnumerator OutGateCoroutine()
    {
        while (transform.localScale.y < 1f)
        {
            yield return null;

            transform.localScale += scaleChange * Time.deltaTime * scaleSpeed;
            transform.Rotate(Vector3.forward * Time.deltaTime * -scaleRotSpeed, Space.World);
        }

        transform.localScale = Vector2.one;
        SetGravity(2.5f);
        GameManager.Inst.Controller.ChangeControlTarget(this);
    }

    public void SetRigidbody(bool v)
    {
        transform.GetComponent<SpriteRenderer>().enabled = v;
        body.simulated = v;
        boxCollider.enabled = v;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 bottom = boxCollider.bounds.center - new Vector3(0.0f, boxCollider.bounds.extents.y - boxSize.y / 2, 0.0f);
        Gizmos.DrawCube(bottom, boxSize);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeState(PlayerState.Moving);

        if (GameManager.Inst.Controller.controlTarget == null)
            GameManager.Inst.Controller.ChangeControlTarget(this);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Cannon"))
        {
            ToCannon(collision);
        }
    }

    public override void Rotate(Vector2 input)
    {
        //throw new System.NotImplementedException();
    }

    public override void Interact()
    {
        //throw new System.NotImplementedException();
    }
}
