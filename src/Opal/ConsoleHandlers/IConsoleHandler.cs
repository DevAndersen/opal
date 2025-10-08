namespace Opal.ConsoleHandlers;

/// <summary>
/// Defines a handler that supports console interactions.
/// </summary>
public interface IConsoleHandler : IDisposable
{
    public event ConsoleSizeChangedEventHandler? OnConsoleSizeChanged;

    public OpalSettings? Settings { get; }

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

    public int BufferWidthOffset { get; }

    public int BufferHeightOffset { get; }

    /// <summary>
    /// Apply changes necessary for Opal to run.
    /// </summary>
    /// <param name="settings"></param>
    public void Start(OpalSettings settings);

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
    /// Returns a user input. If the returned value is <c>null</c>, it represents a lack of new user input.
    /// </summary>
    /// <returns></returns>
    public IConsoleInput? GetInput();

    /// <summary>
    /// Returns a new instance of <c><see cref="IConsoleHandler"/></c> using <c><see cref="CreateDefaultHandlerForCurrentPlatform"/></c> and set to fullscreen mode, after executing its <c><see cref="Start()"/></c> method.
    /// Intended for use in <c>using</c> blocks.
    /// </summary>
    /// <param name="settings"></param>
    /// <returns>A new instance of <c><see cref="IConsoleHandler"/></c> for the current platform, where its <see cref="Start"/> method has been executed.</returns>
    public static IConsoleHandler StartNewFullscreen()
    {
        return StartNew(OpalSettings.CreateFullscreen());
    }

    /// <summary>
    /// Returns a new instance of <c><see cref="IConsoleHandler"/></c> using <c><see cref="CreateDefaultHandlerForCurrentPlatform"/></c>, after executing its <c><see cref="Start()"/></c> method.
    /// Intended for use in <c>using</c> blocks.
    /// </summary>
    /// <param name="settings"></param>
    /// <returns>A new instance of <c><see cref="IConsoleHandler"/></c> for the current platform, where its <see cref="Start"/> method has been executed.</returns>
    public static IConsoleHandler StartNew(OpalSettings settings)
    {
        IConsoleHandler handler = CreateDefaultHandlerForCurrentPlatform();
        handler.Start(settings);
        return handler;
    }

    /// <summary>
    /// Returns a new instance of <c><typeparamref name="TConsoleHandler"/></c>, after executing its <c><see cref="Start()"/></c> method.
    /// Intended for use in <c>using</c> blocks.
    /// </summary>
    /// <typeparam name="TConsoleHandler"></typeparam>
    /// <param name="settings"></param>
    /// <returns>A new instance of <c><see cref="IConsoleHandler"/></c>, where its <see cref="Start"/> method has been executed.</returns>
    public static IConsoleHandler StartNew<TConsoleHandler>(OpalSettings settings) where TConsoleHandler : IConsoleHandler, new()
    {
        IConsoleHandler handler = new TConsoleHandler();
        handler.Start(settings);
        return handler;
    }

    /// <summary>
    /// Returns a new instance of the default <c><see cref="IConsoleHandler"/></c> for the current platform.
    /// </summary>
    /// <returns></returns>
    public static IConsoleHandler CreateDefaultHandlerForCurrentPlatform()
    {
        return Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => new WindowsConsoleHandler(),
            _ => new CommonConsoleHandler()
        };
    }
}
