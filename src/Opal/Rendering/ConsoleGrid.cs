using Opal.ConsoleHandlers;

namespace Opal.Rendering;

/// <summary>
/// Represents a grid of <c><see cref="ConsoleChar"/></c> that can be printed to the console.
/// </summary>
public class ConsoleGrid : BaseConsoleGrid
{
    private readonly Lock _lockObject;

    internal Memory<ConsoleChar> Buffer { get; private set; }

    public override ConsoleChar this[int x, int y]
    {
        get
        {
            lock (_lockObject)
            {
                return IsCoordinateWithinGrid(x, y)
                    ? Buffer.Span[x + y * Width]
                    : default;
            }
        }
        set
        {
            lock (_lockObject)
            {
                if (IsCoordinateWithinGrid(x, y))
                {
                    Buffer.Span[x + y * Width] = value;
                }
            }
        }
    }

    public ConsoleGrid(int width, int height) : base(width, height)
    {
        _lockObject = new Lock();
        SetSize(width, height, true);
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
    /// Updates the size of the console grid, if <paramref name="width"/> or <paramref name="height"/> are different from the current dimensions of the console grid, or if <paramref name="force"/> is set to <c>true</c>.
    /// </summary>
    /// <param name="width">The new width of the console grid.</param>
    /// <param name="height">The new height of the console grid.</param>
    /// <param name="force">If true, the console grid will be set (and, in doing so, cleared), even if the dimensions of the console grid won't change. This means the method will always return <c>true</c>.</param>
    /// <returns>Returns <c>true</c> if the console grid was cleared, otherwise returns <c>false</c>.</returns>
    public bool SetSize(int width, int height, bool force = false)
    {
        lock (_lockObject)
        {
            if (force || Width != width || Height != height)
            {
                Width = width;
                Height = height;
                Buffer = new ConsoleChar[Width * Height];
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Creates a new <see cref="ConsoleGrid"/> with the same dimensions and buffer content as this grid.
    /// </summary>
    /// <returns></returns>
    public ConsoleGrid MakeClone()
    {
        lock (_lockObject)
        {
            ConsoleGrid clone = new ConsoleGrid(Width, Height);
            Buffer.CopyTo(clone.Buffer);
            return clone;
        }
    }

    /// <summary>
    /// Attempt to copy the buffer of this grid into <paramref name="grid"/>, if they have the same size.
    /// </summary>
    /// <returns></returns>
    public bool TryCopyTo(ConsoleGrid? grid)
    {
        if (grid == null)
        {
            return false;
        }

        lock (_lockObject)
        {
            if (Width != grid.Width || Height != grid.Height)
            {
                return false;
            }

            try
            {
                Buffer.CopyTo(grid.Buffer);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Determines if <paramref name="other"/> has a different size or content that this grid.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsDifferentFrom(ConsoleGrid other)
    {
        return Width != other.Width
            || Height != other.Height
            || !Buffer.Span.SequenceEqual(other.Buffer.Span);
    }
}
