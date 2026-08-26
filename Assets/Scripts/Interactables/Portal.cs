using UnityEngine;

public class Portal : MonoBehaviour, IButtonAction
{
    [SerializeField] private Color _originColor;
    ParticleSystem ps;

    public bool IsPortal;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        ButtonAction(IsPortal);
    }

    public void ButtonAction(bool v)
    {
        var settings = ps.main;
        settings.startColor = v ? _originColor : new Color(_originColor.r, _originColor.g, _originColor.b, 0f);
        IsPortal = v;
    }
}
