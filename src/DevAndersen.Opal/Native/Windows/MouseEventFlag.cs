namespace DevAndersen.Opal.Native.Windows;

/// <summary>
/// Defines mouse event flags.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/mouse-event-record-str"/>
/// </remarks>
internal enum MouseEventFlag
{
    None = 0,
    DOUBLE_CLICK = 0x0002,
    MOUSE_HWHEELED = 0x0008,
    MOUSE_MOVED = 0x0001,
    MOUSE_WHEELED = 0x0004
}
