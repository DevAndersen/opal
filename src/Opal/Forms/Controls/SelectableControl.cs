using Opal.Rendering;

namespace Opal.Forms.Controls;

public abstract class SelectableControl : IControl, ISelectable
{
    public bool IsSelected { get; private set; }

    public int PosX { get; set; }

    public int PosY { get; set; }

    public virtual void FocusChange(bool isFocused)
    {
        IsSelected = isFocused;
    }

    public abstract void Render(IConsoleGrid grid);
}
