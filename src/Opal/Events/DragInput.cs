namespace Opal.Events;

/// <summary>
/// Represents a mouse drag input.
/// </summary>
/// <param name="PressedButtons">All mouse buttons currently pressed down.</param>
/// <param name="Modifiers"></param>
/// <param name="X">The X coordinate of the input.</param>
/// <param name="Y">The Y coordinate of the input.</param>
/// <param name="DragStartX">The X coordinate of the start of the drag event.</param>
/// <param name="DragStartY">The Y coordinate of the start of the drag event.</param>
public record DragInput(MouseButtons PressedButtons, ConsoleModifiers Modifiers, int X, int Y, int DragStartX, int DragStartY) : IConsoleInput
{
    public bool Handled { get; set; }
}
