using Opal.Drawing;
using Opal.Events;
using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.Misc;

internal class DrawingDemo
{
    public static async Task RunAsync()
    {
        OpalController controller = new OpalController();
        await controller.StartAsync(new DrawingView());
    }
}

public class DrawingView : ConsoleView, IKeyInputHandler
{
    private int _x = 0;
    private int _y = 0;

    public void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
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
            default:
                break;
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        DrawingHelper.DrawString(grid, 2, 2, $"X: {_x}");
        DrawingHelper.DrawString(grid, 2, 4, $"Y: {_y}");

        int posX = 40;
        int posY = 2;
        int width = 24;
        int height = 12;

        DrawingHelper.DrawBox(grid, posX, posY, width, height, DrawStyle.DoubleDrawStyle, new ConsoleChar { ForegroundSimple = ConsoleColor.Magenta });
        DrawingHelper.DrawTextArea(grid, posX + 1, posY + 1, width - 2, height - 2, _x, _y, "Line one\nLine two\r\nLine three\n\nLine five\nThis line is really rather long.", new ConsoleChar { ForegroundRgb = 0x3377ff });
    }
}
