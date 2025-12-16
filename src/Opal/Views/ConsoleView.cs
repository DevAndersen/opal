using Opal.Rendering;

namespace Opal.Views;

public abstract class ConsoleView : IConsoleView
{
    public virtual void Init()
    {
    }

    public abstract void Render(IConsoleGrid grid);

    public virtual void Update(IConsoleState state)
    {
    }
}
