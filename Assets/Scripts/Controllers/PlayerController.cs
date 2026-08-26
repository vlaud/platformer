using UnityEngine;

public class PlayerController : Controller, IController
{
    public void InputMoveAxis(Vector2 move)
    {
        if(controlTarget != null)
            controlTarget.Move(move);
    }

    private void InputRotateAxis()
    {
        if (controlTarget != null)
            controlTarget.Rotate(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    }

    public void InputInteractAction()
    {
        if (controlTarget != null)
                controlTarget.Interact();
    }

    public void InputJumpAction()
    {
        if (controlTarget != null)
                controlTarget.Jump();
    }
}
