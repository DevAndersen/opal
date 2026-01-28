using Opal.Events;
using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.Misc;

public static class ViewDemo
{
    public static async Task RunAsync()
    {
        OpalManager manager = new OpalManager();
        await manager.StartAsync(new WriteView());
    }
}

public class TimeView : ConsoleView
{
    private int _count = 0;

    //public override int Delay => 100;

    public override void Update(IConsoleState consoleState)
    {
        _count++;
        if (_count > 50)
        {
            consoleState.ExitView();
            return;
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        string s = $" The time is {DateTime.Now} ";
        for (int i = 0; i < s.Length; i++)
        {
            char item = s[i];

            int fg = (int)((float)i / s.Length * 255);
            int bg = 255 - (fg / 2);

            grid[2 + i, 2] = new ConsoleChar(item, fg, bg << 8);
        }
    }
}

public class WriteView : ConsoleView, IKeyInputHandler
{
    private string _s = string.Empty;

    public async Task HandleKeyInputAsync(KeyInput keyEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        if (keyEvent.Key == ConsoleKey.Enter)
        {
            consoleState.GotoChild(new TimeView());
        }
        else if (keyEvent.Key == ConsoleKey.X)
        {
            consoleState.GotoChild(new ColorView());
        }
        else if (keyEvent.Key == ConsoleKey.Escape)
        {
            consoleState.ExitView();
        }
        else if (keyEvent.Key == ConsoleKey.Backspace)
        {
            if (_s.Length != 0)
            {
                _s = _s[..^1];
            }
        }
        else if (char.IsLetterOrDigit(keyEvent.KeyChar) || char.IsWhiteSpace(keyEvent.KeyChar) || char.IsPunctuation(keyEvent.KeyChar))
        {
            _s += keyEvent.KeyChar;
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        for (int i = 0; i < _s.Length; i++)
        {
            char item = _s[i];
            int fg = int.Clamp((_s.Length - i) * 15, 0, 255);
            grid[2 + i, 2] = new ConsoleChar(item, fg + (127 << 8) + (127 << 16))
            {
                Modes = item == 'a' ? ConsoleCharModes.Underscore : ConsoleCharModes.None
            };
        }

        grid[0, 0] = new ConsoleChar('X');
        grid[0, grid.Height - 1] = new ConsoleChar('X');
        grid[grid.Width - 1, 0] = new ConsoleChar('X');
        grid[grid.Width - 1, grid.Height - 1] = new ConsoleChar('X');
    }
}

public class ColorView : ConsoleView
{
    private readonly DateTime _startTime;
    private DateTime _lastTime;
    private int _frame;
    private (int f, int b)[,] _colors = default!;

    public ColorView()
    {
        _startTime = _lastTime = DateTime.Now;
    }


    public override void Update(IConsoleState consoleState)
    {
        if (_frame == 0)
        {
            _colors = new (int, int)[consoleState.Width, consoleState.Height];
        }

        for (int y = 0; y < consoleState.Height; y++)
        {
            for (int x = 0; x < consoleState.Width; x++)
            {
                if (_frame == 0)
                {
                    _colors[x, y] = new(Random.Shared.Next(), Random.Shared.Next());
                }
                else
                {
                    _colors[x, y] = _colors[x, y] with
                    {
                        f = _colors[x, y].f + 1,
                        b = _colors[x, y].b + 1,
                    };
                }
            }
        }

        string fps = double.Round(1000 / (DateTime.Now - _lastTime).TotalMilliseconds, 1).ToString("#.0");
        Console.Title = $"Frame {_frame:0000} | {fps} FPS";

        if ((DateTime.Now - _startTime) > TimeSpan.FromSeconds(15))
        {
            consoleState.ExitView();
            return;
        }

        _lastTime = DateTime.Now;
        _frame++;
    }

    public override void Render(IConsoleGrid grid)
    {
        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                grid[x, y] = new ConsoleChar
                {
                    Character = (char)0x2580,
                    ForegroundRgb = _colors[x, y].f,
                    BackgroundRgb = _colors[x, y].b
                };
            }
        }
    }
}
