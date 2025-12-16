namespace Opal.Views;

internal class ConsoleState : IConsoleState
{
    public int Height { get; private set; }

    public int Width { get; private set; }

    public (IConsoleView View, bool RemoveCurrentViewFromViewStack)? NextViewState { get; private set; }

    public bool HasExitViewBeenRequested { get; private set; }

    public bool HasExitBeenRequested { get; private set; }

    public bool HaltViewExecution => NextViewState != null
             || HasExitViewBeenRequested
             || HasExitBeenRequested;

    internal void Reset(int height, int width)
    {
        Height = height;
        Width = width;
        NextViewState = null;
        HasExitViewBeenRequested = false;
    }

    public void Goto(IConsoleView view)
    {
        NextViewState = (view, true);
    }

    public void GotoChild(IConsoleView view)
    {
        NextViewState = (view, false);
    }

    public void ExitView()
    {
        HasExitViewBeenRequested = true;
    }

    public void Exit()
    {
        HasExitBeenRequested = true;
    }
}
