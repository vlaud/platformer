using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPedal : MonoBehaviour
{
    [SerializeField] private Color _originColor;
    [SerializeField] private Color _changeColor;
    [SerializeField] private LayerMask interactables;
    [SerializeField] private Transform _clickPedal;
    [SerializeField] private Transform[] targets;

    private void Update()
    {
        if(_clickPedal.localPosition.y < 0f)
        {
            _clickPedal.GetComponent<SpriteRenderer>().color = _changeColor;

            foreach (Transform target in targets)
                target.GetComponent<iButtonAction>().ButtonAction(true);
        }
        else
        {
            _clickPedal.GetComponent<SpriteRenderer>().color = _originColor;

            foreach (Transform target in targets)
                target.GetComponent<iButtonAction>().ButtonAction(false);
        }
    }
}

public interface iButtonAction
{
    void ButtonAction(bool v);
}
