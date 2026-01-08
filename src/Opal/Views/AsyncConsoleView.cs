using Opal.Rendering;

namespace Opal.Views;

public abstract class AsyncConsoleView : IAsyncConsoleView
{
    private bool _isInitialized;

    public async ValueTask InitializeViewAsync()
    {
        if (!_isInitialized)
        {
            await Initialize();
            _isInitialized = true;
        }
    }

    public virtual ValueTask Initialize()
    {
        return ValueTask.CompletedTask;
    }

    public abstract void Render(IConsoleGrid grid);

    public virtual ValueTask UpdateAsync(IConsoleState state, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
