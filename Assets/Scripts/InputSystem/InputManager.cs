using System.Collections.Generic;
using UnityEngine;
using Rito.InputBindings;
using Axis = WindowsInput.Axis;
using Project.Tools.InterfaceHelp;

public class InputManager : MonoBehaviour, IInputManager
{
    [field: Header("키 바인딩")]
    [SerializeField]
    private InputBinding inputBinding = new InputBinding(false)
    {
        localDirectoryPath = @"Rito\2. Study\2021_0129_Input Binding\Presets",
        fileName = "BindingPreset",
        extName = "txt",
        id = "1"
    };

    [field: Header("타깃")]
    // controlTarget variable
    [SerializeField] private InterfaceHolder<IController> controller_;

    // variable to store keycodes with icommand
    private Dictionary<KeyCode, List<ICommand>> keyMapping = new Dictionary<KeyCode, List<ICommand>>();
    // variable to store reset keymapping
    private Dictionary<KeyCode, List<ICommand>> resetKeyMapping = new Dictionary<KeyCode, List<ICommand>>();

    // Axis variable for horizontal and vertical
    [SerializeField] private Axis horizontal = new Axis(0.001f, 3f, 3f);
    [SerializeField] private Axis vertical = new Axis(0.001f, 3f, 3f);

    [field: Header("움직임 값")]
    // Vector2 variable to store horizontal and vertical axis
    [SerializeField] private Vector2 moveAmount;
    // function to add keymapping
    public void AddKeyMapping(KeyCode key, ICommand command, ICommand resetCommand)
    {
        if (!keyMapping.ContainsKey(key))
        {
            keyMapping.Add(key, new List<ICommand>());
            resetKeyMapping.Add(key, new List<ICommand>());
        }

        keyMapping[key].Add(command);
        resetKeyMapping[key].Add(resetCommand);
    }

    public void RemoveKeyMapping(KeyCode key) { keyMapping.Remove(key); }

    // function to reset keymapping to default
    public void ResetKeyMapping()
    {
        keyMapping = new Dictionary<KeyCode, List<ICommand>>(resetKeyMapping);
    }

    // function to execute actions every keys in keymapping foreach loop
    public void ExecuteKeyMapping()
    {
        foreach (var key in keyMapping)
        {
            //if (Input.GetKeyDown(key.Key))
        }
    }

    public void SetBinding(InputBindingInfo info)
    {
        inputBinding.numberOfKeys = info.numberOfKeys;
        inputBinding.localDirectoryPath = info.localDirectoryPath;
        inputBinding.fileName = info.fileName;
        inputBinding.extName = info.extName;
        inputBinding.id = info.id;
        inputBinding.showDebug = info.showDebug;

        LoadPreset();
    }

    private void LoadPreset()
    {
        if (inputBinding.LoadFromFile() == false)
        {
            inputBinding.ResetAll();
            inputBinding.SaveToFile();
        }
    }

    private void Awake()
    {
        // when controlTarget is null, set controlTarget to player
        if (controller_.Value == null)
            controller_.SetValue(ComponentTypeFinder.FindFirstImplementing<IController>());
            
        var m = ComponentTypeFinder.FindFirstImplementing<ISceneLoadAction>();
        Debug.Log(m);
    }

    private void Update()
    {
        var lefts = inputBinding.Bindings[UserAction.MoveLeft];
        var rights = inputBinding.Bindings[UserAction.MoveRight];
        horizontal.UpdateAxisFromLegacyInput(rights, lefts, ref moveAmount.x);
        controller_.Value?.InputMoveAxis(moveAmount);

        var jumps = inputBinding.Bindings[UserAction.Jump];
        var interacts = inputBinding.Bindings[UserAction.Interact];

        foreach (var key in jumps)
        {
            if (Input.GetKeyDown(key))
            {
                controller_.Value?.InputJumpAction();
                break;
            }
        }

        foreach (var key in interacts)
        {
            if (Input.GetKeyDown(key))
            {
                controller_.Value?.InputInteractAction();
                break;
            }
        }
    }
}
