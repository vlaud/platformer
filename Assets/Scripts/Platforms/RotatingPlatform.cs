using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : Platform
{
    [SerializeField] private Vector2 direction = Vector2.up;
    [SerializeField] private float rotSpeed = 10f;
    Coroutine CoroutineAngle = null;
    WaitForSeconds waitTime = new WaitForSeconds(2f);

    IEnumerator RotatingToPosition(Vector3 dir)
    {
        float Angle = Vector3.Angle(transform.up, dir);
        float rotDir = 1.0f;

        if (Vector3.Dot(transform.right, dir) > 0.0f)
        {
            rotDir = -rotDir;
        }

        yield return waitTime;

        while (Angle > Mathf.Epsilon)
        {
            Debug.Log("rott");

            float delta = rotSpeed * GameManager.Inst.GameDeltaTime;

            if (delta > Angle)
            {
                delta = Angle;
            }

            Angle -= delta;

            transform.Rotate(Vector3.forward * delta * rotDir, Space.World);

            yield return null;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopRotating();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        RotateToVector();
    }

    private void StopRotating()
    {
        if (CoroutineAngle != null)
        {
            StopCoroutine(CoroutineAngle);
            CoroutineAngle = null;
        }
    }

    private void RotateToVector()
    {
        StopRotating();
        CoroutineAngle = StartCoroutine(RotatingToPosition(direction));
    }
}
