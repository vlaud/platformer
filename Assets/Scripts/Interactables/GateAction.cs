using System;
using UnityEngine;

[RequireComponent(typeof(Portal))]
public class GateAction : Controlable, IGateAction
{
    public bool IsEntrance = false;
    public bool toNextStage = false;
    public GateAction ConnectedGate_;

    private Portal portal;
    private Action<IGateAction> nextStageAction;

    private void Awake()
    {
        portal = GetComponent<Portal>();
        if (IsEntrance && ConnectedGate_ != null) CreateConnection(this, false);

        nextStageAction = toNextStage ? ToNextStage : null;
    }

    private void Update()
    {
        if (!IsEntrance)
        {
            portal.ButtonAction(ConnectedGate_.portal.IsPortal);
        }
    }

    private void ToNextStage(IGateAction gate)
    {
        SceneLoader.ChangeScene("Title");
    }
    
    public void CreateConnection(GateAction gate, bool v)
    {
        ConnectedGate_.ConnectedGate_ = gate;
        ConnectedGate_.IsEntrance = v;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && portal.IsPortal)
        {
            if (IsEntrance)
            {
                Debug.Log("gateIn");
                collision.gameObject.GetMostDerivedComponent<IGateSubject>().ToGate(this, nextStageAction);
            }
            else
            {
                if (GameManager.Inst.Controller.controlTarget == null) return;
                if (GameManager.Inst.Controller
                    .controlTarget.transform.GetType<GateAction>() == null) return;

                Debug.Log("gateOut");
                collision.gameObject.GetMostDerivedComponent<IGateSubject>().OutGate();
            }
        }
    }

    public void GateIn()
    {
        GameManager.Inst.Controller.ChangeControlTarget(this);
    }

    public IGateAction GetConnectedGate()
    {
        return ConnectedGate_;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public override void Move(Vector2 input)
    {

    }

    public override void Rotate(Vector2 input)
    {

    }

    public override void Interact()
    {

    }

    public override void Jump()
    {

    }
}
