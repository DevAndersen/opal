namespace Opal.Rendering;

/// <summary>
/// Contains shared logic for console grids.
/// </summary>
public abstract class BaseConsoleGrid : IConsoleGrid
{
    public abstract ConsoleChar this[int x, int y] { get; set; }

    public int Width { get; protected set; }

    public int Height { get; protected set; }

    public BaseConsoleGrid(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public ConsoleSubgrid CreateSubgrid(int x, int y, int width, int height)
    {
        return new ConsoleSubgrid(this, x, y, width, height);
    }

    /// <summary>
    /// Returns <c>true</c> if the coordinate (<paramref name="x"/>, <paramref name="y"/>) is a valid location within the grid. Otherwise, returns <c>false</c>.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected bool IsCoordinateWithinGrid(int x, int y)
        => x >= 0
        && x < Width
        && y >= 0
        && y < Height;
}
