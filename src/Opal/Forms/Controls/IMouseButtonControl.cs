using Opal.Events;

namespace Opal.Forms.Controls;

public interface IMouseButtonControl
{
    Action<MouseButtonInput>? OnMouseDown { get; set; }
    
    Func<MouseButtonInput, CancellationToken, Task>? OnMouseDownAsync { get; set; }
    
    Action<MouseButtonInput>? OnMouseUp { get; set; }
    
    Func<MouseButtonInput, CancellationToken, Task>? OnMouseUpAsync { get; set; }
}
