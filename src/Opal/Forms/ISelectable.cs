namespace Opal.Forms;

public interface ISelectable
{
    bool IsSelected { get; }

    void FocusChange(bool isFocused);
}
