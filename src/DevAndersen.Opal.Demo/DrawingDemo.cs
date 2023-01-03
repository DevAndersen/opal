using DevAndersen.Opal.Drawing;
using DevAndersen.Opal.Rendering;
using DevAndersen.Opal.Views;

namespace DevAndersen.Opal.Demo;

internal class DrawingDemo
{
    public static void Run()
    {
        OpalController controller = new OpalController();
        controller.Start(new DrawingView());
    }
}

public class DrawingView : ConsoleView
{
    public override int Delay => 50;

    public override void Render(ConsoleGrid grid)
    {
        ConsolePainter.DrawBox(grid, 2, 2, 5, 7, new ConsoleChar { Modes = ConsoleCharModes.DoubleUnderscore });
        ConsolePainter.DrawBox(grid, 12, 2, 5, 7, DrawStyle.HeavyDrawStyle, new ConsoleChar { ForegroundSimple = ConsoleColor.Green });

        ConsolePainter.DrawHorizontalLine(grid, 12, 13, 5);
        ConsolePainter.DrawVerticalLine(grid, 5, 12, 3);

        ConsolePainter.DrawString(grid, 22, 3, "Some text");
        ConsolePainter.DrawWrappingString(grid, 22, 8, 6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");

        ConsolePainter.DrawTextArea(grid, 38, 3, "Line one\nLine two\r\nLine three\n\n\tIndented line");
    }
}
