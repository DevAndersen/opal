﻿using DevAndersen.Opal.ConsoleHandlers;
using DevAndersen.Opal.Rendering;
using System.Diagnostics;

namespace DevAndersen.Opal.Demo;

internal class RawRenderDemo
{
    public static void Run()
    {
        bool keepGoing = true;
        int loops = 0;

        using IConsoleHandler handler = IConsoleHandler.StartNewFullscreen();
        ConsoleRenderer renderer = new ConsoleRenderer(handler);
        ConsoleGrid grid = OpalController.GetConsoleGrid(handler);
        Stopwatch sw = Stopwatch.StartNew();

        handler.OnConsoleSizeChanged += (s, e) =>
        {
            grid.SetSize(handler.Width, handler.Height);
        };

        while (keepGoing)
        {
            for (int y = 0; y < handler.Height; y++)
            {
                for (int x = 0; x < handler.Width; x++)
                {
                    ConsoleChar character = grid[x, y];

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
        sw.Stop();
        Console.Title = $"Rendered {loops} frames in {sw.ElapsedMilliseconds} ms";

        Console.ReadLine();
    }
}
