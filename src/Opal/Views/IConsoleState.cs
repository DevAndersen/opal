namespace Opal.Views;

public interface IConsoleState
{
    public int Height { get; }

    public int Width { get; }

    public void Goto(IConsoleView view);

    public void GotoChild(IConsoleView view);

    public void ExitView();

    public void Exit();
}
