using Opal.Events;

namespace Opal.Views;

public interface IMouseMoveInputHandler
{
    bool AcceptsMouseMoveInput() => true;

    Task HandleMouseMoveInputAsync(MouseMoveInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken);
}
