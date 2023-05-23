namespace DevAndersen.Opal.Native.Windows;

/// <summary>
/// Defines mouse button states.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/mouse-event-record-str"/>
/// </remarks>
internal enum MouseButtonState
{
    None = 0,
    FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001,
    FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004,
    FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008,
    FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010,
    RIGHTMOST_BUTTON_PRESSED = 0x0002
}
