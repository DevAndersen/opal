using Opal.Drawing;
using Opal.Events;
using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.Misc;

internal static class DrawingDemo
{
    public static async Task RunAsync()
    {
        OpalManager manager = new OpalManager();
        await manager.StartAsync(new DrawingView());
    }
}

public class DrawingView : ConsoleView, IKeyInputHandler
{
    private int _x;
    private int _y;

    public Task HandleKeyInputAsync(KeyInput keyEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        switch (keyEvent.Key)
        {
            case ConsoleKey.RightArrow when _x < 10:
                _x++;
                break;
            case ConsoleKey.LeftArrow when _x > 0:
                _x--;
                break;
            case ConsoleKey.DownArrow when _y < 10:
                _y++;
                break;
            case ConsoleKey.UpArrow when _y > 0:
                _y--;
                break;
            case ConsoleKey.Escape:
                consoleState.ExitView();
                break;
        }

        return Task.CompletedTask;
    }

    public override void Render(IConsoleGrid grid)
    {
        grid.DrawString(2, 2, $"X: {_x}");
        grid.DrawString(2, 4, $"Y: {_y}");

        int posX = 40;
        int posY = 2;
        int width = 24;
        int height = 12;

        grid.DrawBox(posX, posY, width, height, DrawStyle.DoubleDrawStyle, new ConsoleChar { ForegroundSimple = ConsoleColor.Magenta });
        grid.DrawTextArea(posX + 1, posY + 1, width - 2, height - 2, _x, _y, "Line one\nLine two\r\nLine three\n\nLine five\nThis line is really rather long.", new ConsoleChar { ForegroundRgb = 0x3377ff });
    }
}
