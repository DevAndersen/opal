namespace Opal.Views;

/// <summary>
/// Defines a type that can accept keyboard input.
/// </summary>
public interface IKeyInputHandler
{
    public bool AcceptsKeyInput() => true;

    public void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState);
}
