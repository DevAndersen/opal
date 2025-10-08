namespace Opal.Native.Unix;

[Flags]
internal enum XTermMouseAction
{
    LeftButton = 0,
    MiddleButton = 1,
    RightButton = 2,
    CursorMove = 35,
    ScrollUp = 64,
    ScrollDown = 65
}
