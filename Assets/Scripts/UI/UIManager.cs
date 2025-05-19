using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Inst;

    public Vector2 minSize;
    public Vector2 maxSize;

    public List<ResizeUI> Vertices;
    public List<ResizeUI> Sides;

    public RectTransform SetTransform;

    private void Awake() => Inst = this;

    private void Start()
    {
        var sides = GetComponentsInChildren<ResizeUI>();

        foreach (var side in sides)
        {
            if (side.isSide) Sides.Add(side);
            else Vertices.Add(side);
        }
    }
}
