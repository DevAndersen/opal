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
    /// Creates a new <see cref="IConsoleGrid"/> which represents the requested region within the grid.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    IConsoleGrid CreateSubgrid(int x, int y, int width, int height);

    /// <summary>
    /// Creates a new <see cref="IConsoleGrid"/> which represents the requested region within the grid.
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    IConsoleGrid CreateSubgrid(Rect rect);

    /// <summary>
    /// Returns the absolute position of the grid.
    /// </summary>
    /// <returns></returns>
    (int PosX, int PosY) GetAbsolutePosition();

    /// <summary>
    /// Returns <c>true</c> if the coordinate (<paramref name="x"/>, <paramref name="y"/>) is a valid location within the grid. Otherwise, returns <c>false</c>.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool IsCoordinateWithinGrid(int x, int y);
}
