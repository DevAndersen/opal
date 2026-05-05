using Opal.Drawing;
using Opal.Rendering;

namespace Opal.Forms.Controls;

public class ProgressBar : IControl
{
    public int PosX { get; set; }

    public int PosY { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public float Value { get; set; }

    public float MinValue { get; set; } = 0;

    public float MaxValue { get; set; } = 100;

    public Rect GetDesiredSize(int width, int height)
    {
        return new Rect(PosX, PosY, Width, Height);
    }

    public void Render(IConsoleGrid grid)
    {
        // Border.
        grid.DrawBox(0, 0, Width, Height, DrawStyle.RoundedDrawStyle, new ConsoleChar { ForegroundSimple = ConsoleColor.DarkGray });

        // Text.
        string percentageString = $"{Value}".PadLeft(4);
        grid.DrawString(grid.Width / 2 - percentageString.Length / 2 - 1, 1, percentageString);

        // Progress.
        float progress = (Value - MinValue) / (MaxValue - MinValue);
        int indicatorWidth = Width - 2;
        float progressLength = progress * indicatorWidth;

        for (int i = 0; i < indicatorWidth; i++)
        {
            ConsoleColor color = i >= progressLength ? ConsoleColor.DarkGray : ConsoleColor.Green;
            grid[i + 1, 1] = new ConsoleChar(grid[i + 1, 1].Character, 0x010101, color);
        }
    }
}
