using Opal.Events;

namespace Opal.Views;

public interface IMouseButtonInputHandler
{
    bool AcceptsMouseButtonInput() => true;

    Task HandleMouseButtonInputAsync(MouseButtonInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken);
}
