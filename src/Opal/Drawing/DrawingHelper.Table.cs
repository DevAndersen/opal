using Opal.Rendering;

namespace Opal.Drawing;

public static partial class DrawingHelper
{
    public static (int Width, int Height) DrawTable(
        this IConsoleGrid grid,
        int posX,
        int posY,
        int columns,
        int rows,
        int cellWidth,
        int cellHeight,
        Action<IConsoleGrid, int, int>? cellFunc,
        ConsoleChar template = default)
    {
        return DrawTable(grid, posX, posY, columns, rows, cellWidth, cellHeight, cellFunc, DrawStyle.StandardDrawStyle, template);
    }

    public static (int Width, int Height) DrawTable(
        this IConsoleGrid grid,
        int posX,
        int posY,
        int columns,
        int rows,
        int cellWidth,
        int cellHeight,
        Action<IConsoleGrid, int, int>? cellFunc,
        DrawStyle style,
        ConsoleChar template = default)
    {
        int[] cellWidths = new int[columns];
        int[] cellHeights = new int[rows];

        Array.Fill(cellWidths, cellWidth);
        Array.Fill(cellHeights, cellHeight);

        return DrawTable(grid, posX, posY, cellWidths, cellHeights, cellFunc, style, template);
    }

    public static (int Width, int Height) DrawTable(
        this IConsoleGrid grid,
        int posX,
        int posY,
        int[] columnWidths,
        int[] rowHeights,
        Action<IConsoleGrid, int, int>? cellFunc,
        ConsoleChar template = default)
    {
        return DrawTable(grid, posX, posY, columnWidths, rowHeights, cellFunc, DrawStyle.StandardDrawStyle, template);
    }

    public static (int Width, int Height) DrawTable(
        this IConsoleGrid grid,
        int posX,
        int posY,
        int[] columnWidths,
        int[] rowHeights,
        Action<IConsoleGrid, int, int>? cellFunc,
        DrawStyle style,
        ConsoleChar template = default)
    {
        int totalWidth = columnWidths.Sum() + columnWidths.Length + 1;
        int totalHeight = rowHeights.Sum() + rowHeights.Length + 1;

        int cellY = 0;
        int relY = 0;

        // Draw table grid
        for (int y = 0; y < totalHeight; y++)
        {
            int cellX = 0;
            int relX = 0;

            // Increment cellY
            if (relY > rowHeights[cellY])
            {
                cellY++;
                relY = 0;
            }

            for (int x = 0; x < totalWidth; x++)
            {
                char c;

                // Increment cellX
                if (relX > columnWidths[cellX])
                {
                    cellX++;
                    relX = 0;
                }

                if (relX == 0 && relY == 0)
                {
                    bool isTop = y == 0;
                    bool isBottom = y == totalHeight - 1;
                    bool isLeft = x == 0;
                    bool isRight = x == totalWidth - 1;

                    c = (isTop, isBottom, isLeft, isRight) switch
                    {
                        // Sides
                        (true, false, false, false) => style.TUp,
                        (false, true, false, false) => style.TDown,
                        (false, false, true, false) => style.TLeft,
                        (false, false, false, true) => style.TRight,

                        // Corners
                        (true, false, true, false) => style.TopLeftCorner,
                        (true, false, false, true) => style.TopRightCorner,
                        (false, true, true, false) => style.BottomLeftCorner,
                        (false, true, false, true) => style.BottomRightCorner,

                        // Inner
                        _ => style.Plus
                    };
                }
                else if (relX == 0)
                {
                    c = style.Vertical;
                }
                else if (relY == 0)
                {
                    c = style.Horizontal;
                }
                else
                {
                    c = ' ';
                }

                grid[posX + x, posY + y] = template with { Character = c };

                relX++;
            }
            relY++;
        }

        if (cellFunc != null)
        {
            // Fill cells
            int offsetY = 0;
            for (int y = 0; y < rowHeights.Length; y++)
            {
                int offsetX = 0;
                for (int x = 0; x < columnWidths.Length; x++)
                {
                    if (columnWidths[x] != 0 && rowHeights[y] != 0)
                    {
                        IConsoleGrid cellGrid = grid.CreateSubgrid(posX + offsetX + 1, posY + offsetY + 1, columnWidths[x], rowHeights[y]);
                        cellFunc(cellGrid, x, y);
                    }

                    offsetX += columnWidths[x] + 1;
                }
                offsetY += rowHeights[y] + 1;
            }
        }

        return (totalWidth, totalHeight);
    }
}
