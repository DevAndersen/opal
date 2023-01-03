using DevAndersen.Opal.Rendering;
using System.Runtime.InteropServices;
using static DevAndersen.Opal.Native.Windows.Kernel32;

namespace DevAndersen.Opal.ConsoleHandlers;

public class WindowsConsoleHandler : IConsoleHandler
{
    private nint inputHandle;
    private nint outputHandle;

    private ConsoleInputModes originalConsoleInputModes;
    private ConsoleOutputModes originalConsoleOutputModes;

    private readonly Thread consoleSizeThread;
    private const int consoleSizeThreadTimeout = 50;

    private OpalSettings? settings;

    public event ConsoleSizeChangedEventHandler? OnConsoleSizeChanged;

    public bool Running { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int BufferWidthOffset => settings?.WidthOffset ?? 0;

    public int BufferHeightOffset => settings?.HeightOffset ?? 0;

    public WindowsConsoleHandler()
    {
        consoleSizeThread = new Thread(ConsoleSizeThreadMethod);
    }

    public void Start(OpalSettings settings)
    {
        if (Running)
        {
            throw new InvalidOperationException();
        }

        Running = true;
        this.settings = settings;

        if (!consoleSizeThread.IsAlive)
        {
            consoleSizeThread.Start();
        }

        (Width, Height) = GetClampedConsoleSize();

        inputHandle = GetStdHandle(StdHandle.STD_INPUT_HANDLE);
        outputHandle = GetStdHandle(StdHandle.STD_OUTPUT_HANDLE);

        GetConsoleInputMode(inputHandle, out originalConsoleInputModes);
        GetConsoleOutputMode(outputHandle, out originalConsoleOutputModes);

        ConsoleOutputModes modifiedConsoleOutputModes = originalConsoleOutputModes | ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        SetConsoleOutputMode(outputHandle, modifiedConsoleOutputModes);

        if (settings.UseAlternateBuffer)
        {
            Print(SequenceProvider.EnableAlternateBuffer());
        }

        Console.CursorVisible = false;
    }

    public void Stop()
    {
        Console.CursorVisible = true;

        if (settings?.UseAlternateBuffer == true)
        {
            Print(SequenceProvider.DisableAlternateBuffer());
        }

        Print(SequenceProvider.Wrap(SequenceProvider.Reset()));
        SetConsoleInputMode(inputHandle, originalConsoleInputModes);
        SetConsoleOutputMode(inputHandle, originalConsoleOutputModes);

        Running = false;
    }

    public void Print(string str)
    {
        WriteConsole(outputHandle, str, str.Length, out _);
    }

    /// <summary>
    /// Print <c><paramref name="stringBuilder"/></c> to the console, without allocating a new <see cref="string"/> in order to avoid GC.
    /// </summary>
    /// <param name="stringBuilder"></param>
    public unsafe void Print(StringBuilder stringBuilder)
    {
        nint ptr = Marshal.AllocHGlobal(stringBuilder.Length * sizeof(char));
        Span<char> span = new Span<char>(ptr.ToPointer(), stringBuilder.Length);
        stringBuilder.CopyTo(0, span, stringBuilder.Length);
        WriteConsole(outputHandle, ptr, stringBuilder.Length, out _);
        Marshal.FreeHGlobal(ptr);
    }

    private void ConsoleSizeThreadMethod()
    {
        while (Running)
        {
            Thread.Sleep(consoleSizeThreadTimeout);

            (int newWidth, int newHeight) = GetClampedConsoleSize();

            if (Width != newWidth || Height != newHeight)
            {
                Width = newWidth;
                Height = newHeight;
                OnConsoleSizeChanged?.Invoke(this, new ConsoleSizeChangedEventArgs(Width, Height));
            }
        }
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Returns the width and height of the console, clamped according to the settings.
    /// </summary>
    /// <returns></returns>
    private (int width, int height) GetClampedConsoleSize()
    {
        int width = Math.Clamp(Console.WindowWidth, settings?.MinWidth ?? 1, settings?.MaxWidth ?? int.MaxValue);
        int height = Math.Clamp(Console.WindowHeight, settings?.MinHeight ?? 1, settings?.MaxHeight ?? int.MaxValue);
        return (width, height);
    }
}
