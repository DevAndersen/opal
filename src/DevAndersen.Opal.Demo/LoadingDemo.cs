using DevAndersen.Opal.Rendering;
using DevAndersen.Opal.Views;

namespace DevAndersen.Opal.Demo;

internal class LoadingDemo
{
    public static void Run()
    {
        OpalController controller = new OpalController(OpalSettings.CreateFixedInline(5, 5, 8, 8));
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

    public override void Render(ConsoleGrid grid)
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
