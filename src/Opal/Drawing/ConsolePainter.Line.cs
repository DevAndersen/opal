using Opal.Rendering;

namespace Opal.Drawing;

public static partial class ConsolePainter
{
    public static void DrawHorizontalLine(IConsoleGrid grid, int posX, int posY, int length)
    {
        DrawHorizontalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, default);
    }

    public static void DrawHorizontalLine(IConsoleGrid grid, int posX, int posY, int length, DrawStyle style)
    {
        DrawHorizontalLine(grid, posX, posY, length, style, default);
    }

    public static void DrawHorizontalLine(IConsoleGrid grid, int posX, int posY, int length, ConsoleChar template)
    {
        DrawHorizontalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, template);
    }

    public static void DrawHorizontalLine(IConsoleGrid grid, int posX, int posY, int length, DrawStyle style, ConsoleChar template)
    {
        for (int x = 0; x < length; x++)
        {
            grid[posX + x, posY] = template with { Character = style.Horizontal };
        }
    }

    public static void DrawVerticalLine(IConsoleGrid grid, int posX, int posY, int length)
    {
        DrawVerticalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, default);
    }

    public static void DrawVerticalLine(IConsoleGrid grid, int posX, int posY, int length, DrawStyle style)
    {
        DrawVerticalLine(grid, posX, posY, length, style, default);
    }

    public static void DrawVerticalLine(IConsoleGrid grid, int posX, int posY, int length, ConsoleChar template)
    {
        DrawVerticalLine(grid, posX, posY, length, DrawStyle.StandardDrawStyle, template);
    }

    public static void DrawVerticalLine(IConsoleGrid grid, int posX, int posY, int length, DrawStyle style, ConsoleChar template)
    {
        for (int y = 0; y < length; y++)
        {
            grid[posX, posY + y] = template with { Character = style.Vertical };
        }
    }
}
