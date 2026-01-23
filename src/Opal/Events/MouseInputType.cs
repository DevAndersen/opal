namespace Opal.Events;

[Flags]
public enum MouseInputType : byte // Todo: Rename to "MouseInputButtons" or something similar.
{
    None = 0,
    LeftButton = 0b_00001,
    MiddleButton = 0b_00010,
    RightButton = 0b_00100,
    ScrollUp = 0b_01000,
    ScrollDown = 0b_10000
}
