namespace Opal.Events;

/// <summary>
///Represents a mouse button input.
/// </summary>
/// <param name="Button">The mouse button being pressed or released.</param>
/// <param name="PressedButtons">All mouse buttons currently pressed down.</param>
/// <param name="Modifiers"></param>
/// <param name="X">The X coordinate of the input.</param>
/// <param name="Y">The Y coordinate of the input.</param>
public record MouseButtonInput(MouseInputType Button, bool IsPressed, MouseInputType PressedButtons, ConsoleModifiers Modifiers, int X, int Y) : IConsoleInput
{
    public bool Handled { get; set; }
}
