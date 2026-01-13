namespace Opal.Forms;

public interface ISelectable
{
    int? Index { get; set; }

    bool IsSelected { get; }

    void SelectionChange(bool isSelected);
}
