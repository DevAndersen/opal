using Opal.Events;

namespace Opal.Views;

public interface IMouseMoveInputHandler
{
    bool AcceptsMouseMoveInput() => true;

    void HandleMouseMoveInput(MouseMoveInput mouseEvent, IConsoleState consoleState);
}
