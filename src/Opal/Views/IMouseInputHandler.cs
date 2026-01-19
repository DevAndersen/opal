using Opal.Events;

namespace Opal.Views;

/// <summary>
/// Defines a type that can accept mouse input.
/// </summary>
public interface IMouseInputHandler
{
    bool AcceptsMouseInput() => true;

    void HandleMouseInput(MouseInput mouseEvent, IConsoleState consoleState);
}
