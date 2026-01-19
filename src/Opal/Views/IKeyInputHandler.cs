using Opal.Events;

namespace Opal.Views;

/// <summary>
/// Defines a type that can accept keyboard input.
/// </summary>
public interface IKeyInputHandler
{
    bool AcceptsKeyInput() => true;

    void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState);
}
