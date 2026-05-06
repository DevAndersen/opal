using Opal.Events;

namespace Opal.Forms.Controls;

public interface IMouseButtonUpControl
{
    ConsoleEventHandler<MouseButtonInput> OnMouseUp { get; }
}
