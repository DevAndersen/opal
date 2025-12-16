using Opal.Rendering;
using Opal.Views;

namespace Opal.Demo;

public static class ViewDemo
{
    public static async Task RunAsync()
    {
        OpalController controller = new OpalController();
        await controller.StartAsync(new WriteView());
    }
}

public class TimeView : ConsoleView
{
    private int count = 0;

    //public override int Delay => 100;

    public override void Update(IConsoleState consoleState)
    {
        count++;
        if (count > 50)
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
    private string s = string.Empty;

    public void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
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
            if (s.Length != 0)
            {
                s = s[..^1];
            }
        }
        else if (char.IsLetterOrDigit(keyEvent.KeyChar) || char.IsWhiteSpace(keyEvent.KeyChar) || char.IsPunctuation(keyEvent.KeyChar))
        {
            s += keyEvent.KeyChar;
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        for (int i = 0; i < s.Length; i++)
        {
            char item = s[i];
            int fg = int.Clamp((s.Length - i) * 15, 0, 255);
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
    private readonly DateTime startTime;
    private DateTime lastTime;
    private int frame;

    public ColorView()
    {
        startTime = lastTime = DateTime.Now;
    }

    private (int f, int b)[,] colors = default!;

    public override void Update(IConsoleState consoleState)
    {
        if (frame == 0)
        {
            colors = new (int, int)[consoleState.Width, consoleState.Height];
        }

        for (int y = 0; y < consoleState.Height; y++)
        {
            for (int x = 0; x < consoleState.Width; x++)
            {
                if (frame == 0)
                {
                    colors[x, y] = new(Random.Shared.Next(), Random.Shared.Next());
                }
                else
                {
                    colors[x, y] = colors[x, y] with
                    {
                        f = colors[x, y].f + 1,
                        b = colors[x, y].b + 1,
                    };
                }
            }
        }

        string fps = double.Round(1000 / (DateTime.Now - lastTime).TotalMilliseconds, 1).ToString("#.0");
        Console.Title = $"Frame {frame:0000} | {fps} FPS";

        if ((DateTime.Now - startTime) > TimeSpan.FromSeconds(15))
        {
            consoleState.ExitView();
            return;
        }

        lastTime = DateTime.Now;
        frame++;
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
                    ForegroundRgb = colors[x, y].f,
                    BackgroundRgb = colors[x, y].b
                };
            }
        }
    }
}
