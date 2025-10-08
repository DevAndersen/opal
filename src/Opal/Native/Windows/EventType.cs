namespace Opal.Native.Windows;

/// <summary>
/// Defines the different types of console input events.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/input-record-str"/>
/// </remarks>
internal enum EventType : ushort
{
    KEY_EVENT = 0x0001,
    MOUSE_EVENT = 0x0002,
    WINDOW_BUFFER_SIZE_EVENT = 0x0004,
    MENU_EVENT = 0x0008,
    FOCUS_EVENT = 0x0010
}
