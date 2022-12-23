namespace DevAndersen.Opal.Rendering;

public readonly struct ConsoleChar
{
    #region Properties and fields

    public char Character { get; init; }

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

    #endregion

    #region Methods

    /// <summary>
    /// Returns <c>true</c> if the style of <c>this</c> and <c><paramref name="other"/></c> are the same. Otherwise, returns <c>false</c>.
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

    #endregion
}
