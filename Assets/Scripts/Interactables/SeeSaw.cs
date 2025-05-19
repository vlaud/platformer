using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EVertices
{
    DownLeft, DownRight, UpLeft, UpRight
}

public class SeeSaw : MonoBehaviour
{
    [SerializeField] private Transform box;
    [SerializeField] private float rayDistance = 10f;
    private BoxCollider2D col;
    private Vector2[] vertices = new Vector2[4];

    private void Awake()
    {
        col = box.GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        SetVertices();
        DrawLines();
    }

    public RaycastHit2D VertexRayHit(Vector2 vertex, bool Upward = false)
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(vertex, Upward ? Vector2.up : Vector2.down, rayDistance, GameManager.Inst.Player.GroundMask);

        return raycastHit;
    }

    void SetVertices()
    {
        var min = col.offset - col.size * 0.5f;
        var max = col.offset + col.size * 0.5f;

        vertices[(int)EVertices.DownLeft] = transform.TransformPoint(new Vector3(min.x, min.y));
        vertices[(int)EVertices.DownRight] = transform.TransformPoint(new Vector3(max.x, min.y));
        vertices[(int)EVertices.UpLeft] = transform.TransformPoint(new Vector3(min.x, max.y));
        vertices[(int)EVertices.UpRight] = transform.TransformPoint(new Vector3(max.x, max.y));
    }

    void DrawLines()
    {
        Debug.DrawLine(vertices[(int)EVertices.UpLeft], vertices[(int)EVertices.UpRight], Color.blue);
        Debug.DrawLine(vertices[(int)EVertices.DownLeft], vertices[(int)EVertices.DownRight], Color.blue);
    }
}
