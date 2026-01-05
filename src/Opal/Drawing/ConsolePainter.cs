namespace Opal.Drawing;

public static partial class ConsolePainter
{
    private static readonly string[] _newlineSequences = ["\n", "\r\n"];

    private static char GetSafeChar(char c)
    {
        if (char.IsWhiteSpace(c) || char.IsControl(c))
        {
            return ' ';
        }
        return c;
    }
}
