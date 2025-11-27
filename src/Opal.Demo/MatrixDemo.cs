using Opal.Rendering;
using Opal.Views;

namespace Opal.Demo;

internal static class MatrixDemo
{
    public static void Run()
    {
        OpalController controller = new OpalController();
        controller.Start(new MatrixView());
    }
}

public class MatrixView : ConsoleView
{
    public int _age;
    private readonly List<MatrixParticle> _particles = [];

    public override void Update()
    {
        _age++;

        if (_age % 3 != 0)
        {
            return;
        }

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            MatrixParticle particle = _particles[i];
            if (particle.PosY - particle.Trail.Count >= ConsoleHeight)
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
            _particles.Add(new MatrixParticle(Random.Shared.Next(ConsoleWidth)));
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        foreach (MatrixParticle particle in _particles)
        {
            for (int i = 0; i < particle.Trail.Count; i++)
            {
                char c = particle.Trail.ElementAt(i);
                grid[particle.PosX, particle.PosY - i] = new ConsoleChar(
                    c,
                    i == 0
                        ? 0xffffff
                        : 0x00ff00 - ((byte.MaxValue / (particle.MaxLength + 1)) << 8));
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
