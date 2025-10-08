namespace Opal.Native.Windows;

/// <summary>
/// The standard console devices.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://learn.microsoft.com/en-us/windows/console/getstdhandle"/>
/// </remarks>
internal enum StandardDevice
{
    /// <summary>
    /// The standard input device.
    /// </summary>
    STD_INPUT_HANDLE = -10,

    /// <summary>
    /// The standard output device.
    /// </summary>
    STD_OUTPUT_HANDLE = -11,

    /// <summary>
    /// The standard error device.
    /// </summary>
    STD_ERROR_HANDLE = -12
}
