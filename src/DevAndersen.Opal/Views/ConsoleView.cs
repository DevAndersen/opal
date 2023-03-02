using DevAndersen.Opal.ConsoleHandlers;
using DevAndersen.Opal.Rendering;

namespace DevAndersen.Opal.Views;

public abstract class ConsoleView : IRenderable
{
    internal OpalController? OpalController { get; set; }

    internal IConsoleHandler? Handler { get; set; }

    public virtual int Delay => 0;

    public int ConsoleWidth => Handler?.Width ?? 0;

    public int ConsoleHeight => Handler?.Height ?? 0;

    public virtual void Init()
    {
    }

    public virtual void Update()
    {
    }

    public abstract void Render(IConsoleGrid grid);

    protected void Goto(ConsoleView view)
    {
        OpalController?.SetView(view, true);
    }

    protected void GotoChild(ConsoleView view)
    {
        OpalController?.SetView(view, false);
    }

    protected void ExitView()
    {
        OpalController?.ExitCurrentView();
    }
}
