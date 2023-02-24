namespace DevAndersen.Opal.Rendering;

/// <summary>
/// Represents a complex console character, including color, styles, and effects.
/// </summary>
public readonly struct ConsoleChar
{
    #region Properties and fields

    private readonly char character;

    /// <summary>
    /// The UTF-16 character that the console character should print as.
    /// </summary>
    /// <remarks>
    /// If <c><see cref="Sequence"/></c> has been set, this contains the string's index in <c><see cref="ConsoleCharStringCache"/></c>.
    /// </remarks>
    public char Character
    {
        get => character;
        init
        {
            character = value;
            Metadata &= ~ConsoleCharMetadata.UseStringCache;
        }
    }

    /// <summary>
    /// The string that the console character should print as.
    /// </summary>
    /// <remarks>
    /// This is intended to be used in cases where the desired symbols/characters consist of multiple UTF-16 characters.
    /// </remarks>
    public string? Sequence
    {
        get => RenderAsString() ? ConsoleCharStringCache.Get(Character) : null;
        init
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Character = (char)ConsoleCharStringCache.Add(value);
                Metadata |= ConsoleCharMetadata.UseStringCache;
            }
        }
    }

    private readonly byte foregroundSimple;
    public ConsoleColor ForegroundSimple
    {
        get => (ConsoleColor)foregroundSimple;
        init
        {
            foregroundSimple = (byte)value;
            Metadata |= ConsoleCharMetadata.ForegroundSet;
        }
    }

    private readonly byte backgroundSimple;
    public ConsoleColor BackgroundSimple
    {
        get => (ConsoleColor)backgroundSimple;
        init
        {
            backgroundSimple = (byte)value;
            Metadata |= ConsoleCharMetadata.BackgroundSet;
        }
    }

    internal readonly byte ForegroundRed { get; private init; }

    internal readonly byte ForegroundGreen { get; private init; }

    internal readonly byte ForegroundBlue { get; private init; }

    public int ForegroundRgb
    {
        init
        {
            ForegroundRed = (byte)(value >> 16);
            ForegroundGreen = (byte)(value >> 8);
            ForegroundBlue = (byte)value;
            Metadata |= ConsoleCharMetadata.ForegroundSet | ConsoleCharMetadata.ForegroundRgb;
        }
        get => (ForegroundRed << 16) + (ForegroundGreen << 8) + ForegroundBlue;
    }

    internal readonly byte BackgroundRed { get; private init; }

    internal readonly byte BackgroundGreen { get; private init; }

    internal readonly byte BackgroundBlue { get; private init; }

    public int BackgroundRgb
    {
        init
        {
            BackgroundRed = (byte)(value >> 16);
            BackgroundGreen = (byte)(value >> 8);
            BackgroundBlue = (byte)value;
            Metadata |= ConsoleCharMetadata.BackgroundSet | ConsoleCharMetadata.BackgroundRgb;
        }
        get => (BackgroundRed << 16) + (BackgroundGreen << 8) + BackgroundBlue;
    }

    internal readonly ConsoleCharMetadata Metadata { get; private init; }

    public ConsoleCharModes Modes { get; init; }

    #endregion

    #region Constructors

    public ConsoleChar(char character)
    {
        Character = character;
    }

    public ConsoleChar(char character, ConsoleColor foregroundColor) : this(character)
    {
        ForegroundSimple = foregroundColor;
    }

    public ConsoleChar(char character, int foregroundColor) : this(character)
    {
        ForegroundRgb = foregroundColor;
    }

    public ConsoleChar(char character, ConsoleColor foregroundColor, ConsoleColor backgroundColor) : this(character, foregroundColor)
    {
        BackgroundSimple = backgroundColor;
    }

    public ConsoleChar(char character, ConsoleColor foregroundColor, int backgroundColor) : this(character, foregroundColor)
    {
        BackgroundRgb = backgroundColor;
    }

    public ConsoleChar(char character, int foregroundColor, ConsoleColor backgroundColor) : this(character, foregroundColor)
    {
        BackgroundSimple = backgroundColor;
    }

    public ConsoleChar(char character, int foregroundColor, int backgroundColor) : this(character, foregroundColor)
    {
        BackgroundRgb = backgroundColor;
    }

    public ConsoleChar(string sequence)
    {
        Sequence = sequence;
    }

    public ConsoleChar(string sequence, ConsoleColor foregroundColor) : this(sequence)
    {
        ForegroundSimple = foregroundColor;
    }

    public ConsoleChar(string sequence, int foregroundColor) : this(sequence)
    {
        ForegroundRgb = foregroundColor;
    }

    public ConsoleChar(string sequence, ConsoleColor foregroundColor, ConsoleColor backgroundColor) : this(sequence, foregroundColor)
    {
        BackgroundSimple = backgroundColor;
    }

    public ConsoleChar(string sequence, ConsoleColor foregroundColor, int backgroundColor) : this(sequence, foregroundColor)
    {
        BackgroundRgb = backgroundColor;
    }

    public ConsoleChar(string sequence, int foregroundColor, ConsoleColor backgroundColor) : this(sequence, foregroundColor)
    {
        BackgroundSimple = backgroundColor;
    }

    public ConsoleChar(string sequence, int foregroundColor, int backgroundColor) : this(sequence, foregroundColor)
    {
        BackgroundRgb = backgroundColor;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns <c>true</c> if the styling of <c>this</c> and <c><paramref name="other"/></c> are identical. Otherwise, returns <c>false</c>.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool HasSameStylingAs(ConsoleChar other)
    {
        return Modes == other.Modes
            && Metadata == other.Metadata
            && ForegroundSimple == other.ForegroundSimple
            && ForegroundRgb == other.ForegroundRgb
            && BackgroundSimple == other.BackgroundSimple
            && BackgroundRgb == other.BackgroundRgb;
    }

    public bool RenderAsString()
    {
        return (Metadata & ConsoleCharMetadata.UseStringCache) == ConsoleCharMetadata.UseStringCache;
    }

    #endregion
}
