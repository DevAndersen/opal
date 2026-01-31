using Opal.Events;
using Opal.Forms.Controls;

namespace Opal.Forms;

public interface ISelectable
{
    int? Index { get; set; }

    bool IsSelected { get; set; }

    ConsoleEventHandler OnSelect { get; }

    ConsoleEventHandler OnUnselect { get; }
}
