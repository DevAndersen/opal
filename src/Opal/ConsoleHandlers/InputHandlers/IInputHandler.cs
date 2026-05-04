using Opal.Events;

namespace Opal.ConsoleHandlers.InputHandlers;

/// <summary>
/// Defines a type that can retrieve user input.
/// </summary>
public interface IInputHandler
{
    /// <summary>
    /// Returns available user input.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IConsoleInput> GetInput();
}
