using UnityEngine;
public interface IObjectAction
{
    void GetTextObject(Transform target, TMPro.TMP_Text showText);
    void SetMessages();
    void SetTextAppear(bool v);

    void SetTextPosition(Vector3 position);
}
