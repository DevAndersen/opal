namespace Opal.Events;

/// <summary>
/// Base interface for types representing a user input.
/// </summary>
public interface IConsoleInput
{
    /// <summary>
    /// Specifies if the event has been handled, and should therefore not be propagated any further.
    /// </summary>
    bool Handled { get; set; }
}
