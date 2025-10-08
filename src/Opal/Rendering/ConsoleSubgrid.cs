namespace Opal.Rendering;

/// <summary>
/// Represents a region of an <c><see cref="IConsoleGrid"/></c>.
/// </summary>
public class ConsoleSubgrid : BaseConsoleGrid
{
    private readonly IConsoleGrid parentGrid;
    private readonly int offsetX;
    private readonly int offsetY;

    public override ConsoleChar this[int x, int y]
    {
        get => IsCoordinateWithinGrid(x, y) ? parentGrid[offsetX + x, offsetY + y] : default;
        set
        {
            if (IsCoordinateWithinGrid(x, y))
            {
                parentGrid[offsetX + x, offsetY + y] = value;
            }
        }
    }

    public ConsoleSubgrid(IConsoleGrid parentGrid, int offsetX, int offsetY, int width, int height) : base(width, height)
    {
        this.parentGrid = parentGrid;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
    }
}
