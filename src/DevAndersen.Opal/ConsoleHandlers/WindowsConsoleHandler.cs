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

    public event ConsoleSizeChangedEventHandler? OnConsoleSizeChanged;

    public bool Running { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public WindowsConsoleHandler()
    {
        consoleSizeThread = new Thread(ConsoleSizeThreadMethod);
    }

    public void Start()
    {
        if (Running)
        {
            throw new InvalidOperationException();
        }

        Running = true;

        if (!consoleSizeThread.IsAlive)
        {
            consoleSizeThread.Start();
        }

        Width = Console.WindowWidth;
        Height = Console.WindowHeight;

        inputHandle = GetStdHandle(StdHandle.STD_INPUT_HANDLE);
        outputHandle = GetStdHandle(StdHandle.STD_OUTPUT_HANDLE);

        GetConsoleInputMode(inputHandle, out originalConsoleInputModes);
        GetConsoleOutputMode(outputHandle, out originalConsoleOutputModes);

        ConsoleOutputModes modifiedConsoleOutputModes = originalConsoleOutputModes | ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        SetConsoleOutputMode(outputHandle, modifiedConsoleOutputModes);

        Print(SequenceProvider.EnableAlternateBuffer());

        Console.CursorVisible = false;
    }

    public void Stop()
    {
        Console.CursorVisible = true;

        Print(SequenceProvider.DisableAlternateBuffer());
        Print(SequenceProvider.Wrap(SequenceProvider.Reset()));
        SetConsoleInputMode(inputHandle, originalConsoleInputModes);
        SetConsoleOutputMode(inputHandle, originalConsoleOutputModes);

        Running = false;
    }

    public void Print(string str)
    {
        WriteConsole(outputHandle, str, str.Length, out _);
    }

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

            int newWidth = Console.WindowWidth;
            int newHeight = Console.WindowHeight;

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
