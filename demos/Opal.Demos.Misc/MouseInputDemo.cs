using Opal.Drawing;
using Opal.Events;
using Opal.Rendering;
using Opal.Views;

namespace Opal.Demos.Misc;

internal static class MouseInputDemo
{
    public static async Task RunAsync()
    {
        OpalManager manager = new OpalManager();
        await manager.StartAsync(new MouseInputDemoView());
    }
}

public class MouseInputDemoView : ConsoleView, IKeyInputHandler, IMouseMoveInputHandler
{
    private readonly List<Particle> _particles = [];

    private int _posX;
    private int _posY;

    public override void Update(IConsoleState consoleState)
    {
        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            Particle particle = _particles[i];
            if (particle.Age > particle.MaxAge)
            {
                _particles.RemoveAt(i);
            }
            else
            {
                particle.Update();
            }
        }

        for (int i = 0; i < Random.Shared.Next(25, 30); i++)
        {
            SpawnParticle(_posX, _posY);
        }
    }

    public Task HandleKeyInputAsync(KeyInput keyEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        if (keyEvent.Key == ConsoleKey.Escape)
        {
            consoleState.ExitView();
        }

        return Task.CompletedTask;
    }

    public Task HandleMouseMoveInputAsync(MouseMoveInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        _posX = mouseEvent.X;
        _posY = mouseEvent.Y;

        return Task.CompletedTask;
    }

    public override void Render(IConsoleGrid grid)
    {
        foreach (Particle particle in _particles.OrderByDescending(x => x.Age > x.TransitionAge))
        {
            grid[particle.X, particle.Y] = particle.GetVisuals();
        }
    }

    private void SpawnParticle(int x, int y)
    {
        _particles.Add(new Particle
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
        public int Age { get; private set; }

        public int EarlyAge { get; init; }

        public int TransitionAge { get; init; }

        public int MaxAge { get; init; }

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

                return new ConsoleChar(CharLib.Block.FullBlock, color);
            }

            char character = (Age - TransitionAge) switch
            {
                < 5 => '#',
                < 10 => ':',
                _ => '.'
            };

            return new ConsoleChar(character, int.Max(0, 0x888888 - (0x040404 * (Age - TransitionAge))));
        }
    }
}
