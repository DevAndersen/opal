namespace Opal.Native.Windows;

/// <summary>
/// The standard console devices (standard streams).
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/getstdhandle"/>
/// </remarks>
internal enum StandardDevice
{
    /// <summary>
    /// The standard input device (<c>stdin</c>).
    /// </summary>
    STD_INPUT_HANDLE = -10,

    /// <summary>
    /// The standard output device (<c>stdout</c>).
    /// </summary>
    STD_OUTPUT_HANDLE = -11,

    /// <summary>
    /// The standard error device (<c>stderr</c>).
    /// </summary>
    STD_ERROR_HANDLE = -12
}
