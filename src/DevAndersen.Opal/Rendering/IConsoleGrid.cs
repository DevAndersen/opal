﻿namespace DevAndersen.Opal.Rendering;

/// <summary>
/// Represents a grid of <c><see cref="ConsoleChar"/></c>.
/// </summary>
public interface IConsoleGrid
{
    /// <summary>
    /// The width of the grid.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The height of the grid.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Access the <c><see cref="ConsoleChar"/></c> at coordinate <c>[<paramref name="x"/>, <paramref name="y"/>]</c>.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public ConsoleChar this[int x, int y] { get; set; }

    /// <summary>
    /// Creates a new <see cref="ConsoleSubgrid"/> which represents the region of this grid bound by the other parameters.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public ConsoleSubgrid CreateSubgrid(int x, int y, int width, int height);
}
