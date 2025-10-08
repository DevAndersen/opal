using Opal.ConsoleHandlers.InputHandlers;
using Opal.Rendering;

namespace Opal.ConsoleHandlers;

/// <summary>
/// A general console handler for common use.
/// </summary>
public class CommonConsoleHandler : IConsoleHandler
{
    protected const int consoleSizeThreadTimeout = 50;
    protected readonly Thread consoleSizeThread;
    protected IInputHandler inputHandler;

    public OpalSettings? Settings { get; protected set; }

    public event ConsoleSizeChangedEventHandler? OnConsoleSizeChanged;

    public bool Running { get; protected set; }

    public int Width { get; protected set; }

    public int Height { get; protected set; }

    public int BufferWidthOffset => Settings?.WidthOffset ?? 0;

    public int BufferHeightOffset => Settings?.HeightOffset ?? 0;

    public CommonConsoleHandler()
    {
        consoleSizeThread = new Thread(ConsoleSizeThreadMethod);
        inputHandler = new UnixInputHandler(this);
    }

    public virtual void Start(OpalSettings settings)
    {
        if (Running)
        {
            throw new InvalidOperationException();
        }

        Running = true;
        Settings = settings;

        if (!consoleSizeThread.IsAlive)
        {
            consoleSizeThread.Start();
        }

        (Width, Height) = GetClampedConsoleSize(settings);

        if (settings.UseAlternateBuffer)
        {
            Print(SequenceProvider.EnableAlternateBuffer());
        }

        Console.CursorVisible = false;
        inputHandler.StartInputListening();
    }

    public virtual void Stop()
    {
        inputHandler.StopInputListening();
        Console.CursorVisible = true;

        if (Settings?.UseAlternateBuffer == true)
        {
            Print(SequenceProvider.DisableAlternateBuffer());
        }
        else
        {
            Print("\n");
        }

        Print(SequenceProvider.Reset());

        Running = false;
    }

    public virtual void Print(string str)
    {
        Console.Write(str);
    }

    public virtual void Print(StringBuilder stringBuilder)
    {
        Console.Out.Write(stringBuilder);
    }

    public virtual IConsoleInput? GetInput() => inputHandler.GetInput();

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
        int width = Math.Clamp(Console.WindowWidth, Settings?.MinWidth ?? 1, Settings?.MaxWidth ?? int.MaxValue);
        int height = Math.Clamp(Console.WindowHeight, Settings?.MinHeight ?? 1, Settings?.MaxHeight ?? int.MaxValue);
        return (width, height);
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }
}
