using Opal.ConsoleHandlers.InputHandlers;
using Opal.Native.Windows;
using Opal.Rendering;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static Opal.Native.Windows.Kernel32;

namespace Opal.ConsoleHandlers;

/// <summary>
/// A console handler for Windows systems.
/// </summary>
public class WindowsConsoleHandler : CommonConsoleHandler<WindowsInputHandler>, IDisposable
{
    /// <summary>
    /// The console input handle.
    /// </summary>
    private nint _inputHandle;

    /// <summary>
    /// The console output handle.
    /// </summary>
    private nint _outputHandle;

    /// <summary>
    /// The initial mode of the console input handle.
    /// </summary>
    private ConsoleInputModes _originalConsoleInputModes;

    /// <summary>
    /// The initial mode of the console output handle.
    /// </summary>
    private ConsoleOutputModes _originalConsoleOutputModes;

    [SetsRequiredMembers]
    public WindowsConsoleHandler()
    {
        InputHandler = new WindowsInputHandler();
    }

    public override void Start(OpalSettings settings)
    {
        if (Running)
        {
            throw new InvalidOperationException();
        }

        Running = true;
        Settings = settings;

        if (!ConsoleSizeThread.IsAlive)
        {
            ConsoleSizeThread.Start();
        }

        (Width, Height) = GetClampedConsoleSize(settings);

        _inputHandle = GetStdHandle(StandardDevice.STD_INPUT_HANDLE);
        _outputHandle = GetStdHandle(StandardDevice.STD_OUTPUT_HANDLE);

        GetConsoleInputMode(_inputHandle, out _originalConsoleInputModes);
        GetConsoleOutputMode(_outputHandle, out _originalConsoleOutputModes);

        ConsoleOutputModes modifiedConsoleOutputModes = _originalConsoleOutputModes | ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        SetConsoleOutputMode(_outputHandle, modifiedConsoleOutputModes);

        if (settings.UseAlternateBuffer)
        {
            Print(SequenceProvider.EnableAlternateBuffer());
        }

        Console.CursorVisible = false;
        InputHandler.Initialize(_inputHandle);
        InputHandler.StartInputListening();
    }

    public override void Stop()
    {
        InputHandler.StopInputListening();
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
        SetConsoleInputMode(_inputHandle, _originalConsoleInputModes);
        SetConsoleOutputMode(_inputHandle, _originalConsoleOutputModes);

        Running = false;
    }

    public override void Print(string str)
    {
        WriteConsole(_outputHandle, str, str.Length, out _);
    }

    /// <summary>
    /// Print <c><paramref name="stringBuilder"/></c> to the console, without allocating a new <see cref="string"/> in order to avoid GC.
    /// </summary>
    /// <param name="stringBuilder"></param>
    public unsafe override void Print(StringBuilder stringBuilder)
    {
        char* ptr = (char*)NativeMemory.Alloc((nuint)stringBuilder.Length, sizeof(char));
        try
        {
            Span<char> span = new Span<char>(ptr, stringBuilder.Length);
            stringBuilder.CopyTo(0, span, stringBuilder.Length);
            WriteConsole(_outputHandle, ptr, stringBuilder.Length, out _);
        }
        finally
        {
            NativeMemory.Free(ptr);
        }
    }
}
