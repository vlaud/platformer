using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PressAnyKeys : MonoBehaviour
{
    [SerializeField] private GameObject _mainmenu;

    private Animator _menuAnimator;

    private void Awake()
    {
        _menuAnimator = _mainmenu.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.anyKeyDown && !_menuAnimator.GetBool("IsPressAnyKey"))
        {
            OnPressAnyKeys();
        }
    }

    private void OnPressAnyKeys()
    {
        _menuAnimator.CrossFade("Menu_MoveUp", 0.2f);
        _menuAnimator.SetBool("IsPressAnyKey", true);
    }

    public void OnOptionsDown()
    {
        _menuAnimator.CrossFade("To_Options", 0.2f);
    }

    public void OnBackMenus()
    {
        _menuAnimator.CrossFade("Back_Menus", 0.2f);
    }

    public void OnKeySets()
    {
        _menuAnimator.CrossFade("To_KeySets", 0.2f);
    }

    public void OnBackOptions()
    {
        _menuAnimator.CrossFade("Back_Options", 0.2f);
    }
}
