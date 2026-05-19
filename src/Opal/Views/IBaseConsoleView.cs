namespace Opal.Views;

public interface IBaseConsoleView : IRenderable
{
    Task InitializeViewAsync(CancellationToken cancellationToken = default);
}
