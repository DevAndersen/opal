namespace Opal.Events;

/// <summary>
/// Represents a mouse button input.
/// </summary>
/// <param name="Button">The mouse button being pressed or released.</param>
/// <param name="IsPressed">The state of <paramref name="Button"/>.</param>
/// <param name="PressedButtons">All mouse buttons currently pressed down.</param>
/// <param name="Modifiers"></param>
/// <param name="X">The X coordinate of the input.</param>
/// <param name="Y">The Y coordinate of the input.</param>
public record MouseButtonInput(MouseButtons Button, bool IsPressed, MouseButtons PressedButtons, ConsoleModifiers Modifiers, int X, int Y) : IConsoleInput
{
    public bool Handled { get; set; }
}
