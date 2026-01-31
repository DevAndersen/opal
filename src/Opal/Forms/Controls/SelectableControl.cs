using Opal.Events;
using Opal.Rendering;

namespace Opal.Forms.Controls;

public abstract class SelectableControl : IControl, ISelectable
{
    public int? Index { get; set; }

    public int PosX { get; set; }

    public int PosY { get; set; }

    public bool IsSelected { get; set; }

    public ConsoleEventHandler OnSelect { get; }

    public ConsoleEventHandler OnUnselect { get; }

    public abstract void Render(IConsoleGrid grid);

    public abstract Rect GetDesiredSize(int width, int height);
}
