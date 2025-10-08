namespace Opal;

/// <summary>
/// Represents a mouse input.
/// </summary>
/// <param name="Action"></param>
/// <param name="Modifiers"></param>
/// <param name="X">The X coordinate of the input.</param>
/// <param name="Y">The Y coordinate of the input.</param>
public record struct MouseInput(MouseInputType InputType, ConsoleModifiers Modifiers, int X, int Y) : IConsoleInput;
