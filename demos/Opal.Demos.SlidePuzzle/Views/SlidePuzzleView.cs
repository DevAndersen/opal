using Opal.Drawing;
using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.SlidePuzzle.Views;

internal class SlidePuzzleView : ConsoleView, IKeyInputHandler
{
    private readonly PuzzleGrid _puzzle;

    public SlidePuzzleView(PuzzleGrid puzzle)
    {
        _puzzle = puzzle;
    }

    public override void Render(IConsoleGrid grid)
    {
        RenderPuzzle(grid);
    }

    private void RenderPuzzle(IConsoleGrid grid)
    {
        ConsolePainter.DrawString(grid, 3, 1, $"Moves: {_puzzle.Moves}");
        ConsolePainter.DrawString(grid, 20, 1, $"Time: {(int)_puzzle.Timer.Elapsed.TotalSeconds}");

        (_, int tableHeight) = ConsolePainter.DrawTable(grid, 2, 2, _puzzle.Width, _puzzle.Height, 7, 3, (grid, x, y) =>
        {
            int value = _puzzle[x, y];
            if (value != _puzzle.ValueToHide)
            {
                ConsoleChar template = new ConsoleChar
                {
                    ForegroundSimple = _puzzle.CheckIsCorrect(x, y) ? ConsoleColor.Green : ConsoleColor.Red
                };

                ConsolePainter.DrawString(grid, 3, 1, value.ToString().PadRight(2), template);
            }
        }, new ConsoleChar { ForegroundSimple = ConsoleColor.DarkGray });

        if (_puzzle.IsSolved)
        {
            ConsolePainter.DrawString(grid, 3, 3 + tableHeight, "Puzzle completed!", new ConsoleChar { ForegroundSimple = ConsoleColor.Green });
        }
    }

    public void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
    {
        switch (keyEvent.Key)
        {
            case ConsoleKey.W or ConsoleKey.UpArrow when !_puzzle.IsSolved:
                _puzzle.MoveUp(true);
                break;

            case ConsoleKey.S or ConsoleKey.DownArrow when !_puzzle.IsSolved:
                _puzzle.MoveDown(true);
                break;

            case ConsoleKey.A or ConsoleKey.LeftArrow when !_puzzle.IsSolved:
                _puzzle.MoveLeft(true);
                break;

            case ConsoleKey.D or ConsoleKey.RightArrow when !_puzzle.IsSolved:
                _puzzle.MoveRight(true);
                break;

            case ConsoleKey.Enter when _puzzle.IsSolved:
            case ConsoleKey.Escape:
                consoleState.ExitView();
                break;
        }
    }
}
