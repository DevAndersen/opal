namespace Opal.Views;

public interface IAsyncConsoleView : IBaseConsoleView
{
    Task UpdateAsync(IConsoleState state, CancellationToken cancellationToken);
}
