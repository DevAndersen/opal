namespace Opal.Native.Unix;

[Flags]
internal enum XTermMouseInput : byte
{
    LeftButton = 0b_0000_0000,
    MiddleButton = 0b_0000_0001,
    RightButton = 0b_0000_0010,
    ScrollUp = 0b_0100_0000,
    ScrollDown = 0b_0100_0001,

    Move = 0b_0010_0000,
    Shift = 0b_0000_0100,
    Alt = 0b_0000_1000,
    Control = 0b_0001_0000,
}
