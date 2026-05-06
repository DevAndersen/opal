using Opal.Events;
using Opal.Native.Windows;
using static Opal.Native.Windows.Kernel32;

namespace Opal.ConsoleHandlers.InputHandlers;

/// <summary>
/// A console input handler for Windows systems.
/// </summary>
public class WindowsInputHandler : IInputHandler
{
    /// <summary>
    /// The handle of the console input device.
    /// </summary>
    private nint _inputHandle;

    /// <summary>
    /// The button states of the previous mouse button event.
    /// Used to keep track of which buttons were pressed, to determine if future mouse events press or release buttons.
    /// </summary>
    private MouseButtons _previousPressedButtons;

    /// <summary>
    /// Buffer for storing peeked input.
    /// </summary>
    private readonly INPUT_RECORD[] _buffer = new INPUT_RECORD[8];

    public void Initialize(nint inputHandle)
    {
        _inputHandle = inputHandle;
    }

    public IEnumerable<IConsoleInput> GetInput()
    {
        PeekConsoleInputArray(_inputHandle, _buffer, (uint)_buffer.Length, out uint eventsRead);

        for (int i = 0; i < eventsRead; i++)
        {
            INPUT_RECORD record = _buffer[i];

            switch (record.EventType)
            {
                // Handle mouse input.
                case EventType.MOUSE_EVENT:
                    yield return HandleMouseInput(record);
                    ReadAndDiscardInput();
                    break;

                // Handle key press input.
                case EventType.KEY_EVENT when record.KeyEvent.bKeyDown == 1 && Console.KeyAvailable:
                    yield return new KeyInput(Console.ReadKey(true));
                    break;

                // Read and discard other kinds of input.
                default:
                    ReadAndDiscardInput();
                    break;
            }
        }
    }

    private IConsoleInput HandleMouseInput(INPUT_RECORD record)
    {
        MOUSE_EVENT_RECORD mouseEvent = record.MouseEvent;

        bool isMoveEvent = mouseEvent.dwEventFlags == MouseEventFlag.MOUSE_MOVED;
        ConsoleModifiers modifiers = ConvertModifiers(mouseEvent.dwControlKeyState);

        if (isMoveEvent)
        {
            return new MouseMoveInput(
                _previousPressedButtons,
                modifiers,
                mouseEvent.dwMousePosition.X,
                mouseEvent.dwMousePosition.Y - Console.WindowTop
            );
        }

        MouseButtons button;
        MouseButtons allPressedButtons = ConvertButton(mouseEvent.dwButtonState);
        bool isPressed = true;

        if (mouseEvent.dwEventFlags == MouseEventFlag.MOUSE_WHEELED)
        {
            button = mouseEvent.dwButtonState > 0
                ? MouseButtons.ScrollUp
                : MouseButtons.ScrollDown;
        }
        else
        {
            MouseButtons pressedButtons = (_previousPressedButtons & allPressedButtons) ^ allPressedButtons;
            MouseButtons releasedButtons = (_previousPressedButtons & allPressedButtons) ^ _previousPressedButtons;

            _previousPressedButtons = allPressedButtons;

            if (pressedButtons == MouseButtons.None)
            {
                isPressed = false;
                button = releasedButtons;
            }
            else
            {
                button = pressedButtons;
            }
        }

        return new MouseButtonInput(
            button,
            isPressed,
            allPressedButtons,
            modifiers,
            mouseEvent.dwMousePosition.X,
            mouseEvent.dwMousePosition.Y - Console.WindowTop
        );
    }

    private void ReadAndDiscardInput()
    {
        ReadConsoleInput(_inputHandle, out _, 1, out _);
    }

    private static ConsoleModifiers ConvertModifiers(ControlKeyStates input)
    {
        ConsoleModifiers result = default;

        if (input.HasFlag(ControlKeyStates.SHIFT_PRESSED))
        {
            result |= ConsoleModifiers.Shift;
        }

        if (input.HasFlag(ControlKeyStates.LEFT_CTRL_PRESSED) || input.HasFlag(ControlKeyStates.RIGHT_CTRL_PRESSED))
        {
            result |= ConsoleModifiers.Control;
        }

        if (input.HasFlag(ControlKeyStates.LEFT_ALT_PRESSED) || input.HasFlag(ControlKeyStates.RIGHT_ALT_PRESSED))
        {
            result |= ConsoleModifiers.Alt;
        }

        return result;
    }

    private static MouseButtons ConvertButton(MouseButtonStates input)
    {
        MouseButtons result = MouseButtons.None;

        if (input.HasFlag(MouseButtonStates.FROM_LEFT_1ST_BUTTON_PRESSED))
        {
            result |= MouseButtons.LeftButton;
        }

        if (input.HasFlag(MouseButtonStates.FROM_LEFT_2ND_BUTTON_PRESSED))
        {
            result |= MouseButtons.MiddleButton;
        }

        if (input.HasFlag(MouseButtonStates.RIGHTMOST_BUTTON_PRESSED))
        {
            result |= MouseButtons.RightButton;
        }

        return result;
    }
}
