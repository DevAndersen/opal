using Opal.Drawing;
using Opal.Rendering;

namespace Opal.Forms.Controls;

public class GroupBox : IControl, IControlSingleParent
{
    public int PosX { get; set; }

    public int PosY { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public IControl? ChildControl { get; set; }

    public string? Text { get; set; }

    public Rect GetDesiredSize(IConsoleGrid grid)
    {
        return new Rect(PosX, PosY, Width ?? grid.Width, Height ?? grid.Height);
    }

    public void Render(IConsoleGrid grid)
    {
        int width = Width ?? grid.Width;
        int height = Height ?? grid.Height;

        grid.DrawBox(0, 0, width, height, DrawStyle.RoundedDrawStyle, new ConsoleChar
        {
            ForegroundSimple = ConsoleColor.DarkGray
        });

        grid.DrawString(2, 0, width - 4, Text, HorizontalAlignment.Left, new ConsoleChar
        {
            ForegroundSimple = ConsoleColor.Gray
        });

        if (ChildControl != null)
        {
            IConsoleGrid subgrid = grid.CreateSubgrid(2, 1, width - 4, height - 2);
            ChildControl.Render(subgrid);
        }
    }
}
