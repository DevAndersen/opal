using Opal.Events;

namespace Opal.ConsoleHandlers;

/// <summary>
/// An implementation of the platform independent aspects of <see cref="IConsoleHandler"/>.
/// </summary>
public abstract class CommonConsoleHandler : IConsoleHandler
{
    private int _widthOffset;
    private int _heightOffset;

    protected const int ConsoleSizeThreadTimeout = 50;

    protected Thread? ConsoleSizeThread { get; private set; }

    public OpalSettings? Settings { get; protected set; }

    public event ConsoleSizeChangedEventHandler? OnConsoleSizeChanged;

    public bool Running { get; protected set; }

    public int Width { get; protected set; }

    public int Height { get; protected set; }

    public int BufferWidthOffset => _widthOffset + Settings?.WidthOffset ?? 0;

    public int BufferHeightOffset => _heightOffset + Settings?.HeightOffset ?? 0;

    public virtual void Start(OpalSettings settings)
    {
        if (Running)
        {
            throw new InvalidOperationException();
        }

        ConsoleSizeThread = new Thread(ConsoleSizeThreadMethod);
        ConsoleSizeThread.Start();

        Running = true;
        Settings = settings;

        if (!settings.UseAlternateBuffer)
        {
            _widthOffset = Console.CursorLeft;
            _heightOffset = Console.CursorTop;
        }
    }

    public abstract void Stop();

    public abstract IEnumerable<IConsoleInput> GetInput();

    public abstract void Print(string str);

    public abstract void Print(StringBuilder stringBuilder);

    protected virtual void ConsoleSizeThreadMethod()
    {
        while (Running)
        {
            Thread.Sleep(ConsoleSizeThreadTimeout);

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
    protected static (int width, int height) GetClampedConsoleSize(OpalSettings? settings)
    {
        int width = int.Clamp(Console.WindowWidth, settings?.MinWidth ?? 1, settings?.MaxWidth ?? int.MaxValue);
        int height = int.Clamp(Console.WindowHeight, settings?.MinHeight ?? 1, settings?.MaxHeight ?? int.MaxValue);
        return (width, height);
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }
}
