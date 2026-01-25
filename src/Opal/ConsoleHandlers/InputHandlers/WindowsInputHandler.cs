using Opal.Events;
using Opal.Native.Windows;
using static Opal.Native.Windows.Kernel32;

namespace Opal.ConsoleHandlers.InputHandlers;

/// <summary>
/// A console input handler for Windows systems.
/// </summary>
public class WindowsInputHandler : IInputHandler
{
    private nint _inputHandle;
    private MouseButtons _previousPressedButtons;

    public IConsoleInput? GetInput()
    {
        PeekConsoleInput(_inputHandle, out INPUT_RECORD peekedRecord, 1, out _);
        if (peekedRecord.EventType == EventType.MOUSE_EVENT)
        {
            ReadConsoleInput(_inputHandle, out INPUT_RECORD record, 1, out _);
            MOUSE_EVENT_RECORD mouseEvent = record.MouseEvent;

            bool isMoveEvent = mouseEvent.dwEventFlags == MouseEventFlag.MOUSE_MOVED;
            MouseButtons button = MouseButtons.None;
            ConsoleModifiers modifiers = ConvertModifiers(mouseEvent.dwControlKeyState);

            if (isMoveEvent)
            {
                return new MouseMoveInput(
                    button,
                    modifiers,
                    mouseEvent.dwMousePosition.X,
                    mouseEvent.dwMousePosition.Y - Console.WindowTop
                );
            }
            else
            {
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
        }
        else if (peekedRecord.EventType == EventType.KEY_EVENT && Console.KeyAvailable)
        {
            return new KeyInput(
                Console.ReadKey(true)
            );
        }
        else if (!peekedRecord.Equals(default) && GetNumberOfConsoleInputEvents(_inputHandle, out uint numberOfEvents) && numberOfEvents > 0)
        {
            ReadConsoleInput(_inputHandle, out _, 1, out _);
        }
        return null;
    }

    public void Initialize(nint inputHandle)
    {
        _inputHandle = inputHandle;
    }

    public void StartInputListening()
    {
        ConsoleInputModes mode = ConsoleInputModes.ENABLE_MOUSE_INPUT | ConsoleInputModes.ENABLE_INSERT_MODE | ConsoleInputModes.ENABLE_PROCESSED_INPUT;
        SetConsoleInputMode(_inputHandle, mode);
    }

    public void StopInputListening()
    {
        // Todo: Restore previous console mode, so it works as normal after Opal is stopped.
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

    private static MouseButtons ConvertButton(MouseButtonStates nput)
    {
        MouseButtons result = MouseButtons.None;

        if (nput.HasFlag(MouseButtonStates.FROM_LEFT_1ST_BUTTON_PRESSED))
        {
            result |= MouseButtons.LeftButton;
        }

        if (nput.HasFlag(MouseButtonStates.FROM_LEFT_2ND_BUTTON_PRESSED))
        {
            result |= MouseButtons.MiddleButton;
        }

        if (nput.HasFlag(MouseButtonStates.RIGHTMOST_BUTTON_PRESSED))
        {
            result |= MouseButtons.RightButton;
        }

        return result;
    }
}
