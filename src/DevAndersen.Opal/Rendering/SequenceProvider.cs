namespace DevAndersen.Opal.Rendering;

public class SequenceProvider
{
    public const char Escape = (char)27;

    public static void Wrap(StringBuilder sb, int startIndex = 0) => sb.Insert(startIndex, $"{Escape}[").Append('m');

    public static void AppendWrapped(StringBuilder sb, string sequence, string ending = "m") => sb.Append($"{Escape}[{sequence}{ending}");

    public static string Wrap(string str) => $"{Escape}[{str}m";

    public static string Reset() => "0";

    public static string Italic(bool state) => state ? "3" : "23";

    public static string Underscore(bool state) => state ? "4" : "24";

    public static string DoubleUnderscore(bool state) => state ? "21" : "24";

    public static string Strike(bool state) => state ? "9" : "29";

    public static string Blinking(bool state) => state ? "5" : "25";

    public static string ForegroundRgb(byte r, byte g, byte b) => $"38;2;{r};{g};{b}";

    public static string BackgroundRgb(byte r, byte g, byte b) => $"48;2;{r};{g};{b}";

    public static string ForegroundSimple(ConsoleColor color) => GetColorCodeFromConsoleColor(color).ToString();

    public static string BackgroundSimple(ConsoleColor color) => (GetColorCodeFromConsoleColor(color) + 10).ToString();

    public static string ResetForeground() => "39";

    public static string ResetBackground() => "49";

    public static string SetCursorPosition(int x, int y) => $"{y};{x}";

    public static string EnableAlternateBuffer() => $"{Escape}[?1049h";

    public static string DisableAlternateBuffer() => $"{Escape}[?1049l";

    private static int GetColorCodeFromConsoleColor(ConsoleColor color)
    {
        return color switch
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
}
