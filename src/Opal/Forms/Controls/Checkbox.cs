using Opal.Drawing;
using Opal.Events;
using Opal.Rendering;

namespace Opal.Forms.Controls;

public class Checkbox : SelectableControl, IMouseButtonUpControl, IKeyControl
{
    public bool Value { get; set; }

    public ConsoleEventHandler<MouseButtonInput> OnMouseUp => new(_ => ChangeValue());

    public ConsoleEventHandler<KeyInput> OnKeyDown => new(_ => ChangeValue());

    private void ChangeValue()
    {
        Value = !Value;
    }

    public override Rect GetDesiredSize(int width, int height)
    {
        return new Rect(PosX, PosY, 1, 1);
    }

    public override void Render(IConsoleGrid grid)
    {
        grid[0, 0] = new ConsoleChar(Value ? CharLib.Shapes.BallotBoxWithLightX : CharLib.Shapes.BallotBox);
    }
}
