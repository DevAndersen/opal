using Opal.Events;

namespace Opal.ConsoleHandlers.InputHandlers;

/// <summary>
/// Defines a type that can retrieve user input.
/// </summary>
public interface IInputHandler
{
    /// <summary>
    /// Returns a user input. If the returned value is <c>null</c>, it represents a lack of new user input.
    /// </summary>
    /// <returns></returns>
    IConsoleInput? GetInput();

    /// <summary>
    /// Modifies the console state in order to set it up to listen for user input.
    /// </summary>
    void StartInputListening();

    /// <summary>
    /// Undoes the modifications to the console state that <see cref="StartInputListening"/> applied.
    /// </summary>
    void StopInputListening();
}
