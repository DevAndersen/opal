namespace Opal.Rendering;

/// <summary>
/// Represents either a <see cref="ConsoleColor"/> or an RGB color.
/// </summary>
public readonly struct Color
{
    private const int _rgbFlagMask = int.MinValue;
    private const int _rgbMask = 0x_00_ff_ff_ff;
    private const int _consoleColorMask = 0x_00_00_00_0f;

    private readonly int _value;

    /// <summary>
    /// Determines if the held value is a <see cref="ConsoleColor"/> or an RGB color.
    /// </summary>
    public bool IsRgb => (_value & _rgbFlagMask) == _rgbFlagMask;

    /// <summary>
    /// Get the held value as a <see cref="ConsoleColor"/>.
    /// </summary>
    public ConsoleColor ConsoleColorValue => (ConsoleColor)(_value & _consoleColorMask);

    /// <summary>
    /// Get the held value as an RGB color.
    /// </summary>
    public int RgbValue => _value & _rgbMask;

    public Color(ConsoleColor value)
    {
        _value = (int)value;
    }

    public Color(int value)
    {
        _value = (value & _rgbMask) | _rgbFlagMask;
    }

    public static Color FromConsoleColor(ConsoleColor color)
    {
        return new Color(color);
    }

    public static Color FromRgb(int color)
    {
        return new Color(color);
    }
}
