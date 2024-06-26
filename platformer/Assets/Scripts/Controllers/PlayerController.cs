using UnityEngine;

public class PlayerController : Controller
{
    void Update()
    {
        //InputMoveAxis();
        InputRotateAxis();
        InputInteractAction();
        InputJumpAction();
    }

    private void InputMoveAxis()
    {
        if(controlTarget != null)
            controlTarget.Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
    }

    private void InputRotateAxis()
    {
        if (controlTarget != null)
            controlTarget.Rotate(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    }

    private void InputInteractAction()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (controlTarget != null)
                controlTarget.Interact();
        }
    }

    private void InputJumpAction()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (controlTarget != null)
                controlTarget.Jump();
        }
    }
}
