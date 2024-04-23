using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Character Cam
    [SerializeField] private Vector2 aheadDistance;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float dropYVelocity = -10f;
    [SerializeField] private Transform player;

    public Transform Player
    {
        get => player;
        set
        {
            player = value;
        }
    }

    private Vector2 lookAhead;

    // Gate Cam
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float gateCameraSpeed;

    void Update()
    {
        if(player != null)
            CharacterCamMove();
    }

    void CharacterCamMove()
    {
        var body = player.GetComponent<Rigidbody2D>();
        transform.position = new Vector3(player.position.x + lookAhead.x, player.position.y + lookAhead.y, transform.position.z);
        lookAhead.x = Mathf.Lerp(lookAhead.x, (aheadDistance.x * player.localScale.x), Time.deltaTime * cameraSpeed);

        lookAhead.y = Mathf.Lerp(lookAhead.y, aheadDistance.y * (body.velocity.y < dropYVelocity ? -1f : 1f), Time.deltaTime * cameraSpeed);
    }

    public void GateCamMove()
    {
        float z = transform.position.z;
        Vector3 playerPos = GameManager.Inst.Player.transform.position;
        Vector3 temp = new Vector3(playerPos.x + lookAhead.x, playerPos.y + lookAhead.y, playerPos.z);
        Vector3 desirePos = Vector3.SmoothDamp(transform.position, temp, ref velocity, gateCameraSpeed);
        desirePos.z = z;
        transform.position = desirePos;
    }
}
