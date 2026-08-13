using UnityEngine;
using System.Collections.Generic;

public interface IUIScriptablesInfos
{
    Vector2 GetHidePos();
    Vector2 GetShowPos();
    UIType GetUIType();
    RectTransform GetRectTransform();
    RectTransform GetBlock();
    float GetMenuMoveTime();
    IUIAction GetIUIAction();
}

public interface IUIAction
{
    void Activate(IUIScriptablesInfos info);
    void Deactivate(IUIScriptablesInfos info);
}

public interface IUISubject
{
    void Init(ref Dictionary<UIType, IUISubject> _UIMenus, float MenuMoveTime);
    void Activate();
    void Deactivate();
}