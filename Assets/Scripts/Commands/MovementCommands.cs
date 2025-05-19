// controlTarget interact command
public class InteractCommand : ICommand
{
    private Controlable controlTarget;

    public InteractCommand(Controlable controlTarget)
    {
        this.controlTarget = controlTarget;
    }

    public void Execute()
    {
        controlTarget.Interact();
    }

    public void Undo()
    {
        // do nothing
    }
}
// controlTarget jump command
public class JumpCommand : ICommand
{
    private Controlable controlTarget;

    public JumpCommand(Controlable controlTarget)
    {
        this.controlTarget = controlTarget;
    }

    public void Execute()
    {
        controlTarget.Jump();
    }

    public void Undo()
    {
        // do nothing
    }
}
