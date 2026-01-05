using Opal.Rendering;

namespace Opal.Drawing;

public static partial class ConsolePainter
{
    public static void DrawBox(IConsoleGrid grid, int posX, int posY, int width, int height)
    {
        DrawBox(grid, posX, posY, width, height, DrawStyle.StandardDrawStyle, default);
    }

    public static void DrawBox(IConsoleGrid grid, int posX, int posY, int width, int height, DrawStyle style)
    {
        DrawBox(grid, posX, posY, width, height, style, default);
    }

    public static void DrawBox(IConsoleGrid grid, int posX, int posY, int width, int height, ConsoleChar template)
    {
        DrawBox(grid, posX, posY, width, height, DrawStyle.StandardDrawStyle, template);
    }

    public static void DrawBox(IConsoleGrid grid, int posX, int posY, int width, int height, DrawStyle style, ConsoleChar template)
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

    public static void DrawFill(IConsoleGrid grid, int posX, int posY, int width, int height, ConsoleChar template)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[posX + x, posY + y] = template;
            }
        }
    }
}
