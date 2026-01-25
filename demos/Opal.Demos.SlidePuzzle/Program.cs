using Opal;
using Opal.Demos.SlidePuzzle.Views;

OpalManager manager = new OpalManager();
await manager.StartAsync(new MenuView());
