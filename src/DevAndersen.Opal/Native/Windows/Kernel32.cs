using System.Runtime.InteropServices;

namespace DevAndersen.Opal.Native.Windows;

/// <summary>
/// Wrapper for base Win32 APIs (kernel32.dll).
/// </summary>
internal static partial class Kernel32
{
    private const string kernel32 = "kernel32.dll";

    private const string getStdHandle = "GetStdHandle";
    private const string getConsoleMode = "GetConsoleMode";
    private const string setConsoleMode = "SetConsoleMode";
    private const string writeConsole = "WriteConsoleW";

    /// <summary>
    /// Gets a handler for the standard console device <c><paramref name="nStdHandle"/></c>.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/getstdhandle"/>
    /// </remarks>
    /// <param name="nStdHandle">A standard console device.</param>
    /// <returns>A handle for the specified standard console device.</returns>
    [LibraryImport(kernel32, EntryPoint = getStdHandle, SetLastError = false)]
    public static partial nint GetStdHandle(StdHandle nStdHandle);

    /// <summary>
    /// Gets the console input mode.
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/getconsolemode"/>
    /// </remarks>
    /// <param name="hConsoleHandle">The console input handle.</param>
    /// <param name="lpMode">The console input mode.</param>
    /// <returns><c>true</c> if successful, <c>false</c> if not successful.</returns>
    [LibraryImport(kernel32, EntryPoint = getConsoleMode, SetLastError = false)]
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
    [LibraryImport(kernel32, EntryPoint = getConsoleMode, SetLastError = false)]
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
    [LibraryImport(kernel32, EntryPoint = setConsoleMode, SetLastError = false)]
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
    [LibraryImport(kernel32, EntryPoint = setConsoleMode, SetLastError = false)]
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
    [LibraryImport(kernel32, EntryPoint = writeConsole, StringMarshalling = StringMarshalling.Utf16, SetLastError = false)]
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
    [LibraryImport(kernel32, EntryPoint = writeConsole, StringMarshalling = StringMarshalling.Utf16, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe partial bool WriteConsole(nint hConsoleOutput, char* lpBuffer, int nNumberOfCharsToWrite, out nint lpNumberOfCharsWritten);

    /// <summary>
    /// Standard console devices.
    /// </summary>
    public enum StdHandle
    {
        /// <summary>
        /// The standard input device.
        /// </summary>
        STD_INPUT_HANDLE = -10,

        /// <summary>
        /// The standard output device.
        /// </summary>
        STD_OUTPUT_HANDLE = -11,

        /// <summary>
        /// The standard error device.
        /// </summary>
        STD_ERROR_HANDLE = -12
    }

    /// <summary>
    /// Modes for the console input buffer.
    /// </summary>
    [Flags]
    public enum ConsoleInputModes : uint
    {
        ENABLE_ECHO_INPUT = 0x0004,
        ENABLE_INSERT_MODE = 0x0020,
        ENABLE_LINE_INPUT = 0x0002,
        ENABLE_MOUSE_INPUT = 0x0010,
        ENABLE_PROCESSED_INPUT = 0x0001,
        ENABLE_QUICK_EDIT_MODE = 0x0040,
        ENABLE_WINDOW_INPUT = 0x0008,
        ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,
        ENABLE_EXTENDED_FLAGS = 0x0080
    }

    /// <summary>
    /// Modes for the console output buffer.
    /// </summary>
    [Flags]
    public enum ConsoleOutputModes : uint
    {
        ENABLE_PROCESSED_OUTPUT = 0x0001,
        ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
        ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
        DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
        ENABLE_LVB_GRID_WORLDWIDE = 0x0010
    }
}
