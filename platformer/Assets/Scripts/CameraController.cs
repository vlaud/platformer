using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float dropYVelocity = -10f;
    [SerializeField] private Transform player;
    private Vector2 lookAhead;
    void Update()
    {
        var body = player.GetComponent<Rigidbody2D>();
        transform.position = new Vector3(player.position.x + lookAhead.x, player.position.y + lookAhead.y, transform.position.z);
        lookAhead.x = Mathf.Lerp(lookAhead.x, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
        
        lookAhead.y = Mathf.Lerp(lookAhead.y, aheadDistance * (body.velocity.y < dropYVelocity ? -1f : 1f), Time.deltaTime * cameraSpeed);
    }
}
