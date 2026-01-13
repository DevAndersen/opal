namespace Opal;

/// <summary>
/// Represents a keyboard input.
/// </summary>
/// <param name="KeyInfo">The key info that the input represents.</param>
public record KeyInput(ConsoleKeyInfo KeyInfo) : IConsoleInput
{
    public bool Handled { get; set; }

    /// <summary>
    /// The keyboard key corresponding to the input.
    /// </summary>
    public ConsoleKey Key => KeyInfo.Key;

    /// <summary>
    /// The keyboard modifiers applied to the input.
    /// </summary>
    public ConsoleModifiers Modifiers => KeyInfo.Modifiers;

    /// <summary>
    /// The <see cref="char"/> representation of the input.
    /// </summary>
    public char KeyChar => KeyInfo.KeyChar;
}
