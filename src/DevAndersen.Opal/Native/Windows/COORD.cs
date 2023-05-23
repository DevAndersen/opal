namespace DevAndersen.Opal.Native.Windows;

/// <summary>
/// Represents a <see cref="short"/> two-dimensional coordinate.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/coord-str"/>
/// </remarks>
internal struct COORD
{
    public short X;
    public short Y;

    public COORD(short X, short Y)
    {
        this.X = X;
        this.Y = Y;
    }
};
