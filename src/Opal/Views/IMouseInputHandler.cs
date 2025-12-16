namespace Opal.Views;

/// <summary>
/// Defines a type that can accept mouse input.
/// </summary>
public interface IMouseInputHandler
{
    public bool AcceptsMouseInput() => true;

    public void HandleMouseInput(MouseInput mouseEvent, IConsoleState consoleState);
}
