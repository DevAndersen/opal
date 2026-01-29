using Opal.Drawing;
using Opal.Events;
using Opal.Rendering;

namespace Opal.Forms.Controls;

public class Button : SelectableControl, IMouseButtonControl
{
    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? Text { get; set; }

    public Action<MouseButtonInput>? OnMouseDown { get; set; }

    public Func<MouseButtonInput, CancellationToken, Task>? OnMouseDownAsync { get; set; }

    public Action<MouseButtonInput>? OnMouseUp { get; set; }

    public Func<MouseButtonInput, CancellationToken, Task>? OnMouseUpAsync { get; set; }

    public override void Render(IConsoleGrid grid)
    {
        grid.DrawBox(0, 0, Width ?? grid.Width, Height ?? grid.Height, DrawStyle.RoundedDrawStyle, new ConsoleChar
        {
            ForegroundSimple = IsSelected ? ConsoleColor.White : ConsoleColor.DarkGray
        });

        grid.DrawString(1, 1, Text);
    }

    public override Rect GetDesiredSize(int width, int height)
    {
        return new Rect(PosX, PosY, Width ?? width, Height ?? height);
    }
}
