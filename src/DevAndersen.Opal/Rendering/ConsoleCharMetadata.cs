namespace DevAndersen.Opal.Rendering;

/// <summary>
/// Metadata about the state of the owning <c><see cref="ConsoleChar"/></c>.
/// </summary>
[Flags]
internal enum ConsoleCharMetadata : byte
{
    /// <summary>
    /// No metadata has been specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// The foreground has been specified.
    /// </summary>
    ForegroundSet = 0b_0000_0001,

    /// <summary>
    /// The background has been specified.
    /// </summary>
    BackgroundSet = 0b_0000_0010,

    /// <summary>
    /// The foreground should use 24-bit RGB colors, rather than simple 4-bit colors.
    /// </summary>
    ForegroundRgb = 0b_0000_0100,

    /// <summary>
    /// The background should use 24-bit RGB colors, rather than simple 4-bit colors.
    /// </summary>
    BackgroundRgb = 0b_0000_1000,

    /// <summary>
    /// Print <c><see cref="ConsoleChar.Sequence"/></c> instead of <c><see cref="ConsoleChar.Character"/></c>.
    /// </summary>
    UseStringCache = 0b_0001_0000,
}
