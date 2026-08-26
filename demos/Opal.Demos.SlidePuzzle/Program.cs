using Opal;
using Opal.Demos.SlidePuzzle.Views;

using OpalManager manager = new OpalManager();
await manager.StartAsync(new MenuView());
