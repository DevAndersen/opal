using Opal.Views;

namespace Opal.Events;

public interface IMouseButtonInputHandler
{
    bool AcceptsMouseButtonInput() => true;

    Task HandleMouseButtonInputAsync(MouseButtonInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken);
}
