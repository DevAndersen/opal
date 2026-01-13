using Opal.Views;

namespace Opal.Forms;

public interface IControl : IRenderable
{
    int PosX { get; set; }

    int PosY { get; set; }
}
