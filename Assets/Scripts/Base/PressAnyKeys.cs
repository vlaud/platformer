using System.Collections.Generic;
using DG.Tweening;
using Rito.InputBindings;
using UnityEngine;

public class PressAnyKeys : MonoBehaviour
{
    [SerializeField] private GameObject _mainmenu;

    [Header("AnyKey")]
    [SerializeField] private float pressAnyKeyBlinkTime = 1f;
    [SerializeField] private TMPro.TMP_Text pressAnyKey;
    bool anyKeyPressed = false;

    [Header("시간")]
    [SerializeField] private float MenuMoveTime = 0.5f;

    [Header("타이틀")]
    [SerializeField] private TMPro.TMP_Text title;

    [SerializeField] private Vector2 TitlePressKeyPos = Vector2.zero;

    private IUISubject curUI;
    private Dictionary<UIType, IUISubject> _UIMenus;
    private Stack<IUISubject> _UIStack;

    [SerializeField] private InputBindingManager InputBindingManager;

    private void Start()
    {
        _UIMenus = new Dictionary<UIType, IUISubject>();
        _UIStack = new Stack<IUISubject>();

        foreach (MonoBehaviour m in _mainmenu.GetComponentsInChildren<MonoBehaviour>())
        {
            if (m is IUISubject i)
            {
                i.Init(ref _UIMenus, MenuMoveTime);
            }
        }

        SetCurUITarget(UIType.MenuButtons);
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PopUI();
        }
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
        title.rectTransform.DOAnchorPos(TitlePressKeyPos, MenuMoveTime, false).SetEase(Ease.OutExpo);
        ShowUI();
    }

    private void ShowUI()
    {
        curUI.Activate();
    }

    private void HideUI()
    {
        curUI.Deactivate();
    }

    private void StackUI()
    {
        if (_UIStack.Count == 0)
        {
            _UIMenus[UIType.BackOption].Activate();
        }
        HideUI();
        _UIStack.Push(curUI);
    }

    private void PopUI()
    {
        if (_UIStack.Count == 0) return;
        
        HideUI();
        SetCurUITarget(_UIStack.Pop());
        ShowUI();
        if (_UIStack.Count == 0)
        {
            _UIMenus[UIType.BackOption].Deactivate();
        }
    }

    private void SetCurUITarget(UIType uib)
    {
        curUI = _UIMenus[uib];
    }

    private void SetCurUITarget(IUISubject s)
    {
        curUI = s;    
    }

    public void OnOptionsDown()
    {
        StackUI();
        SetCurUITarget(UIType.Options);
        ShowUI();
    }

    public void OnBackMenus()
    {
        PopUI();
    }

    public void OnKeySets()
    {
        StackUI();
        SetCurUITarget(UIType.SubOptions);
        ShowUI();
    }

    public void OnBackOptions()
    {
        PopUI();
    }

    public void StartGame()
    {
        SceneLoader.ChangeScene("Stage1");
    }

    public void ExitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
