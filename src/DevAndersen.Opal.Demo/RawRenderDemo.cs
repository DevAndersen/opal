using DevAndersen.Opal.ConsoleHandlers;
using DevAndersen.Opal.Rendering;
using System.Diagnostics;

namespace DevAndersen.Opal.Demo;

internal class RawRenderDemo
{
    public static void Run()
    {
        bool keepGoing = true;
        int loops = 0;

        using IConsoleHandler handler = IConsoleHandler.StartNew();
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
                    var v2 = grid[x, y];

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

            //for (int i = 0; i < 100; i++)
            //{
            //    grid[6 + i, 3] = new ConsoleChar
            //    {
            //        Character = (char)(48 + (i % 10)),
            //        ForegroundRgb = 0xff0000 + (i << 8) + (255 - i * 2)
            //    };

            //    grid[6 + i, 4] = new ConsoleChar
            //    {
            //        Character = (char)0x2584,
            //        ForegroundRgb = 0x880000 + (i << 8) + (255 - i * 2),
            //        BackgroundRgb = 0xff0000 + (i << 8) + (255 - i * 2)
            //    };
            //}

            //grid[posX, posY] = new ConsoleChar
            //{
            //    Character = (char)0x2588,
            //    ForegroundSimple = ConsoleColor.Green,
            //    BackgroundSimple = ConsoleColor.Magenta
            //};

            renderer.Render(grid);

            //switch (Console.ReadKey(true).Key)
            //{
            //    case ConsoleKey.W:
            //        posY = Math.Max(posY - 1, 0);
            //        break;
            //    case ConsoleKey.A:
            //        posX = Math.Max(posX - 1, 0);
            //        break;
            //    case ConsoleKey.S:
            //        posY = Math.Min(posY + 1, height - 1);
            //        break;
            //    case ConsoleKey.D:
            //        posX = Math.Min(posX + 1, width - 1);
            //        break;
            //    default:
            //        break;
            //}
            if (loops >= 3000000)
            {
                //keepGoing = false;
            }
            else
            {
                loops++;
            }
            //Console.Title = loops.ToString();
        }
        sw.Stop();
        Console.Title = $"Rendered {loops} frames in {sw.ElapsedMilliseconds} ms";

        Console.ReadLine();
    }
}
