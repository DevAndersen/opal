using Opal.Events;

namespace Opal.Views;

public interface IMouseButtonInputHandler
{
    bool AcceptsMouseButtonInput() => true;

    void HandleMouseButtonInput(MouseButtonInput mouseEvent, IConsoleState consoleState);
}
