using Opal.Events;

namespace Opal.Forms.Controls;

public interface IMouseHoverControl
{
     bool IsHovered { get; set; }

    ConsoleEventHandler<MouseMoveInput> OnMouseEnter { get; }

    ConsoleEventHandler<MouseMoveInput> OnMouseLeave { get; }
}
