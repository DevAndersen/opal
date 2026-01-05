using Opal.Rendering;

namespace Opal.Drawing;

public static partial class ConsolePainter
{
    public static (int Width, int Height) DrawTable(IConsoleGrid grid, int columns, int rows, int cellWidth, int cellHeight, Action<ConsoleSubgrid, int, int>? cellFunc = null, ConsoleChar template = default)
    {
        int[] cellWidths = new int[columns];
        int[] cellHeights = new int[rows];

        Array.Fill(cellWidths, cellWidth);
        Array.Fill(cellHeights, cellHeight);

        return DrawTable(grid, cellWidths, cellHeights, cellFunc, template);
    }

    public static (int Width, int Height) DrawTable(IConsoleGrid grid, int[] columnWidths, int[] rowHeights, Action<ConsoleSubgrid, int, int>? cellFunc = null, ConsoleChar template = default)
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
                        (true, false, false, false) => DrawStyle.StandardDrawStyle.TUp,
                        (false, true, false, false) => DrawStyle.StandardDrawStyle.TDown,
                        (false, false, true, false) => DrawStyle.StandardDrawStyle.TLeft,
                        (false, false, false, true) => DrawStyle.StandardDrawStyle.TRight,

                        // Corners
                        (true, false, true, false) => DrawStyle.StandardDrawStyle.TopLeftCorner,
                        (true, false, false, true) => DrawStyle.StandardDrawStyle.TopRightCorner,
                        (false, true, true, false) => DrawStyle.StandardDrawStyle.BottomLeftCorner,
                        (false, true, false, true) => DrawStyle.StandardDrawStyle.BottomRightCorner,

                        // Inner
                        _ => DrawStyle.StandardDrawStyle.Plus
                    };
                }
                else if (relX == 0)
                {
                    c = DrawStyle.StandardDrawStyle.Vertical;
                }
                else if (relY == 0)
                {
                    c = DrawStyle.StandardDrawStyle.Horizontal;
                }
                else
                {
                    c = ' ';
                }

                grid[x, y] = template with { Character = c };

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
                        ConsoleSubgrid cellGrid = grid.CreateSubgrid(offsetX + 1, offsetY + 1, columnWidths[x], rowHeights[y]);
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
