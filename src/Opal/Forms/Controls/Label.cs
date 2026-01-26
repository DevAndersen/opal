using Opal.Drawing;
using Opal.Rendering;

namespace Opal.Forms.Controls;

public class Label : IControl
{
    public int PosX { get; set; }

    public int PosY { get; set; }

    public string? Text { get; set; }

    public Rect GetDesiredSize(int width, int height)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return default;
        }

        return new Rect(PosX, PosY, Text.Length, 1);
    }

    public virtual void Render(IConsoleGrid grid)
    {
        grid.DrawString(0, 0, Text);
    }
}
