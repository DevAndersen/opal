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

    public IConsoleInput? GetInput()
    {
        PeekConsoleInput(_inputHandle, out INPUT_RECORD peekedRecord, 1, out _);
        if (peekedRecord.EventType == EventType.MOUSE_EVENT)
        {
            ReadConsoleInput(_inputHandle, out INPUT_RECORD record, 1, out _);
            MOUSE_EVENT_RECORD mouseEvent = record.MouseEvent;

            bool isMoveEvent = mouseEvent.dwEventFlags == MouseEventFlag.MOUSE_MOVED;
            MouseInputType inputType = MouseInputType.None;
            switch (mouseEvent.dwEventFlags)
            {
                case MouseEventFlag.None:
                    inputType = mouseEvent.dwButtonState switch
                    {
                        MouseButtonStates.FROM_LEFT_1ST_BUTTON_PRESSED => MouseInputType.LeftButton,
                        MouseButtonStates.FROM_LEFT_2ND_BUTTON_PRESSED => MouseInputType.MiddleButton,
                        MouseButtonStates.RIGHTMOST_BUTTON_PRESSED => MouseInputType.RightButton,
                        _ => MouseInputType.None
                    };
                    break;

                case MouseEventFlag.MOUSE_WHEELED:
                    if (mouseEvent.dwButtonState > 0)
                    {
                        inputType = MouseInputType.ScrollUp;
                    }
                    else if (mouseEvent.dwButtonState < 0)
                    {
                        inputType = MouseInputType.ScrollDown;
                    }
                    break;
            }

            ConsoleModifiers modifiers = default;
            if (mouseEvent.dwControlKeyState.HasFlag(ControlKeyStates.SHIFT_PRESSED))
            {
                modifiers |= ConsoleModifiers.Shift;
            }
            if (mouseEvent.dwControlKeyState.HasFlag(ControlKeyStates.LEFT_CTRL_PRESSED) || mouseEvent.dwControlKeyState.HasFlag(ControlKeyStates.RIGHT_CTRL_PRESSED))
            {
                modifiers |= ConsoleModifiers.Control;
            }
            if (mouseEvent.dwControlKeyState.HasFlag(ControlKeyStates.LEFT_ALT_PRESSED) || mouseEvent.dwControlKeyState.HasFlag(ControlKeyStates.RIGHT_ALT_PRESSED))
            {
                modifiers |= ConsoleModifiers.Alt;
            }

            if (isMoveEvent)
            {
                return new MouseMoveInput(
                    inputType, // Todo: All pressed buttons.
                    modifiers,
                    mouseEvent.dwMousePosition.X,
                    mouseEvent.dwMousePosition.Y - Console.WindowTop
                );
            }
            else
            {
                return new MouseButtonInput(
                    inputType, // Todo: Only the triggering buttons.
                    inputType, // Todo: All pressed buttons.
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
    }
}
