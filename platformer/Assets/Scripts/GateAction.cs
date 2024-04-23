using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Portal))]
public class GateAction : Controlable
{
    public bool IsEntrance = false;
    public GateAction ConnectedGate;

    private Portal portal;

    private void Awake()
    {
        portal = GetComponent<Portal>();
        if (IsEntrance) CreateConnection(this, false);
    }

    private void Update()
    {
        if (!IsEntrance)
        {
            portal.ButtonAction(ConnectedGate.portal.IsPortal);
        }
    }

    public void CreateConnection(GateAction gate, bool v)
    {
        ConnectedGate.ConnectedGate = gate;
        ConnectedGate.IsEntrance = v;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && portal.IsPortal)
        {
            if (IsEntrance)
            {
                Debug.Log("gateIn");
                collision.GetComponent<PlayerMovement>().ToGate(this);
            }
            else
            {
                Debug.Log("gateOut");
                collision.GetComponent<PlayerMovement>().OutGate();
            }
        }
    }

    public override void Move(Vector2 input)
    {
        //throw new System.NotImplementedException();
    }

    public override void Rotate(Vector2 input)
    {
        //throw new System.NotImplementedException();
    }

    public override void Interact()
    {
        //throw new System.NotImplementedException();
    }

    public override void Jump()
    {
        //throw new System.NotImplementedException();
    }
}
