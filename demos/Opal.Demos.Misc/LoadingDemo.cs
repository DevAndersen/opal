using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.Misc;

internal static class LoadingDemo
{
    public static async Task RunAsync()
    {
        using OpalManager manager = new OpalManager(OpalSettings.CreateFixedInline(32, 16, 5, 5));
        await manager.StartAsync(new LoadingView());
    }
}

public class LoadingView : ConsoleView
{
    private int _count;
    private DateTime _end;

    protected override void Initialize()
    {
        _end = DateTime.Now.AddSeconds(3);
    }

    public override void Update(IConsoleState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        _count++;

        if (DateTime.Now > _end)
        {
            state.ExitView();
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                grid[x: x, y] = new ConsoleChar('.');
            }
        }

        string s = _count.ToString().PadLeft(5, '.');
        for (int i = 0; i < s.Length; i++)
        {
            grid[i, 1] = new ConsoleChar(s[i]);
        }
    }
}
