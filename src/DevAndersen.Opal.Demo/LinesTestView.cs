using DevAndersen.Opal.Rendering;
using DevAndersen.Opal.Views;

namespace DevAndersen.Opal.Demo;

public static class LinesTestDemo
{
    public static void Run()
    {
        OpalController controller = new OpalController();
        controller.Start(new LinesTestView());
        Console.WriteLine($"GC: {GC.CollectionCount(0)} | {GC.CollectionCount(1)} | {GC.CollectionCount(2)} | {GC.CollectionCount(3)}");
    }
}

public class LinesTestView : ConsoleView
{
    private int cycles = 0;

    public override int Delay => 200;

    public override void Update()
    {
        if (cycles > 10000)
        {
            ExitView();
            return;
        }
        cycles++;
    }

    public override void Render(IConsoleGrid grid)
    {
        for (int x = 0; x < ConsoleWidth; x++)
        {
            for (int y = 0; y < ConsoleHeight; y++)
            {
                int index = (short)'0' + (x % 10);
                grid[x, y] = new ConsoleChar((char)index, (ConsoleColor)(5 + (y % 2) * ((cycles % 2) * 2)));
            }
        }

        grid[0, 0] = new ConsoleChar('X');
        grid[0, ConsoleHeight - 1] = new ConsoleChar('X');
        grid[ConsoleWidth - 1, 0] = new ConsoleChar('X');
        grid[ConsoleWidth - 1, ConsoleHeight - 1] = new ConsoleChar('X');
    }
}
