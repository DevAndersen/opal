using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.Misc;

internal static class MatrixDemo
{
    public static async Task RunAsync()
    {
        OpalController controller = new OpalController();
        await controller.StartAsync(new MatrixView());
    }
}

public class MatrixView : ConsoleView
{
    public int _age;
    private readonly List<MatrixParticle> _particles = [];

    public override void Update(IConsoleState consoleState)
    {
        _age++;

        if (_age % 3 != 0)
        {
            return;
        }

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            MatrixParticle particle = _particles[i];
            if (particle.PosY - particle.Trail.Count >= consoleState.Height)
            {
                _particles.RemoveAt(i);
            }
            else
            {
                particle.Update();
            }
        }

        if (Random.Shared.Next(2) == 0)
        {
            _particles.Add(new MatrixParticle(Random.Shared.Next(consoleState.Width)));
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        foreach (MatrixParticle particle in _particles)
        {
            for (int i = 0; i < particle.Trail.Count; i++)
            {
                byte primary = (byte)(255 - (float)i / particle.Trail.Count * 200);
                byte secondary = i switch
                {
                    0 => 255,
                    1 => 255 / 2,
                    2 => 255 / 3,
                    _ => 0
                };

                int col = secondary + (primary << 8) + (secondary << 16);

                char c = particle.Trail.ElementAt(i);
                grid[particle.PosX, particle.PosY - i] = new ConsoleChar(c, col);
            }
        }
    }

    private class MatrixParticle
    {
        private static readonly string _validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-*/+=ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺ";

        public int MaxLength { get; }

        public int PosX { get; }

        public int PosY { get; private set; }

        public MatrixParticle(int posX)
        {
            MaxLength = Random.Shared.Next(8, 20);
            PosX = posX;
            PosY = 0;
        }

        public IReadOnlyCollection<char> Trail => _trail;

        private readonly LinkedList<char> _trail = [];

        public void Update()
        {
            PosY++;

            char c = _validChars[Random.Shared.Next(_validChars.Length)];
            _trail.AddFirst(c);

            if (_trail.Count > MaxLength)
            {
                _trail.RemoveLast();
            }
        }
    }
}
