namespace Opal.Forms.Controls;

/// <summary>
/// A rectangle, described by its position (<paramref name="PosX"/>, <paramref name="PosY"/>) and its dimensions (<paramref name="Width"/>, <paramref name="Height"/>).
/// </summary>
/// <param name="PosX"></param>
/// <param name="PosY"></param>
/// <param name="Width"></param>
/// <param name="Height"></param>
public record struct Rect(int PosX, int PosY, int Width, int Height);
