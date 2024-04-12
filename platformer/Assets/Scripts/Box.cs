using UnityEngine;

public class Box : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private LayerMask groundMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("SeeSaw"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, Vector2.down, 10f, 1 << LayerMask.NameToLayer("SeeSaw"));

        Debug.Log(raycastHit.normal);
        Debug.Log(raycastHit.transform);

        if(rb.velocity.y > 5f)
        {
            rb.AddForce(raycastHit.normal * 10f, ForceMode2D.Impulse);
            Debug.Log("shoot");
        }
    }
}
