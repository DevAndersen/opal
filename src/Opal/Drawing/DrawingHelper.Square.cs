using Opal.Rendering;

namespace Opal.Drawing;

public static partial class DrawingHelper
{
    extension(IConsoleGrid grid)
    {
        public void DrawBox(int posX, int posY, int width, int height)
        {
            grid.DrawBox(posX, posY, width, height, DrawStyle.StandardDrawStyle, default);
        }

        public void DrawBox(int posX, int posY, int width, int height, DrawStyle style)
        {
            grid.DrawBox(posX, posY, width, height, style, default);
        }

        public void DrawBox(int posX, int posY, int width, int height, ConsoleChar template)
        {
            grid.DrawBox(posX, posY, width, height, DrawStyle.StandardDrawStyle, template);
        }

        public void DrawBox(int posX, int posY, int width, int height, DrawStyle style, ConsoleChar template)
        {
            grid[posX, posY] = template with { Character = style.TopLeftCorner };
            grid[posX + width - 1, posY] = template with { Character = style.TopRightCorner };
            grid[posX, posY + height - 1] = template with { Character = style.BottomLeftCorner };
            grid[posX + width - 1, posY + height - 1] = template with { Character = style.BottomRightCorner };

            for (int x = 1; x < width - 1; x++)
            {
                grid[posX + x, posY] = template with { Character = style.Horizontal };
                grid[posX + x, posY + height - 1] = template with { Character = style.Horizontal };
            }

            for (int y = 1; y < height - 1; y++)
            {
                grid[posX, posY + y] = template with { Character = style.Vertical };
                grid[posX + width - 1, posY + y] = template with { Character = style.Vertical };
            }
        }

        public void DrawFill(int posX, int posY, int width, int height, ConsoleChar template)
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
}
