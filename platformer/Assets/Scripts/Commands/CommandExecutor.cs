using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandExecutor
{
    void ExecuteCommand();
}

public class CommandExecutor : MonoBehaviour
{
    public ICommand command;

    public void SetCommand(ICommand command)
    {
        this.command = command;
    }

    public void ExecuteCommand()
    {
        command.Execute();
    }
}
