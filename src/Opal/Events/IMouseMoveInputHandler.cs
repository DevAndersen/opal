using Opal.Views;

namespace Opal.Events;

public interface IMouseMoveInputHandler
{
    bool AcceptsMouseMoveInput() => true;

    Task HandleMouseMoveInputAsync(MouseMoveInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken);
}
