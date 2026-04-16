using Opal.Views;

namespace Opal.Forms.Controls;

/// <summary>
/// Stores synchronous and asynchronous event listeners for an event with no arguments.
/// </summary>
public struct ConsoleEventHandler
{
    /// <summary>
    /// The registered stateless synchronous event listeners.
    /// </summary>
    private Action? _statelessAction;

    /// <summary>
    /// The registered synchronous event listeners.
    /// </summary>
    private Action<IConsoleState>? _action;

    /// <summary>
    /// The registered stateless asynchronous event listeners.
    /// </summary>
    private Func<CancellationToken, Task>? _statelessFunc;

    /// <summary>
    /// The registered asynchronous event listeners.
    /// </summary>
    private Func<IConsoleState, CancellationToken, Task>? _func;

    public ConsoleEventHandler(Action action)
    {
        _statelessAction += action;
    }

    public ConsoleEventHandler(Action<IConsoleState> action)
    {
        _action += action;
    }

    public ConsoleEventHandler(Func<CancellationToken, Task> func)
    {
        _statelessFunc += func;
    }

    public ConsoleEventHandler(Func<IConsoleState, CancellationToken, Task> func)
    {
        _func += func;
    }

    /// <summary>
    /// Invoke the registered event listeners.
    /// </summary>
    /// <remarks>
    /// Listeners are invoked sequentially, with synchronous listeners being invoked before asynchronous listeners.
    /// </remarks>
    /// <param name="consoleState"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async readonly Task InvokeAsync(IConsoleState consoleState, CancellationToken cancellationToken)
    {
        _action?.Invoke(consoleState);
        _statelessAction?.Invoke();

        if (_func != null && !cancellationToken.IsCancellationRequested)
        {
            await _func.Invoke(consoleState, cancellationToken);
        }

        if (_statelessFunc != null && !cancellationToken.IsCancellationRequested)
        {
            await _statelessFunc.Invoke(cancellationToken);
        }
    }

    /// <summary>
    /// Register a synchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ConsoleEventHandler operator +(ConsoleEventHandler handler, Action action)
    {
        handler._statelessAction += action;
        return handler;
    }

    /// <summary>
    /// Register a synchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ConsoleEventHandler operator +(ConsoleEventHandler handler, Action<IConsoleState> action)
    {
        handler._action += action;
        return handler;
    }

    /// <summary>
    /// Register an asynchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static ConsoleEventHandler operator +(ConsoleEventHandler handler, Func<CancellationToken, Task> func)
    {
        handler._statelessFunc += func;
        return handler;
    }

    /// <summary>
    /// Register an asynchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static ConsoleEventHandler operator +(ConsoleEventHandler handler, Func<IConsoleState, CancellationToken, Task> func)
    {
        handler._func += func;
        return handler;
    }

    /// <summary>
    /// Unregister a synchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ConsoleEventHandler operator -(ConsoleEventHandler handler, Action action)
    {
        handler._statelessAction -= action;
        return handler;
    }

    /// <summary>
    /// Unregister a synchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ConsoleEventHandler operator -(ConsoleEventHandler handler, Action<IConsoleState> action)
    {
        handler._action -= action;
        return handler;
    }

    /// <summary>
    /// Unregister an asynchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static ConsoleEventHandler operator -(ConsoleEventHandler handler, Func<CancellationToken, Task> func)
    {
        handler._statelessFunc -= func;
        return handler;
    }

    /// <summary>
    /// Unregister an asynchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static ConsoleEventHandler operator -(ConsoleEventHandler handler, Func<IConsoleState, CancellationToken, Task> func)
    {
        handler._func -= func;
        return handler;
    }
}

/// <summary>
/// Stores synchronous and asynchronous event listeners for an event that takes <typeparamref name="T"/> as its argument.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct ConsoleEventHandler<T>
{
    /// <summary>
    /// The registered stateless synchronous event listeners.
    /// </summary>
    private Action<T>? _statelessAction;

    /// <summary>
    /// The registered synchronous event listeners.
    /// </summary>
    private Action<T, IConsoleState>? _action;

    /// <summary>
    /// The registered stateless asynchronous event listeners.
    /// </summary>
    private Func<T, CancellationToken, Task>? _statelessFunc;

    /// <summary>
    /// The registered asynchronous event listeners.
    /// </summary>
    private Func<T, IConsoleState, CancellationToken, Task>? _func;

    public ConsoleEventHandler(Action<T> action)
    {
        _statelessAction += action;
    }

    public ConsoleEventHandler(Action<T, IConsoleState> action)
    {
        _action += action;
    }

    public ConsoleEventHandler(Func<T, CancellationToken, Task> func)
    {
        _statelessFunc += func;
    }

    public ConsoleEventHandler(Func<T, IConsoleState, CancellationToken, Task> func)
    {
        _func += func;
    }

    /// <summary>
    /// Invoke the registered event listeners.
    /// </summary>
    /// <remarks>
    /// Listeners are invoked sequentially, with synchronous listeners being invoked before asynchronous listeners.
    /// </remarks>
    /// <param name="argument"></param>
    /// <param name="consoleState"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async readonly Task InvokeAsync(T argument, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        _action?.Invoke(argument, consoleState);
        _statelessAction?.Invoke(argument);

        if (_func != null && !cancellationToken.IsCancellationRequested)
        {
            await _func.Invoke(argument, consoleState, cancellationToken);
        }

        if (_statelessFunc != null && !cancellationToken.IsCancellationRequested)
        {
            await _statelessFunc.Invoke(argument, cancellationToken);
        }
    }

    /// <summary>
    /// Register a synchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ConsoleEventHandler<T> operator +(ConsoleEventHandler<T> handler, Action<T> action)
    {
        handler._statelessAction += action;
        return handler;
    }

    /// <summary>
    /// Register a synchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ConsoleEventHandler<T> operator +(ConsoleEventHandler<T> handler, Action<T, IConsoleState> action)
    {
        handler._action += action;
        return handler;
    }

    /// <summary>
    /// Register an asynchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static ConsoleEventHandler<T> operator +(ConsoleEventHandler<T> handler, Func<T, CancellationToken, Task> func)
    {
        handler._statelessFunc += func;
        return handler;
    }

    /// <summary>
    /// Register an asynchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static ConsoleEventHandler<T> operator +(ConsoleEventHandler<T> handler, Func<T, IConsoleState, CancellationToken, Task> func)
    {
        handler._func += func;
        return handler;
    }

    /// <summary>
    /// Unregister a synchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ConsoleEventHandler<T> operator -(ConsoleEventHandler<T> handler, Action<T> action)
    {
        handler._statelessAction -= action;
        return handler;
    }

    /// <summary>
    /// Unregister a synchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ConsoleEventHandler<T> operator -(ConsoleEventHandler<T> handler, Action<T, IConsoleState> action)
    {
        handler._action -= action;
        return handler;
    }

    /// <summary>
    /// Unregister an asynchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static ConsoleEventHandler<T> operator -(ConsoleEventHandler<T> handler, Func<T, CancellationToken, Task> func)
    {
        handler._statelessFunc -= func;
        return handler;
    }

    /// <summary>
    /// Unregister an asynchronous event listener.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static ConsoleEventHandler<T> operator -(ConsoleEventHandler<T> handler, Func<T, IConsoleState, CancellationToken, Task> func)
    {
        handler._func -= func;
        return handler;
    }

    /// <summary>
    /// Returns a new <see cref="ConsoleEventHandler{T}"/> that invokes <paramref name="handler"/> when invoked.
    /// </summary>
    /// <remarks>
    /// When invoked, the <typeparamref name="T"/> argument will not be passed to <paramref name="handler"/>.
    /// </remarks>
    /// <param name="handler"></param>
    public static implicit operator ConsoleEventHandler<T>(ConsoleEventHandler handler)
    {
        return new ConsoleEventHandler<T>(async (_, consoleState, cancellationToken) =>
        {
            await handler.InvokeAsync(consoleState, cancellationToken);
        });
    }

    /// <summary>
    /// Returns a new <see cref="ConsoleEventHandler{T}"/> that invokes <paramref name="input.Handler"/> when invoked
    /// if <paramref name="input.Predicate"/> is <c>true</c>.
    /// </summary>
    /// <remarks>
    /// When invoked, the <typeparamref name="T"/> argument will not be passed to <paramref name="handler"/>.
    /// </remarks>
    /// <param name="input"></param>
    public static implicit operator ConsoleEventHandler<T>((ConsoleEventHandler<T> Handler, Predicate<T> Predicate) input)
    {
        return new ConsoleEventHandler<T>(async (value, consoleState, cancellationToken) =>
        {
            if (input.Predicate(value))
            {
                await input.Handler.InvokeAsync(value, consoleState, cancellationToken);
            }
        });
    }
}
