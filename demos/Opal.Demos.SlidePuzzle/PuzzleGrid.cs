using System.Diagnostics;

namespace Opal.Demos.SlidePuzzle;

internal class PuzzleGrid
{
    private readonly int[,] _grid;

    public int X { get; private set; }

    public int Y { get; private set; }

    public int Width => _grid.GetLength(0);

    public int Height => _grid.GetLength(1);

    public int ValueToHide { get; private set; }

    public int Moves { get; private set; }

    public Stopwatch Timer { get; private set; }

    public bool IsSolved { get; private set; }

    public int this[int x, int y]
    {
        get
        {
            return IsWithinGrid(x, y)
                ? _grid[x, y]
                : 0;
        }
    }

    public PuzzleGrid(int size)
    {
        _grid = GenerateGrid(size);
        X = _grid.GetLength(0) - 1;
        Y = _grid.GetLength(1) - 1;
        ValueToHide = _grid.Length;
        Timer = new Stopwatch();
    }

    public static int[,] GenerateGrid(int size)
    {
        int i = 1;
        int[,] grid = new int[size, size];

        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, y] = i++;
            }
        }

        return grid;
    }

    public void Shuffle(int moves)
    {
        int movesDone = 0;

        Func<bool, bool>[] possibleMoves = [MoveUp, MoveDown, MoveLeft, MoveRight];

        while (movesDone < moves)
        {
            Func<bool, bool> move = Random.Shared.GetItems(possibleMoves, 1)[0];
            if (move(false))
            {
                movesDone++;
            }
        }

        Moves = 0;
        Timer.Restart();
    }

    public bool MoveUp(bool countMove)
    {
        return Move(0, -1, countMove);
    }

    public bool MoveDown(bool countMove)
    {
        return Move(0, 1, countMove);
    }

    public bool MoveLeft(bool countMove)
    {
        return Move(-1, 0, countMove);
    }

    public bool MoveRight(bool countMove)
    {
        return Move(1, 0, countMove);
    }

    private bool Move(int x, int y, bool countMove)
    {
        bool validMove = MoveInternal(x, y);
        if (validMove && countMove)
        {
            Moves++;
            CheckIsSolved();
        }
        return validMove;
    }

    private bool MoveInternal(int x, int y)
    {
        int newX = X + x;
        int newY = Y + y;

        if (!IsWithinGrid(newX, newY))
        {
            return false;
        }

        (_grid[X, Y], _grid[newX, newY]) = (_grid[newX, newY], _grid[X, Y]);
        (X, Y) = (newX, newY);

        return true;
    }

    private bool IsWithinGrid(int x, int y)
    {
        bool validX = x >= 0 && x <= _grid.GetLength(0) - 1;
        bool validY = y >= 0 && y <= _grid.GetLength(1) - 1;

        return validX && validY;
    }

    public bool CheckIsCorrect(int x, int y)
    {
        int correctValue = x + (y * _grid.GetLength(1)) + 1;
        return _grid[x, y] == correctValue;
    }

    private bool CheckIsSolved()
    {
        for (int y = 0; y < _grid.GetLength(1); y++)
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                if (!CheckIsCorrect(x, y))
                {
                    return false;
                }
            }
        }

        Timer.Stop();
        IsSolved = true;
        return true;
    }
}
