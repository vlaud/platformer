using TMPro;
using UnityEngine;

public enum ObjectType
{
    Box, Circle,
}

public abstract class SwitchableObjects : MonoBehaviour, IObjectAction
{
    protected Rigidbody2D rb;
    [SerializeField] protected float shootForce = 10f;
    [SerializeField] protected Vector3 size;
    [SerializeField] protected LayerMask groundMask;
    public ObjectType type;


    [Header("Show Message")]
    [SerializeField] protected Transform _messages;
    [SerializeField] protected TMPro.TMP_Text showText;

    public void GetTextObject(Transform target, TMP_Text showText)
    {
        _messages = target;
        this.showText = showText;
    }

    public void SetMessages()
    {
        if (showText == null) return;
        showText.text = "Switch";
    }

    public void SetTextAppear(bool v)
    {
        showText.gameObject.SetActive(v);
    }

    public void SetTextPosition(Vector3 position)
    {
        _messages.position = position;
    }

    protected void Init()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
