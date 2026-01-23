using Opal.Rendering;

namespace Opal.Views;

public abstract class AsyncConsoleView : IAsyncConsoleView
{
    private bool _isInitialized;

    public async Task InitializeViewAsync()
    {
        if (!_isInitialized)
        {
            await Initialize();
            _isInitialized = true;
        }
    }

    public virtual Task Initialize()
    {
        return Task.CompletedTask;
    }

    public abstract void Render(IConsoleGrid grid);

    public virtual Task UpdateAsync(IConsoleState state, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
