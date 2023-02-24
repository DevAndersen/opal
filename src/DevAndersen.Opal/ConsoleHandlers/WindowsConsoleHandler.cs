﻿using DevAndersen.Opal.Rendering;
using System.Runtime.InteropServices;
using static DevAndersen.Opal.Native.Windows.Kernel32;

namespace DevAndersen.Opal.ConsoleHandlers;

/// <summary>
/// A console handler for Windows systems.
/// </summary>
public class WindowsConsoleHandler : CommonConsoleHandler, IDisposable
{
    /// <summary>
    /// The console input handle.
    /// </summary>
    private nint inputHandle;

    /// <summary>
    /// The console output handle.
    /// </summary>
    private nint outputHandle;

    /// <summary>
    /// The initial mode of the console input handle.
    /// </summary>
    private ConsoleInputModes originalConsoleInputModes;

    /// <summary>
    /// The initial mode of the console output handle.
    /// </summary>
    private ConsoleOutputModes originalConsoleOutputModes;

    public override void Start(OpalSettings settings)
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

    public override void Stop()
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

    public override void Print(string str)
    {
        WriteConsole(outputHandle, str, str.Length, out _);
    }

    /// <summary>
    /// Print <c><paramref name="stringBuilder"/></c> to the console, without allocating a new <see cref="string"/> in order to avoid GC.
    /// </summary>
    /// <param name="stringBuilder"></param>
    public unsafe override void Print(StringBuilder stringBuilder)
    {
        nint ptr = Marshal.AllocHGlobal(stringBuilder.Length * sizeof(char));
        Span<char> span = new Span<char>(ptr.ToPointer(), stringBuilder.Length);
        stringBuilder.CopyTo(0, span, stringBuilder.Length);
        WriteConsole(outputHandle, ptr, stringBuilder.Length, out _);
        Marshal.FreeHGlobal(ptr);
    }
}
