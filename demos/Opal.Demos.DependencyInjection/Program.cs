using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Opal;
using Opal.Drawing;
using Opal.Events;
using Opal.Forms;
using Opal.Forms.Controls;
using Opal.Rendering;
using Opal.Views;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

builder.Services
    .AddHostedService<HostedService>()
    .AddSingleton<OpalManager>()
    .AddSingleton<MainForm>()
    .AddSingleton<PersistentCounterForm>()
    .AddTransient<TemporaryCounterForm>();

IHost app = builder.Build();
await app.RunAsync();

internal class HostedService : IHostedService
{
    private readonly IHost _host;
    private readonly IServiceProvider _serviceProvider;
    private readonly OpalManager _opalManager;

    public HostedService(IHost host, IServiceProvider serviceProvider, OpalManager opalManager)
    {
        _host = host;
        _serviceProvider = serviceProvider;
        _opalManager = opalManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _opalManager.StartAsync(_serviceProvider.GetRequiredService<MainForm>(), cancellationToken);
        await _host.StopAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}

public class MainForm : ConsoleForm
{
    private readonly IServiceProvider _serviceProvider;

    public MainForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        ChildControls.Add(new Button
        {
            Text = "Go to persistent counter",
            PosX = 0,
            PosY = 2,
            Height = 3,
            OnClick = new ConsoleEventHandler(() => { })
        });

        ChildControls.Add(new Button
        {
            Text = "Go to temporary counter",
            PosX = 0,
            PosY = 6,
            Height = 3,
            OnClick = new ConsoleEventHandler(() => { })
        });
    }
}

public abstract class CounterForm : ConsoleForm
{
    private int _counter = 0;

    public abstract string Title { get; }

    public override void Render(IConsoleGrid grid)
    {
        base.Render(grid);
        grid.DrawString(2, 2, Title);
        grid.DrawString(2, 3, $"Counter: {_counter}");
    }

    public override async Task HandleKeyInputAsync(KeyInput keyEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        await base.HandleKeyInputAsync(keyEvent, consoleState, cancellationToken);

        if (keyEvent.Key == ConsoleKey.UpArrow)
        {
            _counter++;
        }
        else if (keyEvent.Key == ConsoleKey.DownArrow)
        {
            _counter--;
        }
        else if (keyEvent.Key == ConsoleKey.Escape)
        {
            consoleState.ExitView();
        }
    }
}

public class PersistentCounterForm : CounterForm
{
    public override string Title => "Persistent counter";
}

public class TemporaryCounterForm : CounterForm
{
    public override string Title => "Temporary counter";
}
