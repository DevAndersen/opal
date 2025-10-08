namespace Opal.ConsoleHandlers;

public class ConsoleSizeChangedEventArgs : EventArgs
{
    public int NewWidth { get; }

    public int NewHeight { get; }

    public ConsoleSizeChangedEventArgs(int newWidth, int newHeight)
    {
        NewWidth = newWidth;
        NewHeight = newHeight;
    }
}
