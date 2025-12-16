using Opal.ConsoleHandlers;
using Opal.Rendering;
using Opal.Views;
using System.Collections.Concurrent;

namespace Opal;

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
    private readonly Stack<IBaseConsoleView> viewStack;

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
    /// This property be checked before invoking <see cref="StartAsync"/>.
    /// </summary>
    public static bool IsRunning { get; private set; }

    public OpalController(IConsoleHandler consoleHandler, OpalSettings settings)
    {
        handler = consoleHandler;
        renderer = new ConsoleRenderer(consoleHandler);
        viewStack = new Stack<IBaseConsoleView>();
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

    public async Task StartAsync(IBaseConsoleView view, CancellationToken cancellationToken = default)
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("Opal is already running.");
        }

        handler.Start(settings);
        handler.OnConsoleSizeChanged += HandleConsoleSizeChanged;

        inputThread.Start();

        IsRunning = true;

        await RunAsync(view, cancellationToken);
    }

    public void Stop()
    {
        // Todo: Dispose of all disposable views in view stack.

        IsRunning = false;
        handler.OnConsoleSizeChanged -= HandleConsoleSizeChanged;
        handler.Stop();
    }

    private async Task RunAsync(IBaseConsoleView initialView, CancellationToken cancellationToken)
    {
        viewStack.Push(initialView);

        ConsoleState state = new ConsoleState();

        while (IsRunning && !cancellationToken.IsCancellationRequested && viewStack.TryPeek(out IBaseConsoleView? currentView))
        {
            // Reset console state object.
            state.Reset(handler.Height, handler.Width);

            // Todo: Call init once per view.

            // Todo: Should the input queue be cleared when changing view?

            // Handle input
            while (inputQueue.TryDequeue(out IConsoleInput? input) && !state.HaltViewExecution)
            {
                if (currentView is IKeyInputHandler keyHandler && keyHandler.AcceptsKeyInput() && input is KeyInput keyInput)
                {
                    keyHandler.HandleKeyInput(keyInput, state);
                }
                else if (currentView is IMouseInputHandler mouseHandler && mouseHandler.AcceptsMouseInput() && input is MouseInput mouseInput)
                {
                    mouseHandler.HandleMouseInput(mouseInput, state);
                }
            }

            // Update view.
            if (!state.HasExitViewBeenRequested && !cancellationToken.IsCancellationRequested)
            {
                if (currentView is IConsoleView updateableView)
                {
                    updateableView.Update(state);
                }
                else if (currentView is IAsyncConsoleView asyncUpdateableView)
                {
                    await asyncUpdateableView.UpdateAsync(state, cancellationToken);
                }
            }

            // Render view.
            if (!state.HasExitViewBeenRequested && !cancellationToken.IsCancellationRequested)
            {
                RenderView(currentView);
            }

            // Delay.
            if (!state.HasExitViewBeenRequested && !cancellationToken.IsCancellationRequested)
            {
                // Todo: Add mechanism for views to specify a custom delay between renders.
                await Task.Delay(10, cancellationToken);
            }

            // Handle console state.
            if (!cancellationToken.IsCancellationRequested)
            {
                if (state.HasExitBeenRequested)
                {
                    IsRunning = false;
                }
                if (state.HasExitViewBeenRequested)
                {
                    viewStack.Pop();
                }
                else if (state.NextViewState is { } nextViewState)
                {
                    SetView(nextViewState.View, nextViewState.RemoveCurrentViewFromViewStack);
                }
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

    public IBaseConsoleView? GetCurrentView()
    {
        return viewStack.TryPeek(out IBaseConsoleView? view) ? view : null;
    }

    private void RenderView(IBaseConsoleView? view)
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

    internal void SetView(IBaseConsoleView view, bool removeCurrentViewFromViewStack)
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
        if (viewStack.TryPeek(out IBaseConsoleView? currentView))
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
