using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private Rigidbody2D rb;
    private Ground _ground;
    [SerializeField] private float originMass;
    [SerializeField] private float flyingMass = 0.001f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        originMass = rb.mass;
    }

    private void Update()
    {
        if (rb.velocity.y > 0.2f && !_ground.OnGround)
            rb.mass = flyingMass;
        else
            rb.mass = originMass;
    }
}
