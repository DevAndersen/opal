namespace DevAndersen.Opal.Rendering;

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
    ForegroundSet = 0b_0001,

    /// <summary>
    /// The background has been specified.
    /// </summary>
    BackgroundSet = 0b_0010,

    /// <summary>
    /// The foreground should use 24-bit RGB colors, rather than simple 4-bit colors.
    /// </summary>
    ForegroundRgb = 0b_0100,

    /// <summary>
    /// The background should use 24-bit RGB colors, rather than simple 4-bit colors.
    /// </summary>
    BackgroundRgb = 0b_1000,
}
