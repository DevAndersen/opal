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
    private readonly IConsoleHandler _handler;

    /// <summary>
    /// The console renderer.
    /// </summary>
    private readonly ConsoleRenderer _renderer;

    /// <summary>
    /// The stack of console views, allowing the exit of one view to return the user to the previous view.
    /// </summary>
    private readonly Stack<IBaseConsoleView> _viewStack;

    /// <summary>
    /// The input queue, containing user input to be processed with the next update of the view.
    /// </summary>
    private readonly ConcurrentQueue<IConsoleInput> _inputQueue;

    /// <summary>
    /// The thread responsible for listening to user input and enqueue these input to <see cref="_inputQueue"/>.
    /// </summary>
    private readonly Thread _inputThread;

    /// <summary>
    /// The base settings that for Opal.
    /// </summary>
    private readonly OpalSettings _settings;

    /// <summary>
    /// The current console grid, containing the data that will be printed to the console.
    /// </summary>
    private ConsoleGrid? _grid;

    /// <summary>
    /// The previous state of <see cref="_grid"/>.
    /// This is used to check if the console grid differs from the previous rendered grid, in order to avoid redundant prints.
    /// </summary>
    private ConsoleGrid? _previousGrid;

    /// <summary>
    /// Returns <c>true</c> if an instance of <see cref="OpalController"/> currently running.
    /// This property be checked before invoking <see cref="StartAsync"/>.
    /// </summary>
    public static bool IsRunning { get; private set; }

    public OpalController(IConsoleHandler consoleHandler, OpalSettings settings)
    {
        _handler = consoleHandler;
        _renderer = new ConsoleRenderer(consoleHandler);
        _viewStack = new Stack<IBaseConsoleView>();
        _inputQueue = new ConcurrentQueue<IConsoleInput>();
        _inputThread = new Thread(InputHandlerThreadMethod);
        _settings = settings;
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

        _handler.Start(_settings);
        _handler.OnConsoleSizeChanged += HandleConsoleSizeChanged;

        _inputThread.Start();

        IsRunning = true;

        await RunAsync(view, cancellationToken);
    }

    public void Stop()
    {
        while (_viewStack.TryPop(out IBaseConsoleView? view))
        {
            ExitView(view);
        }

        IsRunning = false;
        _handler.OnConsoleSizeChanged -= HandleConsoleSizeChanged;
        _handler.Stop();
    }

    private async Task RunAsync(IBaseConsoleView initialView, CancellationToken cancellationToken)
    {
        _viewStack.Push(initialView);

        ConsoleState state = new ConsoleState();

        while (IsRunning && !cancellationToken.IsCancellationRequested && _viewStack.TryPeek(out IBaseConsoleView? currentView))
        {
            // Reset console state object.
            state.Reset(_handler.Height, _handler.Width);

            // Todo: Call init once per view.

            // Todo: Should the input queue be cleared when changing view?

            // Handle input
            while (_inputQueue.TryDequeue(out IConsoleInput? input) && !state.HaltViewExecution)
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
                    ExitView(_viewStack.Pop());
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
            IConsoleInput? input = _handler.GetInput();
            if (input != null)
            {
                _inputQueue.Enqueue(input);
            }
        }
    }

    public IBaseConsoleView? GetCurrentView()
    {
        return _viewStack.TryPeek(out IBaseConsoleView? view) ? view : null;
    }

    private static void ExitView(IBaseConsoleView view)
    {
        if (view is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void RenderView(IBaseConsoleView? view)
    {
        if (view != null)
        {
            _previousGrid = _grid?.MakeClone();
            _grid = GetConsoleGrid(_handler, _grid);
            view.Render(_grid);

            // Only print the grid if the current grid is different from the previously printed grid.
            if (_previousGrid?.IsDifferentFrom(_grid) != false)
            {
                RenderGrid(_renderer, _grid);
            }
        }
    }

    internal void SetView(IBaseConsoleView view, bool removeCurrentViewFromViewStack)
    {
        if (removeCurrentViewFromViewStack)
        {
            _viewStack.Pop();
        }
        _viewStack.Push(view);
    }

    internal void ExitCurrentView()
    {
        _viewStack.Pop();
        RenderView(GetCurrentView());
    }

    private void HandleConsoleSizeChanged(object? sender, ConsoleSizeChangedEventArgs args)
    {
        _grid = GetConsoleGrid(_handler, _grid);
        _grid.SetSize(args.NewWidth, args.NewHeight);
        if (_viewStack.TryPeek(out IBaseConsoleView? currentView))
        {
            currentView.Render(_grid);
        }
        RenderGrid(_renderer, _grid);
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

        _handler.Dispose();
        GC.SuppressFinalize(this);
    }
}
