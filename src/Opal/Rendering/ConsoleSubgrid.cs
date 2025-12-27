namespace Opal.Rendering;

/// <summary>
/// Represents a region of an <c><see cref="IConsoleGrid"/></c>.
/// </summary>
public class ConsoleSubgrid : BaseConsoleGrid
{
    private readonly IConsoleGrid _parentGrid;
    private readonly int _offsetX;
    private readonly int _offsetY;

    public override ConsoleChar this[int x, int y]
    {
        get => IsCoordinateWithinGrid(x, y) ? _parentGrid[_offsetX + x, _offsetY + y] : default;
        set
        {
            if (IsCoordinateWithinGrid(x, y))
            {
                _parentGrid[_offsetX + x, _offsetY + y] = value;
            }
        }
    }

    public ConsoleSubgrid(IConsoleGrid parentGrid, int offsetX, int offsetY, int width, int height) : base(width, height)
    {
        _parentGrid = parentGrid;
        _offsetX = offsetX;
        _offsetY = offsetY;
    }
}
