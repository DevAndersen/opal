using DevAndersen.Opal.ConsoleHandlers;
using DevAndersen.Opal.Rendering;
using DevAndersen.Opal.Views;

namespace DevAndersen.Opal;

public class OpalController : IDisposable
{
    private readonly IConsoleHandler handler;
    private readonly ConsoleRenderer renderer;
    private readonly Stack<ConsoleView> viewStack;
    private readonly OpalSettings settings;
    private ConsoleGrid? grid;

    public static bool IsRunning { get; private set; }

    public OpalController(IConsoleHandler consoleHandler, OpalSettings settings)
    {
        handler = consoleHandler;
        renderer = new ConsoleRenderer(consoleHandler);
        viewStack = new Stack<ConsoleView>();
        this.settings = settings;
    }

    public OpalController(OpalSettings settings) : this(IConsoleHandler.CreateDefaultHandlerForCurrentPlatform(), settings)
    {
    }

    public OpalController(IConsoleHandler consoleHandler) : this(consoleHandler, OpalSettings.CreateFullscreen())
    {
    }

    public OpalController() : this(IConsoleHandler.CreateDefaultHandlerForCurrentPlatform(), OpalSettings.CreateFullscreen())
    {
    }

    public void Start(ConsoleView view)
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("Opal is already running.");
        }

        handler.Start(settings);
        handler.OnConsoleSizeChanged += HandleConsoleSizeChanged;
        viewStack.Push(view);
        IsRunning = true;
        Run();
    }

    public async Task StartAsync(ConsoleView view)
    {
        await Task.Run(() => Start(view));
    }

    public void Stop()
    {
        IsRunning = false;
        handler.OnConsoleSizeChanged -= HandleConsoleSizeChanged;
        handler.Stop();
    }

    private void Run()
    {
        while (IsRunning && viewStack.TryPeek(out ConsoleView? currentView))
        {
            bool initRun = false;
            if (currentView.OpalController == null)
            {
                initRun = true;
                currentView.OpalController = this;
                currentView.Handler = handler;
                currentView.Init();
            }

            if (!initRun)
            {
                if (currentView is IKeyboardInputHandler keyboardHandler && keyboardHandler.AcceptsKeyboardInput())
                {
                    ConsoleKeyInfo readKey = Console.ReadKey(true);
                    keyboardHandler.HandleKeyInput(new ConsoleKeyEvent(readKey));
                }
                else
                {
                    // Discard keyboard input if the current view is not listening.
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                }
            }

            if (GetCurrentView() == currentView)
            {
                currentView.Update();
            }

            if (GetCurrentView() == currentView)
            {
                RenderView(currentView);
            }

            if (currentView.Delay > 0)
            {
                Task.Delay(currentView.Delay).GetAwaiter().GetResult();
            }
        }

        Stop();
    }

    public ConsoleView? GetCurrentView()
    {
        return viewStack.TryPeek(out ConsoleView? view) ? view : null;
    }

    private void RenderView(ConsoleView? view)
    {
        if (view != null)
        {
            grid = GetConsoleGrid(handler, grid);
            view.Render(grid);
            RenderGrid(renderer, grid);
        }
    }

    internal void SetView(ConsoleView view, bool removeCurrentViewFromViewStack)
    {
        if (removeCurrentViewFromViewStack)
        {
            viewStack.Pop();
        }
        viewStack.Push(view);
    }

    internal void ExitCurrentView()
    {
        viewStack.Pop();
        RenderView(GetCurrentView());
    }

    private void HandleConsoleSizeChanged(object? sender, ConsoleSizeChangedEventArgs args)
    {
        grid = GetConsoleGrid(handler, grid);
        grid.SetSize(args.NewWidth, args.NewHeight);
        handler.Print(SequenceProvider.Wrap(SequenceProvider.EnableAlternateBuffer()));
        if (viewStack.TryPeek(out ConsoleView? currentView))
        {
            currentView.Render(grid);
        }
        RenderGrid(renderer, grid);
    }

    public static void RenderGrid(ConsoleRenderer renderer, ConsoleGrid grid)
    {
        renderer.Render(grid);
    }

    public static ConsoleGrid GetConsoleGrid(IConsoleHandler handler, ConsoleGrid? grid = null)
    {
        if (grid == null)
        {
            grid = new ConsoleGrid(handler.Width, handler.Height);
        }
        else
        {
            if (grid.Width != handler.Width || grid.Height != handler.Height)
            {
                grid.SetSize(handler.Width, handler.Height);
            }
            grid.Grid.Span.Clear();
        }
        return grid;
    }

    public void Dispose()
    {
        if (IsRunning)
        {
            Stop();
        }

        handler.Dispose();
        GC.SuppressFinalize(this);
    }
}
