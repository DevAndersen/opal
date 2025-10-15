using Opal.Rendering;
using Opal.Views;

namespace Opal.Demo;

internal class MouseInputDemo
{
    public static void Run()
    {
        OpalController controller = new OpalController();
        controller.Start(new MouseInputDemoView());
    }
}

public class MouseInputDemoView : ConsoleView, IKeyInputHandler, IMouseInputHandler
{
    private readonly List<Particle> particles = new List<Particle>();

    private int posX = 0;
    private int posY = 0;

    public override void Update()
    {
        base.Update();

        for (int i = particles.Count - 1; i >= 0; i--)
        {
            Particle particle = particles[i];
            if (particle.Age > particle.MaxAge)
            {
                particles.RemoveAt(i);
            }
            else
            {
                particle.Update();
            }
        }

        for (int i = 0; i < Random.Shared.Next(25, 30); i++)
        {
            SpawnParticle(posX, posY);
        }
    }

    public void HandleKeyInput(KeyInput keyEvent)
    {
        if (keyEvent.Key == ConsoleKey.Escape)
        {
            ExitView();
        }
    }

    public void HandleMouseInput(MouseInput mouseEvent)
    {
        posX = mouseEvent.X;
        posY = mouseEvent.Y;
    }

    public override void Render(IConsoleGrid grid)
    {
        foreach (Particle particle in particles.OrderByDescending(x => x.Age > x.TransitionAge))
        {
            grid[particle.X, particle.Y] = particle.GetVisuals();
        }
    }

    private void SpawnParticle(int x, int y)
    {
        particles.Add(new Particle
        {
            EarlyAge = 3 + Random.Shared.Next(2),
            TransitionAge = 10 + Random.Shared.Next(2),
            MaxAge = 20 + Random.Shared.Next(7),
            X = x + Random.Shared.Next(-3, 2),
            Y = y + Random.Shared.Next(-3, 2)
        });
    }

    private class Particle
    {
        public int Age { get; set; }

        public int EarlyAge { get; set; }

        public int TransitionAge { get; set; }

        public int MaxAge { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public void Update()
        {
            Age++;
            X += Random.Shared.Next(2) == 0
                ? Random.Shared.Next(-2, 3)
                : 0;
            Y--;
        }

        public ConsoleChar GetVisuals()
        {
            if (Age < TransitionAge)
            {
                int color = Age < EarlyAge
                    ? 0xffffcc - (0x001133 * Age)
                    : 0xff0000 + (0x001100 * Age);

                return new ConsoleChar('█', color);
            }
            else
            {
                char character = (Age - TransitionAge) switch
                {
                    <5 => '#',
                    <10 => ':',
                    _ => '.'
                };
                return new ConsoleChar(character, int.Max(0, 0x888888 - (0x040404 * (Age - TransitionAge))));
            }
        }
    }
}
