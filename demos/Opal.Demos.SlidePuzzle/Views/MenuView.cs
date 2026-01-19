using Opal.Drawing;
using Opal.Events;
using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.SlidePuzzle.Views;

internal class MenuView : ConsoleView, IKeyInputHandler
{
    private const int _minSize = 3;
    private const int _maxSize = 7;
    private const int _sizeIncrement = 1;

    private const int _minShuffleMoves = 5;
    private const int _maxShuffleMoves = 200;
    private const int _shuffleMoveIncrement = 5;

    private int _size = 5;
    private int _shuffleMoves = 50;

    private int _tabIndex = 0;

    public void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
    {
        if (keyEvent.Key is ConsoleKey.D or ConsoleKey.RightArrow)
        {
            if (_tabIndex == 0)
            {
                _size = int.Clamp(_size + _sizeIncrement, _minSize, _maxSize);
            }
            else if (_tabIndex == 1)
            {
                _shuffleMoves = int.Clamp(_shuffleMoves + _shuffleMoveIncrement, _minShuffleMoves, _maxShuffleMoves);
            }
        }
        else if (keyEvent.Key is ConsoleKey.A or ConsoleKey.LeftArrow)
        {
            if (_tabIndex == 0)
            {
                _size = int.Clamp(_size - _sizeIncrement, _minSize, _maxSize);
            }
            else if (_tabIndex == 1)
            {
                _shuffleMoves = int.Clamp(_shuffleMoves - _shuffleMoveIncrement, _minShuffleMoves, _maxShuffleMoves);
            }
        }
        else if (keyEvent.Key is ConsoleKey.S or ConsoleKey.DownArrow)
        {
            SelectAscending();
        }
        else if (keyEvent.Key is ConsoleKey.W or ConsoleKey.UpArrow)
        {
            SelectDescending();
        }
        else if (keyEvent.Key is ConsoleKey.Tab)
        {
            if (keyEvent.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                SelectDescending();
            }
            else
            {
                SelectAscending();
            }
        }
        else if (keyEvent.Key == ConsoleKey.Enter)
        {
            if (_tabIndex < 2)
            {
                _tabIndex++;
            }
            else
            {
                PuzzleGrid puzzle = new PuzzleGrid(_size);
                puzzle.Shuffle(_shuffleMoves);
                consoleState.GotoChild(new SlidePuzzleView(puzzle));
            }
        }
        else if (keyEvent.Key == ConsoleKey.Escape)
        {
            consoleState.Exit();
        }
    }

    private void SelectAscending()
    {
        _tabIndex = int.Abs((_tabIndex + 1) % 3);
    }

    private void SelectDescending()
    {
        if (_tabIndex <= 0)
        {
            _tabIndex = 2;
        }
        else
        {
            _tabIndex--;
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        int centerX = grid.Width / 2;
        int centerY = grid.Height / 2;

        DrawTextbox(grid, centerX, centerY - 2, 7, "Size", _size, _tabIndex == 0);
        DrawTextbox(grid, centerX, centerY + 1, 7, "Shuffle", _shuffleMoves, _tabIndex == 1);

        DrawButton(grid, centerX - 9, centerY + 5, 18, "Start", _tabIndex == 2);

        DrawingHelper.DrawTextArea(grid, centerX - 6, centerY - 6, "Slide puzzle", new ConsoleChar { ForegroundSimple = ConsoleColor.Green });

        DrawingHelper.DrawBox(grid, centerX - 13, centerY - 8, 26, 16, DrawStyle.DoubleDrawStyle, new ConsoleChar { ForegroundSimple = ConsoleColor.Blue });
    }

    private static void DrawTextbox(IConsoleGrid grid, int posX, int posY, int width, string label, int value, bool selected)
    {
        string text = value.ToString();
        ConsoleChar template = new ConsoleChar
        {
            ForegroundSimple = selected ? ConsoleColor.White : ConsoleColor.DarkGray
        };

        DrawingHelper.DrawString(grid, posX, posY, text.PadLeft(width));
        DrawingHelper.DrawString(grid, posX - label.Length - 1, posY, label);
        DrawingHelper.DrawBox(grid, posX, posY - 1, 8, 3, template);
    }

    private static void DrawButton(IConsoleGrid grid, int posX, int posY, int width, string text, bool selected)
    {
        ConsoleChar template = new ConsoleChar
        {
            ForegroundSimple = selected ? ConsoleColor.White : ConsoleColor.Gray
        };

        DrawingHelper.DrawBox(grid, posX, posY - 1, width, 3, selected ? DrawStyle.HeavyDrawStyle : DrawStyle.RoundedDrawStyle, template);
        DrawingHelper.DrawString(grid, posX + (width / 2) - (text.Length / 2), posY, text);
    }
}
