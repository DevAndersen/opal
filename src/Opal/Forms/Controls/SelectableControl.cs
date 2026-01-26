using Opal.Rendering;

namespace Opal.Forms.Controls;

public abstract class SelectableControl : IControl, ISelectable
{
    public int? Index { get; set; }

    public bool IsSelected { get; private set; }

    public int PosX { get; set; }

    public int PosY { get; set; }

    public virtual void SelectionChange(bool isSelected)
    {
        IsSelected = isSelected;
    }

    public abstract void Render(IConsoleGrid grid);

    public abstract Rect GetDesiredSize(int width, int height);
}
