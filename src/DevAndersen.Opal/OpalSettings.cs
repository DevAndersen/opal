namespace DevAndersen.Opal;

/// <summary>
/// Contains settings relevant for starting Opal.
/// </summary>
public class OpalSettings
{
    public bool UseAlternateBuffer { get; }

    public int WidthOffset { get; set; }

    public int HeightOffset { get; set; }

    public int? MinWidth { get; }

    public int? MinHeight { get; }

    public int? MaxWidth { get; }

    public int? MaxHeight { get; }

    public OpalSettings(bool useAlternateBuffer, int widthOffset, int heightOffset, int? minWidth, int? minHeight, int? maxWidth, int? maxHeight)
    {
        UseAlternateBuffer = useAlternateBuffer;

        WidthOffset = widthOffset;
        HeightOffset = heightOffset;

        MinWidth = minWidth;
        MinHeight = minHeight;

        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
    }

    /// <summary>
    /// Create a standardized setting for fullscreen mode.
    /// </summary>
    /// <returns></returns>
    public static OpalSettings CreateFullscreen()
    {
        return new OpalSettings(true, 0, 0, null, null, null, null);
    }

    /// <summary>
    /// Create a standardized setting for inline mode.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static OpalSettings CreateFixedInline(int width, int height)
    {
        return new OpalSettings(false, 0, 0, width, height, width, height);
    }

    /// <summary>
    /// Create a standardized setting for inline mode, with offsets.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="widthOffset"></param>
    /// <param name="heightOffset"></param>
    /// <returns></returns>
    public static OpalSettings CreateFixedInline(int width, int height, int widthOffset, int heightOffset)
    {
        return new OpalSettings(false, widthOffset, heightOffset, width, height, width, height);
    }

    /// <summary>
    /// Create a standardized setting for flexible mode.
    /// </summary>
    /// <param name="minWidth"></param>
    /// <param name="minHeight"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    /// <returns></returns>
    public static OpalSettings CreateFlexibleInline(int? minWidth, int? minHeight, int? maxWidth, int? maxHeight)
    {
        return new OpalSettings(false, 0, 0, minWidth, minHeight, maxWidth, maxHeight);
    }
}
