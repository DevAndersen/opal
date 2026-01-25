using Opal;

Console.WriteLine("Loading, please wait...");

OpalManager manager = new OpalManager(OpalSettings.CreateFixedInline(32, 3));
await manager.StartAsync(new ProgressBarView(TimeSpan.FromSeconds(2)));

Console.WriteLine("Loading complete");
