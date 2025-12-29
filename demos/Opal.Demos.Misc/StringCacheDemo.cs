using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.Misc;

internal class StringCacheDemo
{
    public static async Task RunAsync()
    {
        OpalController controller = new OpalController();
        await controller.StartAsync(new StringCacheView());
    }
}

public class StringCacheView : ConsoleView, IKeyInputHandler
{
    private bool _hasRenderedOnce = false;

    public override void Render(IConsoleGrid grid)
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

    public override void Update(IConsoleState consoleState)
    {
        if (_hasRenderedOnce)
        {
            consoleState.ExitView();
        }
        _hasRenderedOnce = true;
    }

    public void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
    {
    }
}
