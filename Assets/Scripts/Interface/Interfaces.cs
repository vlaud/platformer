using UnityEngine;
using System.Collections.Generic;
using System;

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

public interface IInputManager
{
    void SetBinding(InputBindingInfo info);
}


public interface IButtonAction
{
    void ButtonAction(bool v);
}

public interface IPlayerMovement
{
    void SetLocalScale(Vector2 scale);
    void SetRigidbody(bool v);
    void Launched();
}

public interface IController
{
    void InputMoveAxis(Vector2 move);
    void InputInteractAction();
    void InputJumpAction();
}

public interface ICannonControlable
{
    Transform Launcher();
    Transform ShootPos();
}

public interface IGateAction
{
    void GateIn();
    IGateAction GetConnectedGate();
    Vector3 GetPos();
}

public interface IGateSubject
{
    void ToGate(IGateAction gate, Action<IGateAction> done = null);
    void OutGate();
}

public interface ICameraController
{
    void SetCamTarget(Transform target);
    void CamDampMove(Transform target);
}

public interface ISceneLoadAction
{
    void SetSceneAction();
    void BeforAction(string sceneName);
    void AfterAction(string sceneName);
}