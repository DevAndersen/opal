namespace DevAndersen.Opal.Native.Unix;

internal enum XTermInputEvent
{
    LeftButton = 0b_0000_0000,
    MiddleButton = 0b_0000_0001,
    RightButton = 0b_0000_0010,
    ScrollUp = 0b_0100_0000,
    ScrollDown = 0b_0100_0001,

    Move = 0b_0010_0000,
    Shift = 0X4,
    Meta = 0X8,
    Control = 0X16,
}
