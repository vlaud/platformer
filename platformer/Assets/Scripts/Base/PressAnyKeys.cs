using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Rito.InputBindings;

public enum UIBlockParents
{
    BackOption,
    MenuButtons,
    Options,
    SubOptions,
}

public class PressAnyKeys : MonoBehaviour
{
    [SerializeField] private GameObject _mainmenu;

    [Header("AnyKey")]
    [SerializeField] private float pressAnyKeyBlinkTime = 1f;
    [SerializeField] private TMPro.TMP_Text pressAnyKey;
    bool anyKeyPressed = false;

    [Header("시간")]
    [SerializeField] private float MenuMoveTime = 1f;
    [SerializeField] private float TimeCounter = 0f;

    [Header("타이틀")]
    [SerializeField] private TMPro.TMP_Text title;

    [SerializeField] private Vector2 TitlePressKeyPos = Vector2.zero;

    [Header("메뉴")]
    [SerializeField] private RectTransform MenuButtons;
    [SerializeField] private Vector2 MenuButtonsPressKeyPos = Vector2.zero;

    //[SerializeField] private RectTransform[] UIBlocks;
    [SerializeField] private EnumArray<UIBlockParents, RectTransform> _UIBlocks;

    [SerializeField] private InputBindingManager InputBindingManager;

    private void Start()
    {
        //UIBlocks = new RectTransform[System.Enum.GetValues(typeof(UIBlockParents)).Length];
        _UIBlocks = new EnumArray<UIBlockParents, RectTransform>(System.Enum.GetValues(typeof(UIBlockParents)).Length);
        foreach (RectTransform rt in _mainmenu.GetComponentsInChildren<RectTransform>())
        {
            if (rt.gameObject.CompareTag("UIBlock"))
            {
                Debug.Log(rt.parent);
                if (rt.parent == MenuButtons)
                {
                    
                    _UIBlocks[UIBlockParents.MenuButtons] = rt;
                    //UIBlocks[(int)UIBlockParents.MenuButtons] = rt;
                }

            }
        }
    }

    void Update()
    {
        TitleBlink();
        if (Input.anyKeyDown && !anyKeyPressed)
        {
            anyKeyPressed = true;
            pressAnyKey.gameObject.SetActive(false);
            AnykeyPressedAction();
        }

        foreach (var key in InputBindingManager._binding.Bindings[UserAction.MoveLeft])
        {
            if (Input.GetKeyDown(key))
            {
                Debug.Log("MoveLeft Pressed"); 
            }

            if (Input.GetKey(key))
            {
                Debug.Log("MoveLeft Pressing");
            }
        }
    }

    private void CountTime()
    {
        TimeCounter = MenuMoveTime;
    }

    private void TitleBlink()
    {
        if (!anyKeyPressed)
        {
            pressAnyKey.alpha = Time.time % pressAnyKeyBlinkTime < pressAnyKeyBlinkTime * .5f ? 0f : 1f;
        }
    }

    private void AnykeyPressedAction()
    {
        SetUIBlock(UIBlockParents.MenuButtons, true);
        title.rectTransform.DOAnchorPos(TitlePressKeyPos, MenuMoveTime, false).SetEase(Ease.OutExpo);
        MenuButtons.DOAnchorPos(MenuButtonsPressKeyPos, MenuMoveTime, false)
            .SetEase(Ease.OutExpo)
            .OnComplete(() => SetUIBlock(UIBlockParents.MenuButtons, false));
    }

    private void SetUIBlock(UIBlockParents uib, bool v)
    {
        _UIBlocks[uib].gameObject.SetActive(v);
    }

    public void OnOptionsDown()
    {

    }

    public void OnBackMenus()
    {

    }

    public void OnKeySets()
    {

    }

    public void OnBackOptions()
    {

    }
}
