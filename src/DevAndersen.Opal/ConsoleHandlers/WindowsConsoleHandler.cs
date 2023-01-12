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

    public OpalSettings? Settings { get; private set; }

    public event ConsoleSizeChangedEventHandler? OnConsoleSizeChanged;

    public bool Running { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int BufferWidthOffset => Settings?.WidthOffset ?? 0;

    public int BufferHeightOffset => Settings?.HeightOffset ?? 0;

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
        Settings = settings;

        if (!consoleSizeThread.IsAlive)
        {
            consoleSizeThread.Start();
        }

        (Width, Height) = IConsoleHandler.GetClampedConsoleSize(settings);

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

        if (Settings?.UseAlternateBuffer == true)
        {
            Print(SequenceProvider.DisableAlternateBuffer());
        }
        else
        {
            Print("\n");
        }

        Print(SequenceProvider.Reset());
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

            (int newWidth, int newHeight) = IConsoleHandler.GetClampedConsoleSize(Settings);

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
}
