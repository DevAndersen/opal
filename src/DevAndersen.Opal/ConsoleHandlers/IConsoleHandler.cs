using System.Text;

namespace DevAndersen.Opal.ConsoleHandlers;

public interface IConsoleHandler : IDisposable
{
    public event ConsoleSizeChangedEventHandler OnConsoleSizeChanged;

    /// <summary>
    /// Is the console handler currently running?
    /// </summary>
    public bool Running { get; }

    /// <summary>
    /// The visible width of the console.
    /// </summary>
    /// <returns></returns>
    public int Width { get; }

    /// <summary>
    /// The visible height of the console.
    /// </summary>
    /// <returns></returns>
    public int Height { get; }

    /// <summary>
    /// Apply changes necessary for Opal to run.
    /// </summary>
    public void Start();

    /// <summary>
    /// Undo the changes done by <see cref="Start"/>, restoring the console to its initial state.
    /// </summary>
    public void Stop();

    /// <summary>
    /// Print <c><paramref name="str"/></c> to the console.
    /// </summary>
    /// <param name="str"></param>
    public void Print(string str);

    /// <summary>
    /// Print <c><paramref name="stringBuilder"/></c> to the console.
    /// </summary>
    /// <param name="stringBuilder"></param>
    public void Print(StringBuilder stringBuilder);

    /// <summary>
    /// Returns a new instance of <c><see cref="IConsoleHandler"/></c> using <c><see cref="CreateDefaultHandlerForCurrentPlatform"/></c>, after executing its <c><see cref="Start()"/></c> method.
    /// Intended for use in <c>using</c> blocks.
    /// </summary>
    /// <returns>A new instance of <c><see cref="IConsoleHandler"/></c> for the current platform, where its <see cref="Start"/> method has been executed.</returns>
    public static IConsoleHandler StartNew()
    {
        IConsoleHandler handler = CreateDefaultHandlerForCurrentPlatform();
        handler.Start();
        return handler;
    }

    /// <summary>
    /// Returns a new instance of <c><typeparamref name="TConsoleHandler"/></c>, after executing its <c><see cref="Start()"/></c> method.
    /// Intended for use in <c>using</c> blocks.
    /// </summary>
    /// <typeparam name="TConsoleHandler"></typeparam>
    /// <returns>A new instance of <c><see cref="IConsoleHandler"/></c>, where its <see cref="Start"/> method has been executed.</returns>
    public static IConsoleHandler StartNew<TConsoleHandler>() where TConsoleHandler : IConsoleHandler, new()
    {
        IConsoleHandler handler = new TConsoleHandler();
        handler.Start();
        return handler;
    }

    /// <summary>
    /// Returns a new instance of the default <c><see cref="IConsoleHandler"/></c> for the current platform.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if no default <c><see cref="IConsoleHandler"/></c> has been specified for the current platform.</exception>
    public static IConsoleHandler CreateDefaultHandlerForCurrentPlatform()
    {
        return Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => new WindowsConsoleHandler(),
            _ => throw new PlatformNotSupportedException()
        };
    }
}
