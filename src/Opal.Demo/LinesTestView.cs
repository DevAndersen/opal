using Opal.Rendering;
using Opal.Views;

namespace Opal.Demo;

public static class LinesTestDemo
{
    public static async Task RunAsync()
    {
        OpalController controller = new OpalController();
        await controller.StartAsync(new LinesTestView());
        Console.WriteLine($"GC: {GC.CollectionCount(0)} | {GC.CollectionCount(1)} | {GC.CollectionCount(2)} | {GC.CollectionCount(3)}");
    }
}

public class LinesTestView : ConsoleView
{
    private int cycles = 0;

    //public override int Delay => 200;

    public override void Update(IConsoleState consoleState)
    {
        if (cycles > 10000)
        {
            consoleState.ExitView();
            return;
        }
        cycles++;
    }

    public override void Render(IConsoleGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                int index = (short)'0' + (x % 10);
                grid[x, y] = new ConsoleChar((char)index, (ConsoleColor)(5 + (y % 2) * ((cycles % 2) * 2)));
            }
        }

        grid[0, 0] = new ConsoleChar('X');
        grid[0, grid.Height - 1] = new ConsoleChar('X');
        grid[grid.Width - 1, 0] = new ConsoleChar('X');
        grid[grid.Width - 1, grid.Height - 1] = new ConsoleChar('X');
    }
}
