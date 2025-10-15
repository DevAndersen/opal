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
        Horizontal = (char)9472,
        Vertical = (char)9474,
        TopLeftCorner = (char)9484,
        TopRightCorner = (char)9488,
        BottomLeftCorner = (char)9492,
        BottomRightCorner = (char)9496,
        TUp = (char)9516,
        TDown = (char)9524,
        TLeft = (char)9500,
        TRight = (char)9508,
        Plus = (char)9532
    };

    public static DrawStyle HeavyDrawStyle { get; } = new DrawStyle
    {
        Horizontal = (char)9473,
        Vertical = (char)9475,
        TopLeftCorner = (char)9487,
        TopRightCorner = (char)9491,
        BottomLeftCorner = (char)9495,
        BottomRightCorner = (char)9499,
        TUp = (char)9523,
        TDown = (char)9531,
        TLeft = (char)9507,
        TRight = (char)9515,
        Plus = (char)9547
    };

    public static DrawStyle DoubleDrawStyle { get; } = new DrawStyle
    {
        Horizontal = (char)9552,
        Vertical = (char)9553,
        TopLeftCorner = (char)9556,
        TopRightCorner = (char)9559,
        BottomLeftCorner = (char)9562,
        BottomRightCorner = (char)9565,
        TUp = (char)9574,
        TDown = (char)9577,
        TLeft = (char)9568,
        TRight = (char)9571,
        Plus = (char)9580
    };

    public static DrawStyle RoundedDrawStyle { get; } = new DrawStyle
    {
        Horizontal = (char)9472,
        Vertical = (char)9474,
        TopLeftCorner = (char)9581,
        TopRightCorner = (char)9582,
        BottomLeftCorner = (char)9584,
        BottomRightCorner = (char)9583,
        TUp = (char)9516,
        TDown = (char)9524,
        TLeft = (char)9500,
        TRight = (char)9508,
        Plus = (char)9532
    };
}
