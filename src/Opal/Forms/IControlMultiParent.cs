using Opal.Forms.Controls;

namespace Opal.Forms;

public interface IControlMultiParent
{
    IList<IControl> ChildControls { get; }

    IEnumerable<(IControl, Rect)> GetChildControlAreas(int width, int height);
}
