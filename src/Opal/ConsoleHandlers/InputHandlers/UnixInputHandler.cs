using Opal.Events;
using Opal.Native.Unix;
using Opal.Rendering;

namespace Opal.ConsoleHandlers.InputHandlers;

/// <summary>
/// A console input handler for UNIX-like systems.
/// </summary>
public class UnixInputHandler : IInputHandler
{
    private readonly IConsoleHandler _consoleHandler;

    private const char _pressSequenceEnding = 'M';
    private const char _releaseSequenceEnding = 'm';
    private const string _inputSequencePrefix = "[<";

    /// <summary>
    /// Lock object to prevent multiple concurrent attempts at reading console input.
    /// </summary>
    /// <remarks>
    /// This is a <see cref="object"/> instead of a <see cref="Lock"/> due to limitations with the latter relating to yield boundaries.
    /// </remarks>
    private readonly object _lockObject = new object();

    /// <summary>
    /// Buffer for storing read input when reading input mouse input sequences.
    /// </summary>
    /// <remarks>
    /// This buffer only needs to be big enough to store the mouse coordinates and the sequence prefix and suffix.
    /// 32 characters should be more than enough.
    /// </remarks>
    private readonly char[] _buffer = new char[32];

    public UnixInputHandler(IConsoleHandler consoleHandler)
    {
        _consoleHandler = consoleHandler;
    }

    public IEnumerable<IConsoleInput> GetInput()
    {
        lock (_lockObject)
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo input = Console.ReadKey(true);

                if (input.KeyChar != '\e')
                {
                    yield return new KeyInput(input);
                }
                else
                {
                    byte offset = 0;

                    while (Console.KeyAvailable && offset < _buffer.Length)
                    {
                        ConsoleKeyInfo read = Console.ReadKey(true);
                        _buffer[offset++] = read.KeyChar;
                        if (read.KeyChar is _pressSequenceEnding or _releaseSequenceEnding)
                        {
                            break;
                        }
                    }

                    if (_buffer.StartsWith(_inputSequencePrefix))
                    {
                        yield return ReadSequence(_buffer.AsSpan()[2..offset]);
                    }
                }
            }
        }
    }

    public void StartInputListening()
    {
        _consoleHandler.Print(SequenceProvider.EnableMouseReporting());
    }

    public void StopInputListening()
    {
        _consoleHandler.Print(SequenceProvider.DisableMouseReporting());
    }

    private static IConsoleInput ReadSequence(ReadOnlySpan<char> buffer)
    {
        Span<int> segments = stackalloc int[3];
        int offset = 0;
        bool release = buffer[^1] == _releaseSequenceEnding;

        for (int i = 0; i < 3; i++)
        {
            while (char.IsDigit(buffer[offset]))
            {
                offset++;
            }

            segments[i] = int.TryParse(buffer[..offset], out int parsedInt) ? parsedInt : 0;
            buffer = buffer[(offset + 1)..];
            offset = 0;
        }

        return CreateMouseInput((XTermMouseInput)segments[0], segments[1], segments[2], release);
    }

    private static IConsoleInput CreateMouseInput(XTermMouseInput mouseEvent, int posX, int posY, bool release)
    {
        const XTermMouseInput mouseButtonMask = XTermMouseInput.LeftButton
            | XTermMouseInput.MiddleButton
            | XTermMouseInput.RightButton
            | XTermMouseInput.ScrollUp
            | XTermMouseInput.ScrollDown;

        MouseButtons button = MouseButtons.None;
        bool isMoveEvent = mouseEvent.HasFlag(XTermMouseInput.Move);

        if (!release)
        {
            XTermMouseInput maskedButtonEvent = mouseEvent & mouseButtonMask;
            button = maskedButtonEvent switch
            {
                XTermMouseInput.LeftButton => MouseButtons.LeftButton,
                XTermMouseInput.MiddleButton => MouseButtons.MiddleButton,
                XTermMouseInput.RightButton => MouseButtons.RightButton,
                XTermMouseInput.ScrollUp => MouseButtons.ScrollUp,
                XTermMouseInput.ScrollDown => MouseButtons.ScrollDown,
                _ => MouseButtons.None
            };
        }

        ConsoleModifiers modifiers = default;
        if (mouseEvent.HasFlag(XTermMouseInput.Control))
        {
            modifiers |= ConsoleModifiers.Control;
        }

        if (mouseEvent.HasFlag(XTermMouseInput.Shift))
        {
            modifiers |= ConsoleModifiers.Shift;
        }

        if (mouseEvent.HasFlag(XTermMouseInput.Alt))
        {
            modifiers |= ConsoleModifiers.Alt;
        }

        if (isMoveEvent)
        {
            return new MouseMoveInput(
                button, // Todo: All pressed buttons.
                modifiers,
                posX - 1,
                posY - 1
            );
        }
        else
        {
            return new MouseButtonInput(
                button, // Todo: Only the triggering buttons.
                false, // Todo: Is pressed.
                button, // Todo: All pressed buttons.
                modifiers,
                posX - 1,
                posY - 1
            );
        }
    }
}
