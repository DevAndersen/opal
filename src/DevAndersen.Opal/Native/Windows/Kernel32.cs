using System.Runtime.InteropServices;

namespace DevAndersen.Opal.Native.Windows;

internal static partial class Kernel32
{
    #region Constants

    private const string kernel32 = "kernel32.dll";

    private const string getConsoleMode = "GetConsoleMode";
    private const string setConsoleMode = "SetConsoleMode";
    private const string writeConsole = "WriteConsoleW";

    #endregion

    #region Methods

    #region General

    [LibraryImport(kernel32, EntryPoint = "GetLastError")]
    public static partial uint GetLastError();

    [LibraryImport(kernel32, EntryPoint = "GetStdHandle", SetLastError = true)]
    public static partial nint GetStdHandle(StdHandle nStdHandle);

    #endregion

    #region GetConsoleMode

    [LibraryImport(kernel32, EntryPoint = getConsoleMode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetConsoleInputMode(nint hConsoleHandle, out ConsoleInputModes consoleInputModes);

    [LibraryImport(kernel32, EntryPoint = getConsoleMode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetConsoleOutputMode(nint hConsoleHandle, out ConsoleOutputModes consoleOutputModes);

    #endregion

    #region SetConsoleMode

    [LibraryImport(kernel32, EntryPoint = setConsoleMode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetConsoleInputMode(nint hConsoleHandle, ConsoleInputModes consoleInputModes);

    [LibraryImport(kernel32, EntryPoint = setConsoleMode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetConsoleOutputMode(nint hConsoleHandle, ConsoleOutputModes consoleOutputModes);

    #endregion

    #region WriteConsole

    [LibraryImport(kernel32, EntryPoint = writeConsole, StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool WriteConsole(nint hConsoleOutput, string lpBuffer, int nNumberOfCharsToWrite, out nint lpNumberOfCharsWritten);

    [LibraryImport(kernel32, EntryPoint = writeConsole, StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool WriteConsole(nint hConsoleOutput, nint lpBuffer, int nNumberOfCharsToWrite, out nint lpNumberOfCharsWritten);

    #endregion

    #endregion

    #region Structs

    #endregion

    #region Enums

    public enum StdHandle
    {
        STD_INPUT_HANDLE = -10,
        STD_OUTPUT_HANDLE = -11,
        STD_ERROR_HANDLE = -12
    }

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

    [Flags]
    public enum ConsoleOutputModes : uint
    {
        ENABLE_PROCESSED_OUTPUT = 0x0001,
        ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
        ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
        DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
        ENABLE_LVB_GRID_WORLDWIDE = 0x0010
    }

    #endregion
}
