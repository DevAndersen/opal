namespace Opal.Events;

/// <summary>
/// Represents a keyboard input.
/// </summary>
/// <param name="KeyInfo">The key info that the input represents.</param>
public record KeyInput : IConsoleInput
{
    public bool Handled { get; set; }

    /// <summary>
    /// The keyboard key corresponding to the input.
    /// </summary>
    public ConsoleKey Key { get; init; }

    /// <summary>
    /// The <see cref="char"/> representation of the input.
    /// </summary>
    public char KeyChar { get; init; }

    /// <summary>
    /// The keyboard modifiers applied to the input.
    /// </summary>
    public ConsoleModifiers Modifiers { get; init; }

    public KeyInput(ConsoleKeyInfo keyInfo)
    {
        Key = keyInfo.Key;
        KeyChar = keyInfo.KeyChar;
        Modifiers = keyInfo.Modifiers;
    }
}
