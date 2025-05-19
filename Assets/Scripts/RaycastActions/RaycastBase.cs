using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBase : MonoBehaviour
{
    [Header("레이캐스트")]
    [SerializeField] protected Vector2 middle;
    [SerializeField] protected Vector2 hitNormal;
    [SerializeField] protected Vector2 oppositeNormal;
    [SerializeField] protected Vector2 midPos;
    [SerializeField] protected LayerMask interactableLayer;
    [SerializeField] protected float rayLength = 1f;
    [SerializeField] protected PlayerMovement playerMovement;
    [SerializeField] protected Collider2D _collider;
    protected IObjectAction currentShowObject;

    public RaycastHit2D GetRay(LayerMask mask, Vector2 originMiddle, out Vector2 middle, Vector2 originMidPos, out Vector2 midPos)
    {
        Vector2 dir = Vector2.right * playerMovement.Direction;
        Vector2 rayOrigin = (Vector2)transform.position + dir * _collider.bounds.extents.x;
        RaycastHit2D raycastHit = Physics2D.Raycast(rayOrigin, dir, rayLength, mask);

        middle = originMiddle;
        midPos = originMidPos;

        if (raycastHit)
        {
            RaycastHit2D opposite = Physics2D.Raycast(raycastHit.point, -dir, rayLength + 0.1f, 1 << LayerMask.NameToLayer("Player"));

            if (opposite)
            {
                VectorDeadValue(raycastHit.normal, out hitNormal, 0.001f);
                VectorDeadValue(opposite.normal, out oppositeNormal, 0.001f);
                if (hitNormal.y + oppositeNormal.y == 0f)
                {
                    middle = Vector2.up;
                }
                else
                {
                    middle = Vector2.Lerp(hitNormal, oppositeNormal, 0.5f).normalized;
                }

                midPos = 0.5f * (raycastHit.point + opposite.point);
                if (middle.y < 0f) middle = -middle;
                Debug.DrawRay(midPos, middle, Color.red);
                Debug.DrawLine(rayOrigin, raycastHit.point, Color.red);
                return raycastHit;
            }
        }

        Debug.DrawRay(rayOrigin, dir * rayLength, Color.blue);

        return raycastHit;
    }

    private void VectorDeadValue(Vector2 origin, out Vector2 v, float dead)
    {
        v = origin;
        if (Mathf.Abs(v.x) < dead)
            v.x = 0f;
        if (Mathf.Abs(v.y) < dead)
            v.y = 0f;
    }
}
