namespace DevAndersen.Opal.Rendering;

/// <summary>
/// The modes (visual styles and effects) of the owning <c><see cref="ConsoleChar"/></c>.
/// </summary>
[Flags]
public enum ConsoleCharModes : byte
{
    /// <summary>
    /// No modes have been specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// The character is italic.
    /// </summary>
    Italic = 0b_0000_0001,

    /// <summary>
    /// The character is underscored.
    /// </summary>
    Underscore = 0b_0000_0010,

    /// <summary>
    /// The character is double-underscored.
    /// </summary>
    DoubleUnderscore = 0b_0000_0100,

    /// <summary>
    /// The character is striked through.
    /// </summary>
    Strike = 0b_0000_1000,

    /// <summary>
    /// The character is blinking.
    /// </summary>
    Blinking = 0b_0001_0000,
}
