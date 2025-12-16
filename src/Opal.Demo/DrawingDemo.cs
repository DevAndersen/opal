using Opal.Drawing;
using Opal.Rendering;
using Opal.Views;

namespace Opal.Demo;

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
    int x = 0;
    int y = 0;

    public void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
    {
        switch (keyEvent.Key)
        {
            case ConsoleKey.RightArrow when x < 10:
                x++;
                break;
            case ConsoleKey.LeftArrow when x > 0:
                x--;
                break;
            case ConsoleKey.DownArrow when y < 10:
                y++;
                break;
            case ConsoleKey.UpArrow when y > 0:
                y--;
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
        ConsolePainter.DrawString(grid, 2, 2, $"X: {x}");
        ConsolePainter.DrawString(grid, 2, 4, $"Y: {y}");

        int posX = 40;
        int posY = 2;
        int width = 24;
        int height = 12;

        ConsolePainter.DrawBox(grid, posX, posY, width, height, DrawStyle.DoubleDrawStyle, new ConsoleChar { ForegroundSimple = ConsoleColor.Magenta });
        ConsolePainter.DrawTextArea(grid, posX + 1, posY + 1, width - 2, height - 2, x, y, 0, "Line one\nLine two\r\nLine three\n\nLine five\nThis line is really rather long.", new ConsoleChar { ForegroundRgb = 0x3377ff });
    }
}
