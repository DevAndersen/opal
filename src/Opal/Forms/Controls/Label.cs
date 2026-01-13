using Opal.Drawing;
using Opal.Rendering;

namespace Opal.Forms.Controls;

public class Label : IControl
{
    public int PosX { get; set; }

    public int PosY { get; set; }

    public string? Text { get; set; }

    public virtual void Render(IConsoleGrid grid)
    {
        grid.DrawString(PosX, PosY, Text);
    }
}
