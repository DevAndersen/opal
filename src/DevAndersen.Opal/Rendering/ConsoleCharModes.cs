namespace DevAndersen.Opal.Rendering;

[Flags]
public enum ConsoleCharModes : byte
{
    None             = 0,
    Italic           = 0b_0000_0001,
    Underscore       = 0b_0000_0010,
    DoubleUnderscore = 0b_0000_0100,
    Strike           = 0b_0000_1000,
    Blinking         = 0b_0001_0000,
}
