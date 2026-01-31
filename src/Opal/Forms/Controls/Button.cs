using Opal.Drawing;
using Opal.Events;
using Opal.Rendering;

namespace Opal.Forms.Controls;

public class Button : SelectableControl, IMouseButtonControl, IKeyControl, IMouseHoverControl
{
    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? Text { get; set; }

    public ConsoleEventHandler<MouseButtonInput> OnMouseDown => OnClick;

    public ConsoleEventHandler<MouseButtonInput> OnMouseUp => OnClick;

    public ConsoleEventHandler<KeyInput> OnKeyDown => (OnClick, keyInput => keyInput.Key == ConsoleKey.Enter);

    public ConsoleEventHandler<MouseMoveInput> OnMouseEnter { get; set; }

    public ConsoleEventHandler<MouseMoveInput> OnMouseLeave { get; set; }

    public ConsoleEventHandler OnClick { get; set; }

    public bool IsHovered { get; set; }

    public Color BorderColor { get; set; } = ConsoleColor.DarkGray;

    public Color BorderColorHighlight { get; set; } = ConsoleColor.White;

    public Color TextColor { get; set; } = ConsoleColor.White;

    public Color TextColorHighlight { get; set; } = ConsoleColor.White;

    public override void Render(IConsoleGrid grid)
    {
        grid.DrawBox(0, 0, Width ?? grid.Width, Height ?? grid.Height, DrawStyle.RoundedDrawStyle, new ConsoleChar
        {
            ForegroundColor = IsSelected || IsHovered ? BorderColorHighlight : BorderColor
        });

        grid.DrawString(1, 1, Text, new ConsoleChar
        {
            ForegroundColor = IsSelected || IsHovered? TextColorHighlight : TextColor
        });
    }

    public override Rect GetDesiredSize(int width, int height)
    {
        return new Rect(PosX, PosY, Width ?? width, Height ?? height);
    }
}
