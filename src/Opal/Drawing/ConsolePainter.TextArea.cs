using Opal.Rendering;

namespace Opal.Drawing;

public static partial class ConsolePainter
{
    public static void DrawTextArea(IConsoleGrid grid, int posX, int posY, string text)
    {
        DrawTextArea(grid, posX, posY, text, default, 4);
    }

    public static void DrawTextArea(IConsoleGrid grid, int posX, int posY, string text, ConsoleChar template)
    {
        DrawTextArea(grid, posX, posY, text, template, 4);
    }

    public static void DrawTextArea(IConsoleGrid grid, int posX, int posY, string text, ConsoleChar template, int tabWidth)
    {
        string[] lines = text.Split(_newlineSequences, StringSplitOptions.None);

        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex].Replace("\t", new string('\t', tabWidth));

            for (int charIndex = 0; charIndex < line.Length; charIndex++)
            {
                grid[posX + charIndex, posY + lineIndex] = template with { Character = GetSafeChar(line[charIndex]) };
            }
        }
    }

    public static void DrawTextArea(IConsoleGrid grid, int posX, int posY, int width, int height, int viewOffsetX, int viewOffsetY, string text, ConsoleChar template)
    {
        string[] lines = text.Split(_newlineSequences, StringSplitOptions.None);

        for (int y = 0; y < height; y++)
        {
            int yOffset = y + viewOffsetY;
            if (yOffset >= 0 && yOffset < lines.Length)
            {
                string line = lines[y + viewOffsetY];

                for (int x = 0; x < width; x++)
                {
                    int xOffset = x + viewOffsetX;
                    if (xOffset >= 0 && xOffset < line.Length)
                    {
                        grid[posX + x, posY + y] = template with { Character = line[xOffset] };
                    }
                }
            }
        }
    }
}
