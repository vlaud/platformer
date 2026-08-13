using UnityEngine;
using System.Collections.Generic;

public class UIScriptablesHolder : MonoBehaviour, IUIScriptablesInfos, IUISubject
{
    [SerializeField] private UIScriptables UIScriptables;
    [SerializeField] private RectTransform _block;

    private IUIAction _UIAction;
    public Vector2 GetHidePos()
    {
        return UIScriptables.hidePos;
    }

    public Vector2 GetShowPos()
    {
        return UIScriptables.showPos;
    }

    public UIType GetUIType()
    {
        return UIScriptables.type;
    }

    public RectTransform GetRectTransform()
    {
        return GetComponent<RectTransform>();
    }

    public RectTransform GetBlock()
    {
        return _block;
    }

    public float GetMenuMoveTime()
    {
        return UIScriptables.MenuMoveTime;
    }

    public void Init(ref Dictionary<UIType, IUISubject> _UIMenus, float MenuMoveTime)
    {
        if (_block == null)
        {
            foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
            {
                if (rt.CompareTag("UIBlock"))
                {
                    _block = rt;
                    break;
                }
            }
        }
        if (_UIAction == null) _UIAction = new UIAction();

        _UIMenus[UIScriptables.type] = this;
        UIScriptables.MenuMoveTime = MenuMoveTime;
    }

    public IUIAction GetIUIAction()
    {
        return _UIAction;
    }

    public void Activate()
    {
        _UIAction.Activate(this);
    }

    public void Deactivate()
    {
        _UIAction.Deactivate(this);
    }
}
