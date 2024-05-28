using UnityEngine;

public class Box : MonoBehaviour, IObjectAction
{
    private Rigidbody2D rb;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private LayerMask groundMask;

    [Header("¸Þ½ÃÁö")]
    [SerializeField] private Transform _messages;
    [SerializeField] private TMPro.TMP_Text showText;

    public void GetTextObject(Transform target, TMPro.TMP_Text showText)
    {
        _messages = target;
        this.showText = showText;
    }

    public void SetTextAppear(bool v)
    {
        showText.gameObject.SetActive(v);
    }

    public void SetMessages()
    {
        if (showText == null) return;
        showText.text = "Switch";
    }

    public void SetTextPosition(Vector3 position)
    {
        _messages.position = position;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

        if (rb.velocity.y > 5f)
        {
            rb.AddForce(raycastHit.normal * shootForce, ForceMode2D.Impulse);
            Debug.Log("shoot");
        }
    }
}
