using Opal.Events;

namespace Opal.Forms.Controls;

public interface IMouseButtonDownControl
{
    ConsoleEventHandler<MouseButtonInput> OnMouseDown { get; }
}
