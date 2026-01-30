namespace Opal.Views;

public interface IConsoleState
{
    /// <summary>
    /// The full height of the console view.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// The full width of the console view.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Go to a new view, without adding the current view to the view stack.
    /// </summary>
    /// <param name="view"></param>
    void Goto(IConsoleView view);

    /// <summary>
    /// Go to a new view, and add the current view to the view stack.
    /// </summary>
    /// <param name="view"></param>
    void GotoChild(IConsoleView view);

    /// <summary>
    /// Exit the current view.
    /// </summary>
    void ExitView();

    /// <summary>
    /// Exit Opal.
    /// </summary>
    void Exit();

    /// <summary>
    /// Exit Opal gracefully, and then rethrow <paramref name="e"/>.
    /// </summary>
    /// <param name="e"></param>
    void Exit(Exception e);
}
