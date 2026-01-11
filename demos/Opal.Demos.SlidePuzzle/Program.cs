using Opal;
using Opal.Demos.SlidePuzzle.Views;

OpalController controller = new OpalController();
await controller.StartAsync(new MenuView());
