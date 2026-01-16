namespace Opal.Native.Windows;

/// <summary>
/// Represents a keyboard input event.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/key-event-record-str"/>
/// </remarks>
internal readonly record struct KEY_EVENT_RECORD
{
    /// <summary>
    /// The boolean state of the key.
    /// <c>1</c> = key down, <c>0</c> = key up.
    /// </summary>
    /// <remarks>
    /// The native type is a 4-byte <c>BOOL</c>, but because .NET <see cref="bool"/> is a single byte and not blittable, <see cref="int"/> is used instead.
    /// </remarks>
    public readonly int bKeyDown;

    public readonly ushort wRepeatCount;

    public readonly ushort wVirtualKeyCode;

    public readonly ushort wVirtualScanCode;

    /// <summary>
    /// The UTF-16 character represented by the keypress.
    /// </summary>
    /// <remarks>
    /// The native type is a union of a 16-bit Unicode and 8-bit ASCII character, but since ASCII is not of interest, it has been simplified to its 16-bit value.
    /// This field is a <see cref="ushort"/> instead of <see cref="char"/>, because the latter is not blittable.
    /// </remarks>
    public readonly ushort UnicodeChar;

    public readonly ControlKeyStates dwControlKeyState;
}
