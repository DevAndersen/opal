using Opal.ConsoleHandlers;
using Opal.Events;
using Opal.Rendering;
using Opal.Views;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace Opal;

public class OpalManager : IDisposable
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
    /// Captures the exception which was set via <see cref="Stop(Exception)"/>, which will then be thrown once gracefully stopped.
    /// </summary>
    /// <remarks>
    /// This is primarily intended to handle exceptions throw from separate threads,
    /// as these could otherwise cause an ungraceful stop, leaving the console in a non-vanilla state.
    /// </remarks>
    private ExceptionDispatchInfo? _stopExceptionDispatchInfo;

    /// <summary>
    /// Will be cancelled when <see cref="Stop(Exception)"/> is invoked, requesting a graceful stop.
    /// </summary>
    private readonly CancellationTokenSource _stopExceptionCancellationTokenSource;

    /// <summary>
    /// Stores the previous key input.
    /// </summary>
    private KeyInput? _previousKeyInput;

    /// <summary>
    /// Returns <c>true</c> if an instance of <see cref="OpalManager"/> currently running.
    /// This property be checked before invoking <see cref="StartAsync"/>.
    /// </summary>
    public static bool IsRunning { get; private set; }

    public OpalManager(IConsoleHandler consoleHandler, OpalSettings settings)
    {
        _handler = consoleHandler;
        _renderer = new ConsoleRenderer(consoleHandler);
        _viewStack = new Stack<IBaseConsoleView>();
        _inputQueue = new ConcurrentQueue<IConsoleInput>();
        _inputThread = new Thread(InputHandlerThreadMethod);
        _settings = settings;
        _stopExceptionCancellationTokenSource = new CancellationTokenSource();
    }

    public OpalManager(OpalSettings settings) : this(IConsoleHandler.CreateDefaultHandlerForCurrentPlatform(), settings)
    {
    }

    public OpalManager(IConsoleHandler consoleHandler) : this(consoleHandler, OpalSettings.CreateFullscreen())
    {
    }

    public OpalManager() : this(IConsoleHandler.CreateDefaultHandlerForCurrentPlatform(), OpalSettings.CreateFullscreen())
    {
    }

    public async Task StartAsync(IBaseConsoleView view, CancellationToken cancellationToken = default)
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("Opal is already running.");
        }

        if (!_stopExceptionCancellationTokenSource.TryReset())
        {
            throw new InvalidOperationException($"Unable to reset stop exception cancellation token source, use a new instance of {nameof(OpalManager)}");
        }

        _handler.Start(_settings);
        _handler.OnConsoleSizeChanged += HandleConsoleSizeChanged;
        Console.CancelKeyPress += CancellationAction;

        IsRunning = true;
        _inputThread.Start();

        try
        {
            // Linked cancellation token source, allowing multiple sources to request a graceful stop.
            CancellationTokenSource runCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _stopExceptionCancellationTokenSource.Token);

            await RunAsync(view, runCancellationTokenSource.Token);
        }
        catch
        {
            throw;
        }
        finally
        {
            Stop();
        }

        // If a stop exception was captured, throw it.
        _stopExceptionDispatchInfo?.Throw();
    }

    public void Stop(Exception e)
    {
        _stopExceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
        _stopExceptionCancellationTokenSource.Cancel();
    }

    public void Stop()
    {
        while (_viewStack.TryPop(out IBaseConsoleView? view))
        {
            ExitView(view);
        }

        IsRunning = false;
        Console.CancelKeyPress -= CancellationAction;
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

            // Initialize the current view.
            await currentView.InitializeViewAsync();

            // Todo: Should the input queue be cleared when changing view?

            // Handle input
            while (_inputQueue.TryDequeue(out IConsoleInput? input) && !state.HaltViewExecution)
            {
                if (currentView is IKeyInputHandler keyHandler && keyHandler.AcceptsKeyInput() && input is KeyInput keyInput)
                {
                    keyHandler.HandleKeyInput(keyInput, state);
                    _previousKeyInput = keyInput;
                }
                else if (currentView is IMouseButtonInputHandler mouseButtonHandler && mouseButtonHandler.AcceptsMouseButtonInput() && input is MouseButtonInput mouseButtonInput)
                {
                    mouseButtonHandler.HandleMouseButtonInput(mouseButtonInput, state);
                }
                else if (currentView is IMouseMoveInputHandler mouseMoveHandler && mouseMoveHandler.AcceptsMouseMoveInput() && input is MouseMoveInput mouseMoveInput)
                {
                    mouseMoveHandler.HandleMouseMoveInput(mouseMoveInput, state);
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
                try
                {
                    // Todo: Add mechanism for views to specify a custom delay between renders.
                    await Task.Delay(10, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                }
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
            Thread.Sleep(1000 / 180); // Todo: Make the input delay adjustable (currently 180 updates/second).

            IBaseConsoleView? currentView = GetCurrentView();
            if (currentView is IKeyInputHandler or IMouseButtonInputHandler or IMouseMoveInputHandler)
            {
                IConsoleInput? input = _handler.GetInput();
                if (input != null)
                {
                    _inputQueue.Enqueue(input);
                }
            }
        }
    }

    private void CancellationAction(object? sender, ConsoleCancelEventArgs args)
    {
        args.Cancel = true;

        try
        {
            if ((GetCurrentView() as ICancellationRequestHandler)?.PreventCancellationRequest() == true)
            {
                KeyInput keyInput = _previousKeyInput ?? new KeyInput(default);

                _inputQueue.Enqueue(keyInput with
                {
                    Key = ConsoleKey.C,
                    KeyChar = default,
                    Modifiers = keyInput.Modifiers | ConsoleModifiers.Control
                });
            }
            else
            {
                Stop();
            }
        }
        catch (Exception e)
        {
            Stop(e);
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
            if (_grid?.TryCopyTo(_previousGrid) != true)
            {
                _previousGrid = _grid?.MakeClone();
            }

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
            else
            {
                grid.Buffer.Span.Clear();
            }
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
