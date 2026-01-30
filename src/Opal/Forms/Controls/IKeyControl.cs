using Opal.Events;

namespace Opal.Forms.Controls;

public interface IKeyControl
{
    ConsoleEventHandler<KeyInput> OnKeyDown { get; }
}
