namespace Opal.Drawing;

public record DrawStyle
{
    public char Horizontal { get; init; }

    public char Vertical { get; init; }

    public char TopLeftCorner { get; init; }

    public char TopRightCorner { get; init; }

    public char BottomLeftCorner { get; init; }

    public char BottomRightCorner { get; init; }

    public char TUp { get; init; }

    public char TDown { get; init; }

    public char TLeft { get; init; }

    public char TRight { get; init; }

    public char Plus { get; init; }

    public static DrawStyle StandardDrawStyle { get; } = new DrawStyle
    {
        Horizontal = CharLib.Box.LightHorizontal,
        Vertical = CharLib.Box.LightVertical,
        TopLeftCorner = CharLib.Box.LightDownAndRight,
        TopRightCorner = CharLib.Box.LightDownAndLeft,
        BottomLeftCorner = CharLib.Box.LightUpAndRight,
        BottomRightCorner = CharLib.Box.LightUpAndLeft,
        TUp = CharLib.Box.LightDownAndHorizontal,
        TDown = CharLib.Box.LightUpAndHorizontal,
        TLeft = CharLib.Box.LightVerticalAndRight,
        TRight = CharLib.Box.LightVerticalAndLeft,
        Plus = CharLib.Box.LightVerticalAndHorizontal
    };

    public static DrawStyle HeavyDrawStyle { get; } = new DrawStyle
    {
        Horizontal = CharLib.Box.HeavyHorizontal,
        Vertical = CharLib.Box.HeavyVertical,
        TopLeftCorner = CharLib.Box.HeavyDownAndRight,
        TopRightCorner = CharLib.Box.HeavyDownAndLeft,
        BottomLeftCorner = CharLib.Box.HeavyUpAndRight,
        BottomRightCorner = CharLib.Box.HeavyUpAndLeft,
        TUp = CharLib.Box.HeavyDownAndHorizontal,
        TDown = CharLib.Box.HeavyUpAndHorizontal,
        TLeft = CharLib.Box.HeavyVerticalAndRight,
        TRight = CharLib.Box.HeavyVerticalAndLeft,
        Plus = CharLib.Box.HeavyVerticalAndHorizontal
    };

    public static DrawStyle DoubleDrawStyle { get; } = new DrawStyle
    {
        Horizontal = CharLib.Box.DoubleHorizontal,
        Vertical = CharLib.Box.DoubleVertical,
        TopLeftCorner = CharLib.Box.DoubleDownAndRight,
        TopRightCorner = CharLib.Box.DoubleDownAndLeft,
        BottomLeftCorner = CharLib.Box.DoubleUpAndRight,
        BottomRightCorner = CharLib.Box.DoubleUpAndLeft,
        TUp = CharLib.Box.DoubleDownAndHorizontal,
        TDown = CharLib.Box.DoubleUpAndHorizontal,
        TLeft = CharLib.Box.DoubleVerticalAndRight,
        TRight = CharLib.Box.DoubleVerticalAndLeft,
        Plus = CharLib.Box.DoubleVerticalAndHorizontal
    };

    public static DrawStyle RoundedDrawStyle { get; } = new DrawStyle
    {
        Horizontal = CharLib.Box.LightHorizontal,
        Vertical = CharLib.Box.LightVertical,
        TopLeftCorner = CharLib.Box.LightArcDownAndRight,
        TopRightCorner = CharLib.Box.LightArcDownAndLeft,
        BottomLeftCorner = CharLib.Box.LightArcUpAndRight,
        BottomRightCorner = CharLib.Box.LightArcUpAndLeft,
        TUp = CharLib.Box.LightDownAndHorizontal,
        TDown = CharLib.Box.LightUpAndHorizontal,
        TLeft = CharLib.Box.LightVerticalAndRight,
        TRight = CharLib.Box.LightVerticalAndLeft,
        Plus = CharLib.Box.LightVerticalAndHorizontal
    };
}
