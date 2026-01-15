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

    public UnixInputHandler(IConsoleHandler consoleHandler)
    {
        _consoleHandler = consoleHandler;
    }

    public IConsoleInput? GetInput()
    {
        ConsoleKeyInfo input = Console.ReadKey(true);
        if (input.KeyChar != '\e')
        {
            return new KeyInput(input);
        }
        else
        {
            const byte bufferSize = 32;
            byte offset = 0;
            Span<char> buffer = stackalloc char[bufferSize];

            while (Console.KeyAvailable && offset < bufferSize)
            {
                ConsoleKeyInfo read = Console.ReadKey(true);
                buffer[offset++] = read.KeyChar;
                if (read.KeyChar is _pressSequenceEnding or _releaseSequenceEnding)
                {
                    break;
                }
            }

            if (buffer.StartsWith(_inputSequencePrefix))
            {
                return ReadSequence(buffer[2..offset]);
            }
        }
        return null;
    }

    public void StartInputListening()
    {
        _consoleHandler.Print(SequenceProvider.EnableMouseReporting());
    }

    public void StopInputListening()
    {
        _consoleHandler.Print(SequenceProvider.DisableMouseReporting());
    }

    private MouseInput ReadSequence(ReadOnlySpan<char> buffer)
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

    private static MouseInput CreateMouseInput(XTermMouseInput mouseEvent, int posX, int posY, bool release)
    {
        const XTermMouseInput mouseButtonMask = XTermMouseInput.LeftButton
            | XTermMouseInput.MiddleButton
            | XTermMouseInput.RightButton
            | XTermMouseInput.ScrollUp
            | XTermMouseInput.ScrollDown;

        MouseInputType action;
        if (mouseEvent.HasFlag(XTermMouseInput.Move))
        {
            action = MouseInputType.Move;
        }
        else if (release)
        {
            action = MouseInputType.None;
        }
        else
        {
            XTermMouseInput maskedButtonEvent = mouseEvent & mouseButtonMask;
            action = maskedButtonEvent switch
            {
                XTermMouseInput.LeftButton => MouseInputType.LeftButton,
                XTermMouseInput.MiddleButton => MouseInputType.MiddleButton,
                XTermMouseInput.RightButton => MouseInputType.RightButton,
                XTermMouseInput.ScrollUp => MouseInputType.ScrollUp,
                XTermMouseInput.ScrollDown => MouseInputType.ScrollDown,
                _ => MouseInputType.None
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

        return new MouseInput(
            action,
            modifiers,
            posX - 1,
            posY - 1
        );
    }
}
