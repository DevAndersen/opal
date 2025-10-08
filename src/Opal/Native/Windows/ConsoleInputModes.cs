namespace Opal.Native.Windows;

/// <summary>
/// Modes for the console input buffer.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/setconsolemode"/>
/// </remarks>
[Flags]
internal enum ConsoleInputModes : uint
{
    None = 0,
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
