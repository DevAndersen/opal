namespace Opal.Native.Windows;

/// <summary>
/// Defines mouse control key states.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/mouse-event-record-str"/>
/// </remarks>
[Flags]
internal enum ControlKeyStates
{
    None = 0,
    CAPSLOCK_ON = 0x0080,
    ENHANCED_KEY = 0x0100,
    LEFT_ALT_PRESSED = 0x0002,
    LEFT_CTRL_PRESSED = 0x0008,
    NUMLOCK_ON = 0x0020,
    RIGHT_ALT_PRESSED = 0x0001,
    RIGHT_CTRL_PRESSED = 0x0004,
    SCROLLLOCK_ON = 0x0040,
    SHIFT_PRESSED = 0x0010
}
