namespace Opal;

/// <summary>
/// Represents a keyboard input.
/// </summary>
/// <param name="KeyInfo">The key info that the input represents.</param>
public record struct KeyInput(ConsoleKeyInfo KeyInfo) : IConsoleInput
{
    /// <summary>
    /// The keyboard key corresponding to the input.
    /// </summary>
    public readonly ConsoleKey Key => KeyInfo.Key;

    /// <summary>
    /// The keyboard modifiers applied to the input.
    /// </summary>
    public readonly ConsoleModifiers Modifiers => KeyInfo.Modifiers;

    /// <summary>
    /// The <see cref="char"/> representation of the input.
    /// </summary>
    public readonly char KeyChar => KeyInfo.KeyChar;
}
