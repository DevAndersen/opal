using Opal;

Console.WriteLine("Loading, please wait...");

OpalController controller = new OpalController(OpalSettings.CreateFixedInline(32, 3));
await controller.StartAsync(new ProgressBarView(TimeSpan.FromSeconds(2)));

Console.WriteLine("Loading complete");
