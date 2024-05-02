using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Character Cam
    [SerializeField] private Vector2 aheadDistance;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float dropYVelocity = -10f;
    [SerializeField] private Transform camTarget;

    public Transform CamTarget
    {
        get => camTarget;
        set
        {
            camTarget = value;
        }
    }

    [SerializeField] private Vector2 lookAhead;

    // Gate Cam
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float gateCameraSpeed;

    void Update()
    {
        if(camTarget != null)
            CamStateProcess();
    }

    void CamStateProcess()
    {
        if (GetTypeofControlable.GetType(camTarget) == typeof(PlayerMovement))
        {
            CharacterCamMove();
        }

        if (GetTypeofControlable.GetType(camTarget) == typeof(CannonControlable))
        {
            CannonCamMove();
        }

        if (GetTypeofControlable.GetType(camTarget) == typeof(GateAction))
        {
            SetVelocityOfCam(Vector2.zero);
            GateCamMove();
        }
    }

    void SetVelocityOfCam(Vector2 value)
    {
        lookAhead = value;
    }

    void CharacterCamMove()
    {
        var body = camTarget.GetComponent<Rigidbody2D>();
        
        lookAhead.x = Mathf.Lerp(lookAhead.x, (aheadDistance.x * camTarget.localScale.x), GameManager.Inst.GameUnscaledDeltaTime * cameraSpeed);
        lookAhead.y = Mathf.Lerp(lookAhead.y, aheadDistance.y * (body.velocity.y < dropYVelocity ? -1f : 1f), GameManager.Inst.GameUnscaledDeltaTime * cameraSpeed);

        transform.position = new Vector3(camTarget.position.x + lookAhead.x, camTarget.position.y + lookAhead.y, transform.position.z);
    }

    void CannonCamMove()
    {
        var cannon = camTarget.GetComponent<CannonControlable>();
        float rot = cannon.Launcher.rotation.z;
        float rotDir = rot <= 0f ? 1f : -1f;

        Vector3 desiredPos = new Vector3(camTarget.position.x + aheadDistance.x * rotDir, camTarget.position.y + aheadDistance.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPos, GameManager.Inst.GameUnscaledDeltaTime * cameraSpeed);

        lookAhead = transform.position - cannon.ShootPos.position;
    }

    public void SetCamTarget(Transform target)
    {
        CamTarget = target;
    }

    void GateCamMove()
    {
        float targetX = camTarget.position.x;
        float playerX = GameManager.Inst.Player.transform.position.x;
        float rotDir = targetX < playerX ? 1f : -1f;

        Vector3 desiredPos = new Vector3(camTarget.position.x + aheadDistance.x * rotDir, camTarget.position.y + aheadDistance.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPos, GameManager.Inst.GameUnscaledDeltaTime * cameraSpeed);
    }

    public void CamDampMove(Transform target)
    {
        float z = transform.position.z;
        Vector3 playerPos = target.position;
        Vector3 temp = new Vector3(playerPos.x + lookAhead.x, playerPos.y + lookAhead.y, playerPos.z);
        Vector3 desirePos = Vector3.SmoothDamp(transform.position, temp, ref velocity, gateCameraSpeed, 1000f, GameManager.Inst.GameUnscaledDeltaTime);
        desirePos.z = z;
        transform.position = desirePos;
    }
}
