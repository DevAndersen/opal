using Opal.Events;

namespace Opal.Forms.Controls;

public interface IMouseButtonControl
{
    ConsoleEventHandler<MouseButtonInput> OnMouseDown { get; }

    ConsoleEventHandler<MouseButtonInput> OnMouseUp { get; }
}
