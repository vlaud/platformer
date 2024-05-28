using System.Collections;
using UnityEngine;

public class PlayerMovement : Controlable
{
    public enum PlayerState
    {
        Moving, Flying
    }

    [Header("카메라")]
    [SerializeField] private CameraController cameraController;

    [Header("움직임, 점프")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower = 300f;
    [SerializeField] private int jumpLimit = 2;
    [SerializeField] private int jumpTimes;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float horizontal;
    [SerializeField] private float direction = 1f;
    public float Direction => direction;

    private Rigidbody2D body;
    private Ground _ground;

    [Header("벽 점프")]
    [SerializeField] private LayerMask wallMask;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpingDirection;
    [SerializeField] private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    [SerializeField] private float walllJumpingDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 16f);
    [SerializeField] private float wallSlidingSpeed = 2f;

    [Header("물리")]
    [SerializeField] private float desireX;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Vector3 groundBoxSize;
    [SerializeField] private Vector3 wallBoxSize;
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("포탈")]
    [SerializeField] private Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] private float gateMoveSpeed = 3f;
    [SerializeField] private float scaleSpeed = 3f;
    [SerializeField] private float scaleRotSpeed = 1000f;

    [Header("대포")]
    [SerializeField] private LayerMask cannonMask;
    [SerializeField] private string cannonCoreTag = "CannonCore";

    [Header("박스")]
    [SerializeField] private LayerMask boxMask;
    [SerializeField] private float rayLength = 1f;
    [SerializeField] private Transform mask;

    [Header("레이캐스트")]
    [SerializeField] private PlayerShowMessageOnRaycast rayCheck;
    [SerializeField] private Vector2 middle;
    [SerializeField] private Vector2 hitNormal;
    [SerializeField] private Vector2 oppositeNormal;

    public Rigidbody2D PlayerBody => body;
    public Collider2D PlayerCollider => _collider;

    public LayerMask GroundMask => groundMask;

    public PlayerState State = PlayerState.Moving;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        jumpTimes = jumpLimit;
        rayCheck = GetComponent<PlayerShowMessageOnRaycast>();

        if (GameManager.Inst.Player != this)
        {
            enabled = false;
        }
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
                if (coyoteTimeCounter > 0f) coyoteTimeCounter -= GameManager.Inst.GameDeltaTime;
                if (coyoteTimeCounter < 0f)
                {
                    jumpTimes++;
                    coyoteTimeCounter = 0f;
                }
            }
        }

        WallSlide();
        WallJump();

        jumpBufferCounter -= GameManager.Inst.GameDeltaTime;

        if (jumpBufferCounter > 0f)
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
                jumpTimes = 1;
                break;
        }
    }

    #region AbstractMethods
    public override void Move(Vector2 input)
    {
        if (isWallJumping) return;

        horizontal = input.x;
        if (horizontal < 0f)
            direction = -1f;
        else if (horizontal > 0f)
            direction = 1f;

        SetLocalScale(new Vector2(direction, 1));

        switch (State)
        {
            case PlayerState.Moving:
                SetVelocity(new Vector2(horizontal * (speed - _ground.Friction), body.velocity.y));
                break;
            case PlayerState.Flying:
                desireX = Mathf.Lerp(desireX, horizontal, speed * GameManager.Inst.GameDeltaTime);
                float x = desireX + body.velocity.x;
                x = Mathf.Clamp(x, -speed, speed);
                SetVelocity(new Vector2(x, body.velocity.y));
                break;
        }
    }

    public override void Jump()
    {
        jumpBufferCounter = jumpBufferTime;

        if (wallJumpingCounter > 0f)
        {
            WallJumpAction();
        }
    }

    public override void Rotate(Vector2 input)
    {
        //throw new System.NotImplementedException();
    }

    public override void Interact()
    {
        rayCheck.Interact();
        RaycastHit2D raycast = GetRay(boxMask, middle, out middle);
        if (raycast)
        {
            if (middle == Vector2.zero) return;
            SwitchBody(raycast, middle);
        }
    }
    #endregion

    #region Actions
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
    #endregion

    #region WallAction
    void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            SetVelocity(new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue)));
        }
        else
        {
            isWallSliding = false;
        }
    }

    void WallJumpAction()
    {
        if (jumpTimes < jumpLimit)
        {
            isWallJumping = true;
            SetVelocity(new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y));

            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), walllJumpingDuration);

            if (direction != wallJumpingDirection)
            {
                direction = wallJumpingDirection;
                SetLocalScale(new Vector2(direction, 1f));
            }
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            jumpTimes = 0;
            isWallJumping = false;
            wallJumpingDirection = -direction;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= GameManager.Inst.GameDeltaTime;
        }
    }

    #endregion

    #region CheckEnvironment
    private RaycastHit2D GetRay(LayerMask mask, Vector2 originMiddle, out Vector2 middle)
    {
        Vector2 dir = Vector2.right * direction;
        Vector2 rayOrigin = (Vector2)transform.position + dir * _collider.bounds.extents.x;
        RaycastHit2D raycastHit = Physics2D.Raycast(rayOrigin, dir, rayLength, mask);

        middle = originMiddle;

        if (raycastHit)
        {
            RaycastHit2D opposite = Physics2D.Raycast(raycastHit.point, -dir, rayLength + 0.1f, 1 << LayerMask.NameToLayer("Player"));

            if (opposite)
            {
                VectorDeadValue(raycastHit.normal, out hitNormal, 0.001f);
                VectorDeadValue(opposite.normal, out oppositeNormal, 0.001f);
                if (hitNormal.y + oppositeNormal.y == 0f)
                {
                    middle = Vector2.up;
                }
                else
                {
                    middle = Vector2.Lerp(hitNormal, oppositeNormal, 0.5f).normalized;
                }

                Vector2 midPos = 0.5f * (raycastHit.point + opposite.point);
                if (middle.y < 0f) middle = -middle;
                Debug.DrawRay(midPos, middle, Color.red);
                Debug.DrawLine(rayOrigin, raycastHit.point, Color.red);
                return raycastHit;
            }
        }

        Debug.DrawRay(rayOrigin, dir * rayLength, Color.blue);

        return raycastHit;
    }

    private void VectorDeadValue(Vector2 origin, out Vector2 v, float dead)
    {
        v = origin;
        if (Mathf.Abs(v.x) < dead)
            v.x = 0f;
        if (Mathf.Abs(v.y) < dead)
            v.y = 0f;
    }

    bool IsGrounded()
    {
        Vector3 bottom = _collider.bounds.center - new Vector3(0.0f, _collider.bounds.extents.y - groundBoxSize.y / 2, 0.0f);
        RaycastHit2D raycastHit = Physics2D.BoxCast(bottom, groundBoxSize, 0f, Vector2.down, 0.1f, groundMask);

        return raycastHit.collider != null;
    }

    bool IsWalled()
    {
        Vector3 wallCheck = _collider.bounds.center
            + new Vector3((_collider.bounds.extents.x - wallBoxSize.x / 2f) * direction, 0.0f, 0.0f);
        RaycastHit2D raycastHit = Physics2D.BoxCast(wallCheck, wallBoxSize, 0f, Vector2.right * direction, 0.1f, wallMask);

        return raycastHit.collider != null;
    }
    #endregion

    #region BasicMethods
    public void SetVelocity(Vector2 velocity)
    {
        body.velocity = velocity;
    }

    public void SetRigidbody(bool v)
    {
        transform.GetComponent<SpriteRenderer>().enabled = v;
        body.simulated = v;
        _collider.enabled = v;
    }

    void SetGravity(float amount)
    {
        body.gravityScale = amount;
    }

    public void SetLocalScale(Vector2 scale)
    {
        transform.localScale = scale;
    }

    #endregion

    #region CannonAction
    void ToCannon(Collision2D collision)
    {
        CannonControlable target = collision.gameObject.GetComponent<CannonControlable>();
        SetRigidbody(false);
        target.SetCannonBall(transform);
        transform.SetParent(target.Launcher);
        GameManager.Inst.Controller.ChangeControlTarget(target);
    }
    #endregion

    #region GateAction
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
        body.isKinematic = true;
        body.freezeRotation = true;
        GameManager.Inst.Controller.ChangeControlTarget(gate);
        SetLocalScale(Vector2.one);

        while (Vector3.Distance(transform.position, gate.transform.position) > 0.1f)
        {
            Debug.Log("Moving");
            yield return null;
            transform.position += (gate.transform.position - transform.position).normalized * GameManager.Inst.GameDeltaTime * gateMoveSpeed;
        }

        yield return new WaitForSeconds(0.5f);

        while (transform.localScale.y > 0.1f)
        {
            yield return null;
            Debug.Log("spin");
            transform.localScale -= scaleChange * GameManager.Inst.GameDeltaTime * scaleSpeed;
            transform.Rotate(Vector3.forward * GameManager.Inst.GameDeltaTime * -scaleRotSpeed, Space.World);
        }

        cameraController.SetCamTarget(null);
        transform.position = gate.ConnectedGate.transform.position;
    }

    IEnumerator OutGateCoroutine()
    {
        while (transform.localScale.y < 1f)
        {
            cameraController.CamDampMove(GameManager.Inst.Player.transform);
            yield return null;

            transform.localScale += scaleChange * GameManager.Inst.GameDeltaTime * scaleSpeed;
            transform.Rotate(Vector3.forward * GameManager.Inst.GameDeltaTime * -scaleRotSpeed, Space.World);
        }

        SetLocalScale(Vector2.one);
        transform.rotation = Quaternion.identity;
        body.freezeRotation = false;
        body.isKinematic = false;
        SetGravity(2.5f);
        GameManager.Inst.Controller.ChangeControlTarget(this);
    }
    #endregion

    #region Collisions

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 bottom = _collider.bounds.center - new Vector3(0.0f, _collider.bounds.extents.y - groundBoxSize.y / 2f, 0.0f);
        Gizmos.DrawCube(bottom, groundBoxSize);

        Gizmos.color = Color.blue;
        Vector3 wallCheck = _collider.bounds.center
            + new Vector3((_collider.bounds.extents.x - wallBoxSize.x / 2f) * direction, 0.0f, 0.0f);

        Gizmos.DrawCube(wallCheck, wallBoxSize);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enabled) return;

        ChangeState(PlayerState.Moving);

        if (GameManager.Inst.Controller.controlTarget == null)
            GameManager.Inst.Controller.ChangeControlTarget(this);

        if ((cannonMask & 1 << collision.gameObject.layer) != 0
            && collision.gameObject.CompareTag(cannonCoreTag))
        {
            ToCannon(collision);
        }
    }

    #endregion

    private void SwitchBody(RaycastHit2D? raycastHit, Vector2? desiredDir = null)
    {
        if (raycastHit == null) return;

        RaycastHit2D newHit = raycastHit.Value;

        float dir = 1f;
        mask.position = 0.5f * (newHit.transform.position + transform.position);

        if (transform.position.x > newHit.transform.position.x)
        {
            dir = -dir;
        }
        mask.localScale = new Vector2(2f * dir, 2f);

        var maskAction = mask.GetComponent<MaskAction>();

        maskAction.GetPlayer(this);
        maskAction.GetSwitchTarget(newHit.transform);
        maskAction.RotateToDown(desiredDir, -desiredDir);
    }

    public void InActivatePlayer()
    {
        // Player inactivated
        GameManager.Inst.Controller.ChangeControlTarget(null);
        SetVelocity(Vector2.zero);
        enabled = false;
    }

    public void SetOppositeDirection()
    {
        SetLocalScale(new Vector2(direction, 1));
    }
}
