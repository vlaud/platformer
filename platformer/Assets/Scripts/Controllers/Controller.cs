using UnityEngine;

public class Controller : MonoBehaviour
{
    public Controlable controlTarget;

    public void ChangeControlTarget(Controlable target)
    {
        controlTarget = target;
    }
}
