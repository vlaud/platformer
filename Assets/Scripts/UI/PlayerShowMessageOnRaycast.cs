using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShowMessageOnRaycast : RaycastBase
{
    [SerializeField] private Transform showTransform;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        _collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckObject();
    }

    public void Interact()
    {
        RaycastHit2D raycast = GetRay(interactableLayer, middle, out middle, midPos, out midPos);

    }

    private void CheckObject()
    {
        if (GameManager.Inst.Controller.controlTarget != GameManager.Inst.Player)
        {
            currentShowObject?.SetTextAppear(false);
            return;
        }

        RaycastHit2D ray = GetRay(interactableLayer, middle, out middle, midPos, out midPos);

        if (ray)
        {
            currentShowObject = ray.transform.GetComponent<IObjectAction>();
            showTransform = ray.transform;

            if (currentShowObject != null)
            {
                currentShowObject.SetMessages();
                currentShowObject.SetTextPosition(midPos);
                currentShowObject.SetTextAppear(true);
                return;
            }
        }
        else
        {
            currentShowObject?.SetTextAppear(false);
        }
    }
}
