namespace DevAndersen.Opal.Native.Windows;

/// <summary>
/// Represents a mouse input event.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/mouse-event-record-str"/>
/// </remarks>
internal struct MOUSE_EVENT_RECORD
{
    public COORD dwMousePosition;
    public MouseButtonState dwButtonState;
    public ControlKeyStates dwControlKeyState;
    public MouseEventFlag dwEventFlags;
}
