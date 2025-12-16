using Opal.Rendering;

namespace Opal.Views;

public abstract class AsyncConsoleView : IAsyncConsoleView
{
    public virtual void Init()
    {
    }

    public abstract void Render(IConsoleGrid grid);

    public virtual ValueTask UpdateAsync(IConsoleState state, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
