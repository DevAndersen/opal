namespace Opal.Views;

public interface IConsoleState
{
    int Height { get; }

    int Width { get; }

    void Goto(IConsoleView view);

    void GotoChild(IConsoleView view);

    void ExitView();

    void Exit();
}
