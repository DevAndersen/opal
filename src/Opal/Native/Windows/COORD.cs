namespace Opal.Native.Windows;

/// <summary>
/// Represents a <see cref="short"/> two-dimensional coordinate.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/coord-str"/>
/// </remarks>
internal record struct COORD(short X, short Y);
