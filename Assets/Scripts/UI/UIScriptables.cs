using UnityEngine;

[CreateAssetMenu(fileName = "UIScriptables", menuName = "Scriptable Objects/UIScriptables")]
public class UIScriptables : ScriptableObject
{
    public Vector2 hidePos;
    public Vector2 showPos;
    public UIType type;
    public float MenuMoveTime;
}
