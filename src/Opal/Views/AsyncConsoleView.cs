using Opal.Rendering;

namespace Opal.Views;

public abstract class AsyncConsoleView : IAsyncConsoleView
{
    private bool _isInitialized;

    public async Task InitializeViewAsync(CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
        {
            await InitializeAsync(cancellationToken);
            _isInitialized = true;
        }
    }

    protected virtual Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public abstract void Render(IConsoleGrid grid);

    public virtual Task UpdateAsync(IConsoleState state, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
