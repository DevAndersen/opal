using Opal.Rendering;

namespace Opal.Drawing;

public static partial class DrawingHelper
{
    public static void DrawHorizontalLine(this IConsoleGrid grid, int posX, int posY, int length)
    {
        DrawHorizontalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, default);
    }

    public static void DrawHorizontalLine(this IConsoleGrid grid, int posX, int posY, int length, DrawStyle style)
    {
        DrawHorizontalLine(grid, posX, posY, length, style, default);
    }

    public static void DrawHorizontalLine(this IConsoleGrid grid, int posX, int posY, int length, ConsoleChar template)
    {
        DrawHorizontalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, template);
    }

    public static void DrawHorizontalLine(this IConsoleGrid grid, int posX, int posY, int length, DrawStyle style, ConsoleChar template)
    {
        for (int x = 0; x < length; x++)
        {
            grid[posX + x, posY] = template with { Character = style.Horizontal };
        }
    }

    public static void DrawVerticalLine(this IConsoleGrid grid, int posX, int posY, int length)
    {
        DrawVerticalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, default);
    }

    public static void DrawVerticalLine(this IConsoleGrid grid, int posX, int posY, int length, DrawStyle style)
    {
        DrawVerticalLine(grid, posX, posY, length, style, default);
    }

    public static void DrawVerticalLine(this IConsoleGrid grid, int posX, int posY, int length, ConsoleChar template)
    {
        DrawVerticalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, template);
    }

    public static void DrawVerticalLine(this IConsoleGrid grid, int posX, int posY, int length, DrawStyle style, ConsoleChar template)
    {
        for (int y = 0; y < length; y++)
        {
            grid[posX, posY + y] = template with { Character = style.Vertical };
        }
    }

    public static void DrawLine(this IConsoleGrid grid, int posX1, int posY1, int posX2, int posY2, ConsoleChar template)
    {
        float length = float.Sqrt(float.Pow(posX2 - posX1, 2) + float.Pow(posY2 - posY1, 2));

        for (int i = 0; i < length + 1; i++)
        {
            int x = (int)(float.Lerp(posX1, posX2, i / length) + 0.5F);
            int y = (int)(float.Lerp(posY1, posY2, i / length) + 0.5F);

            grid[x, y] = template;
        }
    }
}
