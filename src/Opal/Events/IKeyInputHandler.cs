using Opal.Views;

namespace Opal.Events;

/// <summary>
/// Defines a type that can accept keyboard input.
/// </summary>
public interface IKeyInputHandler
{
    bool AcceptsKeyInput() => true;

    Task HandleKeyInputAsync(KeyInput keyEvent, IConsoleState consoleState, CancellationToken cancellationToken);
}
