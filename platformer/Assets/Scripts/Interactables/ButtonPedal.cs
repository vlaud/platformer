using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPedal : MonoBehaviour
{
    [SerializeField] private Color _originColor;
    [SerializeField] private Color _changeColor;
    [SerializeField] private LayerMask interactables;
    [SerializeField] private Transform _clickPedal;
    [SerializeField] private Portal _portal;

    private void Update()
    {
        if(_clickPedal.localPosition.y < 0f)
        {
            _clickPedal.GetComponent<SpriteRenderer>().color = _changeColor;
            _portal.SetGate(true);
        }
        else
        {
            _clickPedal.GetComponent<SpriteRenderer>().color = _originColor;
            _portal.SetGate(false);
        }
    }
}