using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Portal))]
public class BlackholeGravity : MonoBehaviour
{
    public Transform BlackholeCenter;
    public Portal _Portal;
    public float PullForce;
    public float BlackholeMass;
    public float RefreshRate;
    [Range(1000f, 10000f)]
    public float MinimumForce = 6000f;
    public LayerMask PullableObjs;

    private Dictionary<Transform, Coroutine> pullableObjs = new Dictionary<Transform, Coroutine>();

    private void Update()
    {
        if (!_Portal.IsPortal) foreach (var col in pullableObjs.Keys) SetGravity(col, 2.5f);
        else foreach (var col in pullableObjs.Keys) SetGravity(col, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((PullableObjs & 1 << collision.gameObject.layer) != 0)
        {
            Debug.Log("enter");
            if (_Portal.IsPortal) SetVelocityByLerp(collision.GetComponent<Rigidbody2D>(), Vector2.zero);
            if (!pullableObjs.ContainsKey(collision.transform))
            {
                pullableObjs.Add(collision.transform, StartCoroutine(pullObject(collision)));
                SetGravity(collision.transform, 0f);
                LerpVelocity(collision, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((PullableObjs & 1 << collision.gameObject.layer) != 0)
        {
            Debug.Log("exit");
            if (pullableObjs.TryGetValue(collision.transform, out var value))
            {
                if (value != null) StopCoroutine(value);
                pullableObjs.Remove(collision.transform);
                SetGravity(collision.transform, 2.5f);
            }
        }
    }

    void SetGravity(Transform x, float amount)
    {
        x.GetComponent<Rigidbody2D>().gravityScale = amount;
    }

    void SetVelocityByLerp(Rigidbody2D body, Vector2 velocity)
    {
        body.velocity = Vector2.Lerp(body.velocity, velocity, Time.deltaTime * 20f);
    }

    void LerpVelocity(Collider2D x, bool IsElipticalOrbit)
    {
        if (!_Portal.IsPortal) return;

        var rigid = x.GetComponent<Rigidbody2D>();

        float r = Vector3.Distance(rigid.transform.position, transform.position);

        Vector2 ForceDir = BlackholeCenter.position - x.transform.position;
        float Angle = Vector3.Angle(transform.up, ForceDir.normalized);
        float rotDir = 1.0f;
        if (Vector3.Dot(transform.right, ForceDir.normalized) > 0.0f)
        {
            rotDir = -rotDir;
        }
        rigid.transform.Rotate(Vector3.forward * rotDir * Angle, Space.World);

        if (IsElipticalOrbit)
        {
            // Eliptic orbit = G * M  ( 2 / r + 1 / a) where G is the gravitational constant, M is the mass of the central object, r is the distance between the two bodies
            // and a is the length of the semi major axis (!!! NOT GAMEOBJECT a !!!)
            rigid.velocity += (Vector2)rigid.transform.right * Time.deltaTime * Mathf.Sqrt((PullForce * BlackholeMass) * ((2 / r) - (1 / (r * 1.5f))));
        }
        else
        {
            //Circular Orbit = ((G * M) / r)^0.5, where G = gravitational constant, M is the mass of the central object and r is the distance between the two objects
            //We ignore the mass of the orbiting object when the orbiting object's mass is negligible, like the mass of the earth vs. mass of the sun
            rigid.velocity += (Vector2)rigid.transform.right * Time.deltaTime * Mathf.Sqrt((PullForce * BlackholeMass) / r);
        }
    }

    IEnumerator pullObject(Collider2D x)
    {
        while (BlackholeCenter != null)
        {
            if (_Portal.IsPortal)
            {
                Debug.Log("pulling");
                Vector2 ForceDir = BlackholeCenter.position - x.transform.position;
                float dist = ForceDir.magnitude;
                var rigid = x.GetComponent<Rigidbody2D>();

                float m1 = rigid.GetComponent<Rigidbody2D>().mass;

                //(G * (m1 * m2) / (r * r)));
                //(PullForce * (m1 * BlackholeMass) / (dist * dist)));
                //(PullForce * (m1 * BlackholeMass) / sqrDist));

                float sqrDist = dist * dist;

                sqrDist = Mathf.Clamp(sqrDist, 1f, 10f);

                float force = PullForce * (m1 * BlackholeMass) / sqrDist;

                force = Mathf.Clamp(force, MinimumForce, force);

                rigid.AddForce(ForceDir.normalized * force * Time.deltaTime);
            }

            yield return new WaitForSeconds(RefreshRate);
        }
    }
}
