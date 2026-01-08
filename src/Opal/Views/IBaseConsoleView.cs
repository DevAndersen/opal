namespace Opal.Views;

public interface IBaseConsoleView : IRenderable
{
    ValueTask InitializeViewAsync();
}
