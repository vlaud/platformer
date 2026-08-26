using System.Collections.Generic;
using Project.Tools.InterfaceHelp;
using UnityEngine.Events;
using UnityEngine;

public class InputBindingHolder : Singleton<InputBindingHolder>, ISceneLoadAction
{
    private InputBindingInfo _binding;
    private Dictionary<string, UnityAction> sceneBeforeAction = new Dictionary<string, UnityAction>();
    private Dictionary<string, UnityAction> sceneAfterAction = new Dictionary<string, UnityAction>();
    [SerializeField] private InterfaceHolder<IInputManager> inputManager_;
    public void SetSceneAction()
    {
        var m = ComponentTypeFinder.FindFirstImplementing<IInputManager>();
        inputManager_.SetValue(m);
        sceneAfterAction["Title"] = SetBinding;
        sceneAfterAction["Stage1"] = SetBinding;
        sceneAfterAction["SampleScene"] = SetBinding;
    }

    public void BeforAction(string sceneName)
    {
        if (!sceneBeforeAction.ContainsKey(sceneName)) return;
        sceneBeforeAction[sceneName].Invoke();
    }

    public void AfterAction(string sceneName)
    {
        if (!sceneAfterAction.ContainsKey(sceneName)) return;
        sceneAfterAction[sceneName].Invoke();
    }

    void Awake()
    {
        Initialize();
        DontDestroyOnLoad(this);
    }

    private void SetBinding()
    {
        _binding.numberOfKeys = 2;
        _binding.localDirectoryPath = @"Input_Binding\Presets";
        _binding.fileName = "BindingPreset";
        _binding.extName = "txt";
        _binding.id = "1";
        _binding.showDebug = true;

        inputManager_.Value?.SetBinding(_binding);
    }
}
