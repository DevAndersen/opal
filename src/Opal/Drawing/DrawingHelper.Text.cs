using Opal.Rendering;

namespace Opal.Drawing;

public static partial class DrawingHelper
{
    public static void DrawString(this IConsoleGrid grid, int posX, int posY, ReadOnlySpan<char> text)
    {
        DrawString(grid, posX, posY, text, default);
    }

    public static void DrawString(this IConsoleGrid grid, int posX, int posY, ReadOnlySpan<char> text, ConsoleChar template)
    {
        if (!text.IsEmpty)
        {
            for (int i = 0; i < text.Length; i++)
            {
                grid[posX + i, posY] = template with { Character = GetSafeChar(text[i]) };
            }
        }
    }

    public static void DrawWrappingString(this IConsoleGrid grid, int posX, int posY, int width, string text)
    {
        DrawWrappingString(grid, posX, posY, width, int.MaxValue, text, default);
    }

    public static void DrawWrappingString(this IConsoleGrid grid, int posX, int posY, int width, int height, string text, ConsoleChar template)
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
}
