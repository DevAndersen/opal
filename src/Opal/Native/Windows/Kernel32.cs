using System.Runtime.InteropServices;

namespace Opal.Native.Windows;

/// <summary>
/// Wrapper for base Win32 APIs (kernel32.dll).
/// </summary>
internal static partial class Kernel32
{
    private const string _kernel32 = "kernel32.dll";

    private const string _getStdHandle = "GetStdHandle";
    private const string _getConsoleMode = "GetConsoleMode";
    private const string _setConsoleMode = "SetConsoleMode";
    private const string _writeConsole = "WriteConsoleW";
    private const string _readConsoleInput = "ReadConsoleInputW";
    private const string _peekConsoleInput = "PeekConsoleInputW";
    private const string _getNumberOfConsoleInputEvents = "GetNumberOfConsoleInputEvents";

    /// <summary>
    /// Gets a handler for the standard console device <c><paramref name="nStdHandle"/></c>.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/getstdhandle"/>
    /// </remarks>
    /// <param name="nStdHandle">A standard console device.</param>
    /// <returns>A handle for the specified standard console device.</returns>
    [LibraryImport(_kernel32, EntryPoint = _getStdHandle, SetLastError = false)]
    public static partial nint GetStdHandle(StandardDevice nStdHandle);

    /// <summary>
    /// Gets the console input mode.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/getconsolemode"/>
    /// </remarks>
    /// <param name="hConsoleHandle">The console input handle.</param>
    /// <param name="lpMode">The console input mode.</param>
    /// <returns><c>true</c> if successful, <c>false</c> if not successful.</returns>
    [LibraryImport(_kernel32, EntryPoint = _getConsoleMode, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetConsoleInputMode(nint hConsoleHandle, out ConsoleInputModes lpMode);

    /// <summary>
    /// Gets the console output mode.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/getconsolemode"/>
    /// </remarks>
    /// <param name="hConsoleHandle">The console output handle.</param>
    /// <param name="lpMode">The console output mode.</param>
    /// <returns>Returns <c>true</c> if successful, or <c>false</c> if not successful.</returns>
    [LibraryImport(_kernel32, EntryPoint = _getConsoleMode, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetConsoleOutputMode(nint hConsoleHandle, out ConsoleOutputModes lpMode);

    /// <summary>
    /// Sets the console input mode.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/setconsolemode"/>
    /// </remarks>
    /// <param name="hConsoleHandle">The console input handle.</param>
    /// <param name="dwMode">The console input mode.</param>
    /// <returns><c>true</c> if successful, <c>false</c> if not successful.</returns>
    [LibraryImport(_kernel32, EntryPoint = _setConsoleMode, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetConsoleInputMode(nint hConsoleHandle, ConsoleInputModes dwMode);

    /// <summary>
    /// Sets the console output mode.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/setconsolemode"/>
    /// </remarks>
    /// <param name="hConsoleHandle">The console output handle.</param>
    /// <param name="dwMode">The console output mode.</param>
    /// <returns><c>true</c> if successful, <c>false</c> if not successful.</returns>
    [LibraryImport(_kernel32, EntryPoint = _setConsoleMode, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetConsoleOutputMode(nint hConsoleHandle, ConsoleOutputModes dwMode);

    /// <summary>
    /// Prints <c><paramref name="lpBuffer"/></c> to the console.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/writeconsole"/>
    /// </remarks>
    /// <param name="hConsoleOutput">The console output handle.</param>
    /// <param name="lpBuffer">A pointer to the buffer containing data to be written to the console.</param>
    /// <param name="nNumberOfCharsToWrite">The number of characters to be printed. Should equal the length of <paramref name="lpBuffer"/>.</param>
    /// <param name="lpNumberOfCharsWritten">Reserved, not used.</param>
    /// <returns><c>true</c> if successful, <c>false</c> if not successful.</returns>
    [LibraryImport(_kernel32, EntryPoint = _writeConsole, StringMarshalling = StringMarshalling.Utf16, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool WriteConsole(nint hConsoleOutput, string lpBuffer, int nNumberOfCharsToWrite, out nint lpNumberOfCharsWritten);

    /// <summary>
    /// Prints <c><paramref name="lpBuffer"/></c> to the console.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/writeconsole"/>
    /// </remarks>
    /// <param name="hConsoleOutput">The console output handle.</param>
    /// <param name="lpBuffer">A pointer to the buffer containing data to be written to the console.</param>
    /// <param name="nNumberOfCharsToWrite">The number of characters to be printed. Should equal the length of <paramref name="lpBuffer"/>.</param>
    /// <param name="lpNumberOfCharsWritten">Reserved, not used.</param>
    /// <returns><c>true</c> if successful, <c>false</c> if not successful.</returns>
    [LibraryImport(_kernel32, EntryPoint = _writeConsole, StringMarshalling = StringMarshalling.Utf16, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe partial bool WriteConsole(nint hConsoleOutput, char* lpBuffer, int nNumberOfCharsToWrite, out nint lpNumberOfCharsWritten);

    /// <summary>
    /// Read an input from the console input stream.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/readconsoleinput"/>
    /// </remarks>
    /// <param name="hConsoleInput">The console input handle.</param>
    /// <param name="lpBuffer">The buffer containing the read data.</param>
    /// <param name="nLength">The number of inputs to read.</param>
    /// <param name="lpNumberOfEventsRead">The number of inputs read from the console input stream.</param>
    /// <returns></returns>
    [LibraryImport(_kernel32, EntryPoint = _readConsoleInput)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ReadConsoleInput(nint hConsoleInput, out INPUT_RECORD lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

    /// <summary>
    /// Peak an input from the console input stream, without removing the input from the stream.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/peekconsoleinput"/>
    /// </remarks>
    /// <param name="hConsoleInput">The console input handle.</param>
    /// <param name="lpBuffer">The buffer containing the read data.</param>
    /// <param name="nLength">The number of inputs to read.</param>
    /// <param name="lpNumberOfEventsRead">The number of inputs read from the console input stream.</param>
    /// <returns></returns>
    [LibraryImport(_kernel32, EntryPoint = _peekConsoleInput)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PeekConsoleInput(nint hConsoleInput, out INPUT_RECORD lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

    /// <summary>
    /// Gets the number of available input events.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/getnumberofconsoleinputevents"/>
    /// </remarks>
    /// <param name="hConsoleInput">The console input handle.</param>
    /// <param name="lpcNumberOfEvents">The number of available input events.</param>
    /// <returns></returns>
    [LibraryImport(_kernel32, EntryPoint = _getNumberOfConsoleInputEvents)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetNumberOfConsoleInputEvents(nint hConsoleInput, out uint lpcNumberOfEvents);
}
