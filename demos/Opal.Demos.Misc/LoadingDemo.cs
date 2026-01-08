using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.Misc;

internal class LoadingDemo
{
    public static async Task RunAsync()
    {
        OpalController controller = new OpalController(OpalSettings.CreateFixedInline(32, 16, 5, 5));
        await controller.StartAsync(new LoadingView());
    }
}

public class LoadingView : ConsoleView
{
    private int _count;
    private DateTime _end;

    public override void Initialize()
    {
        _end = DateTime.Now.AddSeconds(3);
    }

    public override void Update(IConsoleState consoleState)
    {
        _count++;

        if (DateTime.Now > _end)
        {
            consoleState.ExitView();
            return;
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                grid[x, y] = new ConsoleChar('.');
            }
        }

        string s = _count.ToString().PadLeft(5, '.');
        for (int i = 0; i < s.Length; i++)
        {
            grid[i, 1] = new ConsoleChar(s[i]);

        }
    }
}
