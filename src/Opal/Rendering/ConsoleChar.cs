namespace Opal.Rendering;

/// <summary>
/// Represents a complex console character, including optional colors, styling, and effects.
/// </summary>
public readonly struct ConsoleChar
{
    /// <summary>
    /// The UTF-16 character that the console character should print as.
    /// </summary>
    /// <remarks>
    /// If <c><see cref="Sequence"/></c> has been set, this contains the string's index in <c><see cref="ConsoleCharStringCache"/></c>.
    /// </remarks>
    public char Character
    {
        get => field;
        init
        {
            field = value;
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
        get => ShouldRenderAsString() ? ConsoleCharStringCache.Get(Character) : null;
        init
        {
            Character = (char)ConsoleCharStringCache.Add(value ?? string.Empty);
            Metadata |= ConsoleCharMetadata.UseStringCache;
        }
    }

    private readonly byte _foregroundSimple;
    public ConsoleColor ForegroundSimple
    {
        get => (ConsoleColor)_foregroundSimple;
        init
        {
            _foregroundSimple = (byte)value;
            Metadata |= ConsoleCharMetadata.ForegroundSet;
        }
    }

    private readonly byte _backgroundSimple;
    public ConsoleColor BackgroundSimple
    {
        get => (ConsoleColor)_backgroundSimple;
        init
        {
            _backgroundSimple = (byte)value;
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

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/>.
    /// </summary>
    /// <param name="character"></param>
    public ConsoleChar(char character)
    {
        Character = character;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a simple foreground color.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="foregroundColor"></param>
    public ConsoleChar(char character, ConsoleColor foregroundColor) : this(character)
    {
        ForegroundSimple = foregroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with an RGB foreground color.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="foregroundColor"></param>
    public ConsoleChar(char character, int foregroundColor) : this(character)
    {
        ForegroundRgb = foregroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a simple foreground color and a simple background color.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="foregroundColor"></param>
    /// <param name="backgroundColor"></param>
    public ConsoleChar(char character, ConsoleColor foregroundColor, ConsoleColor backgroundColor) : this(character, foregroundColor)
    {
        BackgroundSimple = backgroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a simple foreground color and an RGB background color.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="foregroundColor"></param>
    /// <param name="backgroundColor"></param>
    public ConsoleChar(char character, ConsoleColor foregroundColor, int backgroundColor) : this(character, foregroundColor)
    {
        BackgroundRgb = backgroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with an RGB foreground color and a simple background color.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="foregroundColor"></param>
    /// <param name="backgroundColor"></param>
    public ConsoleChar(char character, int foregroundColor, ConsoleColor backgroundColor) : this(character, foregroundColor)
    {
        BackgroundSimple = backgroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with an RGB foreground color and an RGB background color.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="foregroundColor"></param>
    /// <param name="backgroundColor"></param>
    public ConsoleChar(char character, int foregroundColor, int backgroundColor) : this(character, foregroundColor)
    {
        BackgroundRgb = backgroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a character sequence.
    /// </summary>
    /// <param name="sequence"></param>
    public ConsoleChar(string sequence)
    {
        Sequence = sequence;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a character sequence and a simple foreground color.
    /// </summary>
    /// <param name="sequence"></param>
    public ConsoleChar(string sequence, ConsoleColor foregroundColor) : this(sequence)
    {
        ForegroundSimple = foregroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a character sequence and an RGB foreground color.
    /// </summary>
    /// <param name="sequence"></param>
    public ConsoleChar(string sequence, int foregroundColor) : this(sequence)
    {
        ForegroundRgb = foregroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a character sequence, a simple foreground color, and a simple background color.
    /// </summary>
    /// <param name="sequence"></param>
    public ConsoleChar(string sequence, ConsoleColor foregroundColor, ConsoleColor backgroundColor) : this(sequence, foregroundColor)
    {
        BackgroundSimple = backgroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a character sequence, a simple foreground color, and an RGB background color.
    /// </summary>
    /// <param name="sequence"></param>
    public ConsoleChar(string sequence, ConsoleColor foregroundColor, int backgroundColor) : this(sequence, foregroundColor)
    {
        BackgroundRgb = backgroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a character sequence, an RGB foreground color, and a simple background color.
    /// </summary>
    /// <param name="sequence"></param>
    public ConsoleChar(string sequence, int foregroundColor, ConsoleColor backgroundColor) : this(sequence, foregroundColor)
    {
        BackgroundSimple = backgroundColor;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleChar"/> with a character sequence, an RGB foreground color, and an RGB background color.
    /// </summary>
    /// <param name="sequence"></param>
    public ConsoleChar(string sequence, int foregroundColor, int backgroundColor) : this(sequence, foregroundColor)
    {
        BackgroundRgb = backgroundColor;
    }

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

    public bool ShouldRenderAsString()
    {
        return (Metadata & ConsoleCharMetadata.UseStringCache) == ConsoleCharMetadata.UseStringCache;
    }
}
