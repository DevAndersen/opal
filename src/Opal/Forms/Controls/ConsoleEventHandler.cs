namespace Opal.Forms.Controls;

/// <summary>
/// Stores synchronous and asynchronous event listeners for an event with no arguments.
/// </summary>
public struct ConsoleEventHandler
{
    /// <summary>
    /// The registered synchronous event listeners.
    /// </summary>
    private Action? _action;

    /// <summary>
    /// The registered asynchronous event listeners.
    /// </summary>
    private Func<CancellationToken, Task>? _func;

    /// <summary>
    /// Invoke the registered event listeners.
    /// </summary>
    /// <remarks>
    /// Listeners are invoked sequentially, with synchronous listeners being invoked before asynchronous listeners.
    /// </remarks>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async readonly Task InvokeAsync(CancellationToken cancellationToken)
    {
        _action?.Invoke();

        if (_func != null && !cancellationToken.IsCancellationRequested)
        {
            await _func.Invoke(cancellationToken);
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
    /// The registered synchronous event listeners.
    /// </summary>
    private Action<T>? _action;

    /// <summary>
    /// The registered asynchronous event listeners.
    /// </summary>
    private Func<T, CancellationToken, Task>? _func;

    public ConsoleEventHandler(Action<T> action)
    {
        _action += action;
    }

    public ConsoleEventHandler(Func<T, CancellationToken, Task> func)
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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async readonly Task InvokeAsync(T argument, CancellationToken cancellationToken)
    {
        _action?.Invoke(argument);

        if (_func != null && !cancellationToken.IsCancellationRequested)
        {
            await _func.Invoke(argument, cancellationToken);
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
        handler._func -= func;
        return handler;
    }
}
