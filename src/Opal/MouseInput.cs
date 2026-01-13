namespace Opal;

/// <summary>
/// Represents a mouse input.
/// </summary>
/// <param name="InputType"></param>
/// <param name="Modifiers"></param>
/// <param name="X">The X coordinate of the input.</param>
/// <param name="Y">The Y coordinate of the input.</param>
public record MouseInput(MouseInputType InputType, ConsoleModifiers Modifiers, int X, int Y) : IConsoleInput
{
    public bool Handled { get; set; }
}
