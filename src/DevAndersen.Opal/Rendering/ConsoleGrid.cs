using DevAndersen.Opal.ConsoleHandlers;

namespace DevAndersen.Opal.Rendering;

/// <summary>
/// Represents a grid of <c><see cref="ConsoleChar"/></c>.
/// </summary>
public class ConsoleGrid
{
    private readonly object lockObject;

    public int Width { get; private set; }

    public int Height { get; private set; }

    internal Memory<ConsoleChar> Grid { get; private set; }

    public ConsoleChar this[int x, int y]
    {
        get
        {
            lock (lockObject)
            {
                return IsCoordinateWithinGrid(x, y)
                    ? Grid.Span[x + y * Width]
                    : default;
            }
        }
        set
        {
            lock (lockObject)
            {
                if (IsCoordinateWithinGrid(x, y))
                {
                    Grid.Span[x + y * Width] = value;
                }
            }
        }
    }

    public ConsoleChar this[float x, float y]
    {
        get => this[(int)Math.Round(x, 0, MidpointRounding.AwayFromZero), (int)Math.Round(y, 0, MidpointRounding.AwayFromZero)];
        set => this[(int)Math.Round(x, 0, MidpointRounding.AwayFromZero), (int)Math.Round(y, 0, MidpointRounding.AwayFromZero)] = value;
    }

    public ConsoleGrid(int width, int height)
    {
        lockObject = new object();
        SetSize(width, height);
    }

    /// <summary>
    /// Update the size of the console grid, if <see cref="IConsoleHandler.Width"/> or <see cref="IConsoleHandler.Height"/> of <paramref name="handler"/> are different from the current dimensions of the console grid.
    /// </summary>
    /// <param name="handler"></param>
    public void SetSize(IConsoleHandler handler)
    {
        SetSize(handler.Width, handler.Height);
    }

    /// <summary>
    /// Updates the size of the console grid, if <paramref name="width"/> or <paramref name="height"/> are different from the current dimensions of the console grid, or if <paramref name="forceClear"/> is set to <c>true</c>.
    /// </summary>
    /// <param name="width">The new width of the console grid.</param>
    /// <param name="height">The new height of the console grid.</param>
    /// <param name="forceClear">If true, the console grid will be reset even if the dimensions of the console grid won't change. This means the method will always return <c>true</c>.</param>
    /// <returns>Returns <c>true</c> if the console grid was cleared, otherwise returns <c>false</c>.</returns>
    public bool SetSize(int width, int height, bool forceClear = false)
    {
        lock (lockObject)
        {
            if (forceClear || Width != width || Height != height)
            {
                Width = width;
                Height = height;
                Grid = new ConsoleChar[Width * Height];
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Returns <c>true</c> if the coordinate (<paramref name="x"/>, <paramref name="y"/>) is a valid location within the grid. Otherwise, returns <c>false</c>.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool IsCoordinateWithinGrid(int x, int y)
        => x >= 0
        && x < Width
        && y >= 0
        && y < Height;
}
