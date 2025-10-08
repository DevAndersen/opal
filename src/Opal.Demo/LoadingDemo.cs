using Opal.Rendering;
using Opal.Views;

namespace Opal.Demo;

internal class LoadingDemo
{
    public static void Run()
    {
        OpalController controller = new OpalController(OpalSettings.CreateFixedInline(32, 16, 5, 5));
        controller.Start(new LoadingView());
    }
}

public class LoadingView : ConsoleView
{
    private int count;
    private DateTime end;

    public override void Init()
    {
        base.Init();
        end = DateTime.Now.AddSeconds(3);
    }

    public override void Update()
    {
        base.Update();
        count++;

        if (DateTime.Now > end)
        {
            ExitView();
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

        string s = count.ToString().PadLeft(5, '.');
        for (int i = 0; i < s.Length; i++)
        {
            grid[i, 1] = new ConsoleChar(s[i]);

        }
    }
}
