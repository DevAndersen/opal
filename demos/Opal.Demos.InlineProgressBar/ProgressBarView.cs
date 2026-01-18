using Opal.Drawing;
using Opal.Rendering;
using Opal.Views;
using System.Diagnostics;

internal class ProgressBarView : ConsoleView
{
    private readonly Stopwatch _stopwatch;
    private readonly TimeSpan _target;

    public ProgressBarView(TimeSpan target)
    {
        _target = target;
        _stopwatch = Stopwatch.StartNew();
    }

    public override void Update(IConsoleState state)
    {
        if (_stopwatch.Elapsed >= _target)
        {
            _stopwatch.Stop();
            state.Exit();
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        grid.DrawBox(0, 0, grid.Width, grid.Height, DrawStyle.RoundedDrawStyle);

        float ratio = float.Clamp((float)(_stopwatch.Elapsed / _target), 0, 1);

        int percentage = (int)(ratio * 100);
        string percentageString = $"{percentage}%".PadLeft(4);
        grid.DrawString(grid.Width / 2 - percentageString.Length / 2, 1, percentageString);

        float width = grid.Width - 4;
        for (int i = 0; i < width; i++)
        {
            float progress = i / width;
            int x = i + 2;
            int y = 1;
            if (ratio > progress)
            {
                int rgb = GetColor(progress);
                grid[x, y] = new ConsoleChar(grid[x, y].Character, 0x010101, rgb);
            }
            else
            {
                grid[x, y] = new ConsoleChar(grid[x, y].Character, 0x010101, ConsoleColor.DarkGray);
            }
        }
    }

    public static int GetColor(float progress)
    {
        int red = (byte)(GetColorForChannel(progress, 2) * 127) + 127;
        int green = (byte)(GetColorForChannel(progress, 3) * 127) + 127;

        return (red << 8) + (green << 16);
    }

    private static float GetColorForChannel(float progress, int channel)
    {
        return float.Sin(progress * float.Pi + (float.Pi / 2) + (float.Tau / 3 * channel));
    }
}
