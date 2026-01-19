using Opal.ConsoleHandlers.InputHandlers;
using Opal.Events;

namespace Opal.ConsoleHandlers;

/// <summary>
/// A implementation of the platform independent aspects of <see cref="IConsoleHandler"/>.
/// </summary>
public abstract class CommonConsoleHandler<TInputHandler> : IConsoleHandler
    where TInputHandler : IInputHandler
{
    private int _widthOffset;
    private int _heightOffset;

    protected const int consoleSizeThreadTimeout = 50;

    protected Thread ConsoleSizeThread { get; init; }

    public required TInputHandler InputHandler { get; init; }

    public OpalSettings? Settings { get; protected set; }

    public event ConsoleSizeChangedEventHandler? OnConsoleSizeChanged;

    public bool Running { get; protected set; }

    public int Width { get; protected set; }

    public int Height { get; protected set; }

    public int BufferWidthOffset => _widthOffset + Settings?.WidthOffset ?? 0;

    public int BufferHeightOffset => _heightOffset + Settings?.HeightOffset ?? 0;

    protected CommonConsoleHandler()
    {
        ConsoleSizeThread = new Thread(ConsoleSizeThreadMethod);
    }

    public virtual void Start(OpalSettings settings)
    {
        if (!settings.UseAlternateBuffer)
        {
            _widthOffset = Console.CursorLeft;
            _heightOffset = Console.CursorTop;
        }
    }

    public abstract void Stop();

    public abstract void Print(string str);

    public abstract void Print(StringBuilder stringBuilder);

    public virtual IConsoleInput? GetInput() => InputHandler.GetInput();

    protected virtual void ConsoleSizeThreadMethod()
    {
        while (Running)
        {
            Thread.Sleep(consoleSizeThreadTimeout);

            (int newWidth, int newHeight) = GetClampedConsoleSize(Settings);

            if (Width != newWidth || Height != newHeight)
            {
                Width = newWidth;
                Height = newHeight;
                OnConsoleSizeChanged?.Invoke(this, new ConsoleSizeChangedEventArgs(Width, Height));
            }
        }
    }

    /// <summary>
    /// Returns the width and height of the console, clamped according to the settings.
    /// </summary>
    /// <returns></returns>
    protected static (int width, int height) GetClampedConsoleSize(OpalSettings? Settings)
    {
        int width = int.Clamp(Console.WindowWidth, Settings?.MinWidth ?? 1, Settings?.MaxWidth ?? int.MaxValue);
        int height = int.Clamp(Console.WindowHeight, Settings?.MinHeight ?? 1, Settings?.MaxHeight ?? int.MaxValue);
        return (width, height);
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }
}
