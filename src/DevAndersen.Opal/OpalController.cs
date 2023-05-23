using DevAndersen.Opal.ConsoleHandlers;
using DevAndersen.Opal.Rendering;
using DevAndersen.Opal.Views;
using System.Collections.Concurrent;

namespace DevAndersen.Opal;

public class OpalController : IDisposable
{
    /// <summary>
    /// The console handler.
    /// </summary>
    private readonly IConsoleHandler handler;

    /// <summary>
    /// The console renderer.
    /// </summary>
    private readonly ConsoleRenderer renderer;

    /// <summary>
    /// The stack of console views, allowing the exit of one view to return the user to the previous view.
    /// </summary>
    private readonly Stack<ConsoleView> viewStack;

    /// <summary>
    /// The input queue, containing user input to be processed with the next update of the view.
    /// </summary>
    private readonly ConcurrentQueue<IConsoleInput> inputQueue;

    /// <summary>
    /// The thread responsible for listening to user input and enqueue these input to <see cref="inputQueue"/>.
    /// </summary>
    private readonly Thread inputThread;

    /// <summary>
    /// The base settings that for Opal.
    /// </summary>
    private readonly OpalSettings settings;

    /// <summary>
    /// The current console grid, containing the data that will be printed to the console.
    /// </summary>
    private ConsoleGrid? grid;

    /// <summary>
    /// The previous state of <see cref="grid"/>.
    /// This is used to check if the console grid differs from the previous rendered grid, in order to avoid redundant prints.
    /// </summary>
    private ConsoleGrid? previousGrid;

    /// <summary>
    /// Returns <c>true</c> if an instance of <see cref="OpalController"/> currently running.
    /// This property be checked before invoking <see cref="Start"/> or <see cref="StartAsync"/>.
    /// </summary>
    public static bool IsRunning { get; private set; }

    public OpalController(IConsoleHandler consoleHandler, OpalSettings settings)
    {
        handler = consoleHandler;
        renderer = new ConsoleRenderer(consoleHandler);
        viewStack = new Stack<ConsoleView>();
        inputQueue = new ConcurrentQueue<IConsoleInput>();
        inputThread = new Thread(InputHandlerThreadMethod);
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
        inputThread.Start();
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
                while (inputQueue.TryDequeue(out IConsoleInput? input))
                {
                    if (currentView is IKeyInputHandler keyHandler && keyHandler.AcceptsKeyInput() && input is KeyInput keyInput)
                    {
                        keyHandler.HandleKeyInput(keyInput);
                    }
                    else if (currentView is IMouseInputHandler mouseHandler && mouseHandler.AcceptsMouseInput() && input is MouseInput mouseInput)
                    {
                        mouseHandler.HandleMouseInput(mouseInput);
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
            else
            {
                Task.Delay(10).GetAwaiter().GetResult();
            }
        }

        Stop();
    }

    public void InputHandlerThreadMethod()
    {
        while (IsRunning)
        {
            IConsoleInput? input = handler.GetInput();
            if (input != null)
            {
                inputQueue.Enqueue(input);
            }
        }
    }

    public ConsoleView? GetCurrentView()
    {
        return viewStack.TryPeek(out ConsoleView? view) ? view : null;
    }

    private void RenderView(ConsoleView? view)
    {
        if (view != null)
        {
            previousGrid = grid?.MakeClone();
            grid = GetConsoleGrid(handler, grid);
            view.Render(grid);

            // Only print the grid if the current grid is different from the previously printed grid.
            if (previousGrid?.IsDifferentFrom(grid) != false)
            {
                RenderGrid(renderer, grid);
            }
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
            grid.Buffer.Span.Clear();
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
