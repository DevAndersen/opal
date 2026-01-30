using Opal.Drawing;
using Opal.Events;
using Opal.Rendering;

namespace Opal.Forms.Controls;

public class Button : SelectableControl, IMouseButtonControl, IKeyControl
{
    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? Text { get; set; }

    public ConsoleEventHandler<MouseButtonInput> OnMouseDown => OnClick;

    public ConsoleEventHandler<MouseButtonInput> OnMouseUp => OnClick;

    public ConsoleEventHandler<KeyInput> OnKeyDown => (OnClick, keyInput => keyInput.Key == ConsoleKey.Enter);

    public ConsoleEventHandler OnClick { get; set; }

    public Color BorderColorUnselected { get; set; } = ConsoleColor.DarkGray;

    public Color BorderColorSelected { get; set; } = ConsoleColor.White;

    public Color TextColorSelected { get; set; } = ConsoleColor.White;

    public Color TextColorUnselected { get; set; } = ConsoleColor.White;

    public override void Render(IConsoleGrid grid)
    {
        grid.DrawBox(0, 0, Width ?? grid.Width, Height ?? grid.Height, DrawStyle.RoundedDrawStyle, new ConsoleChar
        {
            ForegroundColor = IsSelected ? BorderColorSelected : BorderColorUnselected
        });

        grid.DrawString(1, 1, Text, new ConsoleChar
        {
            ForegroundColor = IsSelected ? TextColorSelected : TextColorUnselected
        });
    }

    public override Rect GetDesiredSize(int width, int height)
    {
        return new Rect(PosX, PosY, Width ?? width, Height ?? height);
    }
}
