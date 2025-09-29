using UnityEngine;

public class Box : SwitchableObjects
{
    private void Awake()
    {
        base.Init();
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

        if (rb.linearVelocity.y > 5f)
        {
            rb.AddForce(raycastHit.normal * shootForce, ForceMode2D.Impulse);
            Debug.Log("shoot");
        }
    }
}
