using System.Runtime.InteropServices;

namespace DevAndersen.Opal.Native.Windows;

/// <summary>
/// Represents a keyboard input event.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/key-event-record-str"/>
/// </remarks>
[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
internal struct KEY_EVENT_RECORD
{
    [FieldOffset(0)]
    public int bKeyDown;

    [FieldOffset(4)]
    public ushort wRepeatCount;

    [FieldOffset(6)]
    public ushort wVirtualKeyCode;

    [FieldOffset(8)]
    public ushort wVirtualScanCode;

    [FieldOffset(10)]
    public ushort UnicodeChar;

    [FieldOffset(12)]
    public ControlKeyStates dwControlKeyState;
}
