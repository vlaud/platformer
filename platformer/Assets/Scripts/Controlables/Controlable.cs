using UnityEngine;

public abstract class Controlable : MonoBehaviour
{
    public abstract void Move(Vector2 input);

    public abstract void Rotate(Vector2 input);

    public abstract void Interact();

    public abstract void Jump();
}
