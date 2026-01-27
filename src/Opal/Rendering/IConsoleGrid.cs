using Opal.Forms.Controls;

namespace Opal.Rendering;

/// <summary>
/// Represents a grid of <c><see cref="ConsoleChar"/></c>.
/// </summary>
public interface IConsoleGrid
{
    /// <summary>
    /// The width of the grid.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// The height of the grid.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Access the <c><see cref="ConsoleChar"/></c> at coordinate <c>[<paramref name="x"/>, <paramref name="y"/>]</c>.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    ConsoleChar this[int x, int y] { get; set; }

    /// <summary>
    /// Creates a new <see cref="IConsoleGrid"/> which represents the requested region within this grid.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    IConsoleGrid CreateSubgrid(int x, int y, int width, int height);

    /// <summary>
    /// Creates a new <see cref="IConsoleGrid"/> which represents the requested region within this grid.
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    IConsoleGrid CreateSubgrid(Rect rect);

    (int PosX, int PosY) GetAbsolutePosition();
}
