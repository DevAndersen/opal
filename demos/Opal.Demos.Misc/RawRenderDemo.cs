using Opal.ConsoleHandlers;
using Opal.Rendering;

namespace Opal.Demos.Misc;

internal static class RawRenderDemo
{
    public static Task RunAsync()
    {
        int loops = 0;

        using IConsoleHandler handler = IConsoleHandler.StartNewFullscreen();
        ConsoleRenderer renderer = new ConsoleRenderer(handler);
        ConsoleGrid grid = OpalManager.GetConsoleGrid(handler);

        handler.OnConsoleSizeChanged += (_, _) =>
        {
            grid.SetSize(handler.Width, handler.Height);
        };

        while (true)
        {
            for (int y = 0; y < handler.Height; y++)
            {
                for (int x = 0; x < handler.Width; x++)
                {
                    if (loops == 0 || grid[x, y].Equals(default(ConsoleChar)))
                    {
                        grid[x, y] = new ConsoleChar
                        {
                            Character = (char)0x2580,
                            ForegroundRgb = Random.Shared.Next(),
                            BackgroundRgb = Random.Shared.Next()
                        };
                    }
                    else
                    {
                        grid[x, y] = grid[x, y] with
                        {
                            Character = (char)0x2580,
                            ForegroundRgb = grid[x, y].ForegroundRgb + 1,
                            BackgroundRgb = grid[x, y].BackgroundRgb + 1
                        };
                    }
                }
            }

            renderer.Render(grid);
            loops++;
        }
    }
}
