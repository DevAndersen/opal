using Opal.Forms.Controls;

namespace Opal.Forms;

public interface IControlMultiParent
{
    IList<IControl> ChildControls { get; }
}
