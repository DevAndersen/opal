using DevAndersen.Opal.Native.Unix;
using DevAndersen.Opal.Rendering;

namespace DevAndersen.Opal.ConsoleHandlers.InputHandlers;

/// <summary>
/// A console input handler for UNIX-like systems.
/// </summary>
internal class UnixInputHandler : IInputHandler
{
    private readonly IConsoleHandler consoleHandler;

    public UnixInputHandler(IConsoleHandler consoleHandler)
    {
        this.consoleHandler = consoleHandler;
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
                if (read.KeyChar == 'm' || read.KeyChar == 'M')
                {
                    break;
                }
                buffer[offset] = read.KeyChar;
                offset++;
            }

            if (buffer[0..2].SequenceEqual("[<"))
            {
                return ReadSequence(buffer[2..]);
            }
        }
        return null;
    }

    public void StartInputListening()
    {
        consoleHandler.Print(SequenceProvider.EnableMouseReporting());
    }

    public void StopInputListening()
    {
        consoleHandler.Print(SequenceProvider.DisableMouseReporting());
    }

    private IConsoleInput? ReadSequence(ReadOnlySpan<char> buffer)
    {
        Span<int> segments = stackalloc int[3];
        int offset = 0;

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

        return CreateMouseInput((XTermMouseInput)segments[0], segments[1], segments[2]);
    }

    private static MouseInput CreateMouseInput(XTermMouseInput mouseEvent, int posX, int posY)
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

        return new MouseInput
        {
            InputType = action,
            Modifiers = modifiers,
            X = posX - 1,
            Y = posY - 1
        };
    }
}
