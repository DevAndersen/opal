using DevAndersen.Opal.Rendering;

namespace DevAndersen.Opal.Drawing;

public static class ConsolePainter
{
    #region DrawBox

    public static void DrawBox(ConsoleGrid grid, int posX, int posY, int width, int height)
    {
        DrawBox(grid, posX, posY, width, height, DrawStyle.StandardDrawStyle, default);
    }

    public static void DrawBox(ConsoleGrid grid, int posX, int posY, int width, int height, DrawStyle style)
    {
        DrawBox(grid, posX, posY, width, height, style, default);
    }

    public static void DrawBox(ConsoleGrid grid, int posX, int posY, int width, int height, ConsoleChar template)
    {
        DrawBox(grid, posX, posY, width, height, DrawStyle.StandardDrawStyle, template);
    }

    public static void DrawBox(ConsoleGrid grid, int posX, int posY, int width, int height, DrawStyle style, ConsoleChar template)
    {
        grid[posX, posY] = template with { Character = style.TopLeftCorner };
        grid[posX + width, posY] = template with { Character = style.TopRightCorner };
        grid[posX, posY + height] = template with { Character = style.BottomLeftCorner };
        grid[posX + width, posY + height] = template with { Character = style.BottomRightCorner };

        for (int x = 1; x < width; x++)
        {
            grid[posX + x, posY] = template with { Character = style.Horizontal };
            grid[posX + x, posY + height] = template with { Character = style.Horizontal };
        }

        for (int y = 1; y < height; y++)
        {
            grid[posX, posY + y] = template with { Character = style.Vertical };
            grid[posX + width, posY + y] = template with { Character = style.Vertical };
        }
    }

    #endregion

    #region DrawHorizontalLine

    public static void DrawHorizontalLine(ConsoleGrid grid, int posX, int posY, int length)
    {
        DrawHorizontalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, default);
    }

    public static void DrawHorizontalLine(ConsoleGrid grid, int posX, int posY, int length, DrawStyle style)
    {
        DrawHorizontalLine(grid, posX, posY, length, style, default);
    }

    public static void DrawHorizontalLine(ConsoleGrid grid, int posX, int posY, int length, ConsoleChar template)
    {
        DrawHorizontalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, template);
    }

    public static void DrawHorizontalLine(ConsoleGrid grid, int posX, int posY, int length, DrawStyle style, ConsoleChar template)
    {
        for (int x = 0; x < length; x++)
        {
            grid[posX + x, posY] = template with { Character = style.Horizontal };
        }
    }

    #endregion

    #region DrawVerticalLine

    public static void DrawVerticalLine(ConsoleGrid grid, int posX, int posY, int length)
    {
        DrawVerticalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, default);
    }

    public static void DrawVerticalLine(ConsoleGrid grid, int posX, int posY, int length, DrawStyle style)
    {
        DrawVerticalLine(grid, posX, posY, length, style, default);
    }

    public static void DrawVerticalLine(ConsoleGrid grid, int posX, int posY, int length, ConsoleChar template)
    {
        DrawVerticalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, template);
    }

    public static void DrawVerticalLine(ConsoleGrid grid, int posX, int posY, int length, DrawStyle style, ConsoleChar template)
    {
        for (int y = 0; y < length; y++)
        {
            grid[posX, posY + y] = template with { Character = style.Vertical };
        }
    }

    #endregion

    #region DrawString

    public static void DrawString(ConsoleGrid grid, int posX, int posY, string text)
    {
        DrawString(grid, posX, posY, text, default);
    }

    public static void DrawString(ConsoleGrid grid, int posX, int posY, string text, ConsoleChar template)
    {
        for (int i = 0; i < text.Length; i++)
        {
            grid[posX + i, posY] = template with { Character = GetSafeChar(text[i]) };
        }
    }

    #endregion

    #region DrawWrappingString

    public static void DrawWrappingString(ConsoleGrid grid, int posX, int posY, int width, string text)
    {
        DrawWrappingString(grid, posX, posY, width, int.MaxValue, text, default);
    }

    public static void DrawWrappingString(ConsoleGrid grid, int posX, int posY, int width, int height, string text, ConsoleChar template)
    {
        for (int i = 0; i < text.Length; i++)
        {
            int xOffset = i % width;
            int yOffset = i / width;

            if (yOffset >= height)
            {
                break;
            }

            grid[posX + xOffset, posY + yOffset] = template with { Character = GetSafeChar(text[i]) };
        }
    }

    #endregion

    #region DrawTextArea

    public static void DrawTextArea(ConsoleGrid grid, int posX, int posY, string text)
    {
        DrawTextArea(grid, posX, posY, text, default, 4);
    }

    public static void DrawTextArea(ConsoleGrid grid, int posX, int posY, string text, ConsoleChar template)
    {
        DrawTextArea(grid, posX, posY, text, template, 4);
    }

    public static void DrawTextArea(ConsoleGrid grid, int posX, int posY, string text, ConsoleChar template, int tabWidth)
    {
        string[] lines = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);

        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex].Replace("\t", new string('\t', tabWidth));

            for (int charIndex = 0; charIndex < line.Length; charIndex++)
            {
                grid[posX + charIndex, posY + lineIndex] = template with { Character = GetSafeChar(line[charIndex]) };
            }
        }
    }

    public static void DrawTextArea(ConsoleGrid grid, int posX, int posY, int width, int height, int viewOffsetX, int viewOffsetY, int wrapWidth, string text, ConsoleChar template)
    {
        string[] lines = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);

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

    #endregion

    #region Private methods

    private static char GetSafeChar(char c)
    {
        if (char.IsWhiteSpace(c) || char.IsControl(c))
        {
            return ' ';
        }
        return c;
    }

    #endregion
}
