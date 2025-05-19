using UnityEngine;

public class Controller : MonoBehaviour
{
    public Controlable controlTarget;

    public void ChangeControlTarget(Controlable target)
    {
        Debug.Log("chagne to :" + target);
        controlTarget = target;

        if (target != null) GameManager.Inst.CameraController.SetCamTarget(target.transform);
        //else GameManager.Inst.CameraController.SetCamTarget(GameManager.Inst.Player.transform);
    }
}
