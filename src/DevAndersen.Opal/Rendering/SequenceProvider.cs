namespace DevAndersen.Opal.Rendering;

public static class SequenceProvider
{
    public const char Escape = (char)27;
    public const char SGREnding = 'm';
    public const char DelimiterCharacter = ';';

    public static StringBuilder AppendStart(this StringBuilder sb, ref bool firstEdit)
    {
        if (firstEdit)
        {
            firstEdit = false;
            return sb.AppendEscapeBracket();
        }
        else
        {
            return sb.Append(DelimiterCharacter);
        }
    }

    public static string Reset()
        => $"{Escape}[0m";

    public static string EnableAlternateBuffer()
        => $"{Escape}[?1049h";

    public static string DisableAlternateBuffer()
        => $"{Escape}[?1049l";

    public static StringBuilder AppendEscapeBracket(this StringBuilder sb)
        => sb
            .Append(Escape)
            .Append('[');

    public static StringBuilder AppendSGREnding(this StringBuilder sb)
        => sb.Append(SGREnding);

    public static StringBuilder AppendReset(this StringBuilder sb)
        => sb.Append('0');

    public static StringBuilder AppendItalic(this StringBuilder sb, bool state)
        => state
            ? sb.Append('3')
            : sb.Append("23");

    public static StringBuilder AppendUnderscore(this StringBuilder sb, bool state)
        => state
            ? sb.Append('4')
            : sb.Append("24");

    public static StringBuilder AppendDoubleUnderscore(this StringBuilder sb, bool state)
        => state
            ? sb.Append("21")
            : sb.Append("24");

    public static StringBuilder AppendStrike(this StringBuilder sb, bool state)
        => state
            ? sb.Append('9')
            : sb.Append("29");

    public static StringBuilder AppendBlinking(this StringBuilder sb, bool state)
        => state
            ? sb.Append('5')
            : sb.Append("25");

    public static StringBuilder AppendForegroundRgb(this StringBuilder sb, byte r, byte g, byte b)
        => sb
            .Append("38;2;")
            .Append(r)
            .Append(DelimiterCharacter)
            .Append(g)
            .Append(DelimiterCharacter)
            .Append(b);

    public static StringBuilder AppendBackgroundRgb(this StringBuilder sb, byte r, byte g, byte b)
        => sb
            .Append("48;2;")
            .Append(r)
            .Append(DelimiterCharacter)
            .Append(g)
            .Append(DelimiterCharacter)
            .Append(b);

    public static StringBuilder AppendForegroundSimple(this StringBuilder sb, ConsoleColor color)
        => sb.Append(GetColorCodeFromConsoleColor(color));

    public static StringBuilder AppendBackgroundSimple(this StringBuilder sb, ConsoleColor color)
        => sb.Append(GetColorCodeFromConsoleColor(color) + 10);

    public static StringBuilder AppendResetForeground(this StringBuilder sb)
        => sb.Append(39);

    public static StringBuilder AppendResetBackground(this StringBuilder sb)
        => sb.Append(49);

    public static StringBuilder AppendSetCursorPosition(this StringBuilder sb, int x, int y)
        => sb
            .Append(y + 1)
            .Append(DelimiterCharacter)
            .Append(x + 1);

    private static int GetColorCodeFromConsoleColor(ConsoleColor color) => color switch
    {
        ConsoleColor.Black => 30,
        ConsoleColor.DarkBlue => 34,
        ConsoleColor.DarkGreen => 32,
        ConsoleColor.DarkCyan => 36,
        ConsoleColor.DarkRed => 31,
        ConsoleColor.DarkMagenta => 35,
        ConsoleColor.DarkYellow => 33,
        ConsoleColor.Gray => 37,
        ConsoleColor.DarkGray => 90,
        ConsoleColor.Blue => 94,
        ConsoleColor.Green => 92,
        ConsoleColor.Cyan => 96,
        ConsoleColor.Red => 91,
        ConsoleColor.Magenta => 95,
        ConsoleColor.Yellow => 93,
        ConsoleColor.White => 97,
        _ => 39
    };
}
