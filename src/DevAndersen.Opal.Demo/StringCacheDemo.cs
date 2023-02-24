using DevAndersen.Opal.Rendering;
using DevAndersen.Opal.Views;

namespace DevAndersen.Opal.Demo;

internal class StringCacheDemo
{
    public static void Run()
    {
        OpalController controller = new OpalController();
        controller.Start(new StringCacheView());
    }
}

public class StringCacheView : ConsoleView, IKeyboardInputHandler
{
    private bool hasRenderedOnce = false;

    public override void Render(ConsoleGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                char c = (char)('0' + (x % 10));
                ConsoleColor color = x == grid.Width - 1 ? ConsoleColor.Green : ConsoleColor.DarkGray;
                grid[x, y] = new ConsoleChar(c, color);
            }
        }

        string green = "\U0001f7e2";
        string yellow = "\U0001f7e1";
        string red = "\U0001F534";

        grid[2, 1] = new ConsoleChar(green);
        grid[2, 2] = new ConsoleChar(yellow);
        grid[2, 3] = new ConsoleChar(red);
    }

    public override void Update()
    {
        base.Update();
        if (hasRenderedOnce)
        {
            ExitView();
        }
        hasRenderedOnce = true;
    }

    public void HandleKeyInput(ConsoleKeyEvent keyEvent)
    {
    }
}
