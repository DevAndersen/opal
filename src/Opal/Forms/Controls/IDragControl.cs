using Opal.Events;

namespace Opal.Forms.Controls;

public interface IDragControl
{
    ConsoleEventHandler<DragInput> OnDragStart { get; }

    ConsoleEventHandler<DragInput> OnDragMove { get; }

    ConsoleEventHandler<DragInput> OnDragStop { get; }
}
