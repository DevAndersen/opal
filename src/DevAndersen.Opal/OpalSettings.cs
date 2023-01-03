namespace DevAndersen.Opal;

public class OpalSettings
{
    public bool UseAlternateBuffer { get; }

    public int WidthOffset { get; set; }

    public int HeightOffset { get; set; }

    public int? MinWidth { get; }

    public int? MaxWidth { get; }

    public int? MinHeight { get; }

    public int? MaxHeight { get; }

    public OpalSettings(bool useAlternateBuffer, int widthOffset, int heightOffset, int? minWidth, int? maxWidth, int? minHeight, int? maxHeight)
    {
        UseAlternateBuffer = useAlternateBuffer;

        WidthOffset = widthOffset;
        HeightOffset = heightOffset;

        MinWidth = minWidth;
        MaxWidth = maxWidth;
        MinHeight = minHeight;
        MaxHeight = maxHeight;
    }

    public static OpalSettings CreateFullscreen()
    {
        return new OpalSettings(true, 0, 0, null, null, null, null);
    }

    public static OpalSettings CreateFixedInline(int width, int height)
    {
        return new OpalSettings(false, 0, 0, width, width, height, height);
    }

    public static OpalSettings CreateFixedInline(int width, int height, int widthOffset, int heightOffset)
    {
        return new OpalSettings(false, widthOffset, heightOffset, width, width, height, height);
    }

    public static OpalSettings CreateFlexibleInline(int? minWidth, int? maxWidth, int? minHeight, int? maxHeight)
    {
        return new OpalSettings(false, 0, 0, minWidth, maxWidth, minHeight, maxHeight);
    }
}
