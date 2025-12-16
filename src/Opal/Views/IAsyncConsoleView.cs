namespace Opal.Views;

public interface IAsyncConsoleView : IBaseConsoleView
{
    ValueTask UpdateAsync(IConsoleState state, CancellationToken cancellationToken);
}
