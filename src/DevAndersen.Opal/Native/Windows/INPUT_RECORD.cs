using System.Runtime.InteropServices;

namespace DevAndersen.Opal.Native.Windows;

/// <summary>
/// A union struct representing a console user input.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/input-record-str"/>
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
internal struct INPUT_RECORD
{
    /// <summary>
    /// The event type of the union.
    /// </summary>
    [FieldOffset(0)]
    public EventType EventType;

    /// <summary>
    /// The key event expression of the union. Should only be used if <c><see cref="EventType"/></c> is <c><see cref="EventType.KEY_EVENT"/></c>.
    /// </summary>
    [FieldOffset(4)]
    public KEY_EVENT_RECORD KeyEvent;

    /// The mouse event expression of the union. Should only be used if <c><see cref="EventType"/></c> is <c><see cref="EventType.MOUSE_EVENT"/></c>.
    [FieldOffset(4)]
    public MOUSE_EVENT_RECORD MouseEvent;
}
