using Opal.Views;

namespace Opal.Forms.Controls;

public interface IControl : IRenderable
{
    /// <summary>
    /// The requested X-dimension (horizontal) coordinate of the control, relative to its parent.
    /// </summary>
    /// <remarks>
    /// Depending on the parent, this value may or may not be used.
    /// </remarks>
    int PosX { get; set; }

    /// <summary>
    /// The requested Y-dimension (vertical) coordinate of the control, relative to its parent.
    /// </summary>
    /// <remarks>
    /// Depending on the parent, this value may or may not be used.
    /// </remarks>
    int PosY { get; set; }

    /// <summary>
    /// Return the desired control size for rendering and mouse input.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    Rect GetDesiredSize(int width, int height);
}
