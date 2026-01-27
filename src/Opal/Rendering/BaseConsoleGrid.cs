using Opal.Forms.Controls;

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

    public IConsoleGrid CreateSubgrid(int x, int y, int width, int height)
    {
        return new ConsoleSubgrid(this, x, y, int.Min(width, Width - x), int.Min(height, Height - y));
    }

    public IConsoleGrid CreateSubgrid(Rect rect)
    {
        return CreateSubgrid(rect.PosX, rect.PosY, rect.Width, rect.Height);
    }

    public abstract (int PosX, int PosY) GetAbsolutePosition();

    public bool IsCoordinateWithinGrid(int x, int y)
        => x >= 0
        && x < Width
        && y >= 0
        && y < Height;
}
