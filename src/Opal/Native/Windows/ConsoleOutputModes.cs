namespace Opal.Native.Windows;

/// <summary>
/// Modes for the console output buffer.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/setconsolemode"/>
/// </remarks>
[Flags]
internal enum ConsoleOutputModes : uint
{
    None = 0,
    ENABLE_PROCESSED_OUTPUT = 0x0001,
    ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
    ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
    DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
    ENABLE_LVB_GRID_WORLDWIDE = 0x0010
}
