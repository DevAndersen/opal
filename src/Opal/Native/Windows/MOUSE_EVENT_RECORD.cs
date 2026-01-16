namespace Opal.Native.Windows;

/// <summary>
/// Represents a mouse input event.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/mouse-event-record-str"/>
/// </remarks>
internal readonly record struct MOUSE_EVENT_RECORD
{
    public readonly COORD dwMousePosition;

    public readonly MouseButtonState dwButtonState;

    public readonly ControlKeyStates dwControlKeyState;

    public readonly MouseEventFlag dwEventFlags;
}
