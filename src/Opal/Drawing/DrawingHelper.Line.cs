using Opal.Rendering;

namespace Opal.Drawing;

public static partial class DrawingHelper
{
    extension(IConsoleGrid grid)
    {
        public void DrawHorizontalLine(int posX, int posY, int length)
        {
            grid.DrawHorizontalLine(posX, posY, length, DrawStyle.StandardDrawStyle, default);
        }

        public void DrawHorizontalLine(int posX, int posY, int length, DrawStyle style)
        {
            grid.DrawHorizontalLine(posX, posY, length, style, default);
        }

        public void DrawHorizontalLine(int posX, int posY, int length, ConsoleChar template)
        {
            grid.DrawHorizontalLine(posX, posY, length, DrawStyle.StandardDrawStyle, template);
        }

        public void DrawHorizontalLine(int posX, int posY, int length, DrawStyle style, ConsoleChar template)
        {
            for (int x = 0; x < length; x++)
            {
                grid[posX + x, posY] = template with { Character = style.Horizontal };
            }
        }

        public void DrawVerticalLine(int posX, int posY, int length)
        {
            grid.DrawVerticalLine(posX, posY, length, DrawStyle.StandardDrawStyle, default);
        }

        public void DrawVerticalLine(int posX, int posY, int length, DrawStyle style)
        {
            grid.DrawVerticalLine(posX, posY, length, style, default);
        }

        public void DrawVerticalLine(int posX, int posY, int length, ConsoleChar template)
        {
            grid.DrawVerticalLine(posX, posY, length, DrawStyle.StandardDrawStyle, template);
        }

        public void DrawVerticalLine(int posX, int posY, int length, DrawStyle style, ConsoleChar template)
        {
            for (int y = 0; y < length; y++)
            {
                grid[posX, posY + y] = template with { Character = style.Vertical };
            }
        }

        public void DrawLine(int posX1, int posY1, int posX2, int posY2, ConsoleChar template)
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
}
