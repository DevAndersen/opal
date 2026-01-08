using Opal.Rendering;

namespace Opal.Views;

public abstract class ConsoleView : IConsoleView
{
    private bool _isInitialized;

    public ValueTask InitializeViewAsync()
    {
        if (!_isInitialized)
        {
            Initialize();
            _isInitialized = true;
        }

        return ValueTask.CompletedTask;
    }

    public virtual void Initialize()
    {
    }

    public abstract void Render(IConsoleGrid grid);

    public virtual void Update(IConsoleState state)
    {
    }
}
