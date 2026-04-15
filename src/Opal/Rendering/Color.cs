using System.Diagnostics;

namespace Opal.Rendering;

/// <summary>
/// Represents either a <see cref="ConsoleColor"/> or an RGB color.
/// </summary>
[DebuggerDisplay("{GetDebuggerDisplay()}")]
public readonly struct Color
{
    /// <summary>
    /// Bit mask for determining if the color represents a 4-bit console color, or a 24-bit RGB color.
    /// </summary>
    private const int _rgbFlagMask = int.MinValue;

    /// <summary>
    /// Bit mask for the bits that are used to represent a 24-bit RGB color.
    /// </summary>
    private const int _rgbMask = 0x_00_ff_ff_ff;

    /// <summary>
    /// Bit mask for the bits that are used to represent a 4-bit console color.
    /// </summary>
    private const int _consoleColorMask = 0x_00_00_00_0f;

    /// <summary>
    /// The raw value of the color, including the RGB flag.
    /// </summary>
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

    public static implicit operator Color(ConsoleColor consoleColor) => new Color(consoleColor);

    public static implicit operator Color(int rgb) => new Color(rgb);

    internal string GetDebuggerDisplay()
    {
        if (IsRgb)
        {
            return "0x" + Convert.ToString(RgbValue, 16);
        }
        else
        {
            return ConsoleColorValue.ToString();
        }
    }
}
