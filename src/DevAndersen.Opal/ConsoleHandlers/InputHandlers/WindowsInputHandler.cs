using DevAndersen.Opal.Native.Windows;
using static DevAndersen.Opal.Native.Windows.Kernel32;

namespace DevAndersen.Opal.ConsoleHandlers.InputHandlers;

/// <summary>
/// A console input handler for Windows systems.
/// </summary>
internal class WindowsInputHandler : IInputHandler
{
    private nint inputHandle;

    public IConsoleInput? GetInput()
    {
        PeekConsoleInput(inputHandle, out INPUT_RECORD peekedRecord, 1, out _);
        if (peekedRecord.EventType == EventType.MOUSE_EVENT)
        {
            ReadConsoleInput(inputHandle, out INPUT_RECORD record, 1, out _);
            MOUSE_EVENT_RECORD mouseEvent = record.MouseEvent;

            MouseInputType inputType = MouseInputType.None;
            switch (mouseEvent.dwEventFlags)
            {
                case MouseEventFlag.None:
                    inputType = mouseEvent.dwButtonState switch
                    {
                        MouseButtonState.FROM_LEFT_1ST_BUTTON_PRESSED => MouseInputType.LeftButton,
                        MouseButtonState.FROM_LEFT_2ND_BUTTON_PRESSED => MouseInputType.MiddleButton,
                        MouseButtonState.RIGHTMOST_BUTTON_PRESSED => MouseInputType.RightButton,
                        _ => MouseInputType.None
                    };
                    break;

                case MouseEventFlag.MOUSE_MOVED:
                    inputType = MouseInputType.Move;
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

            return new MouseInput
            {
                InputType = inputType,
                Modifiers = modifiers,
                X = mouseEvent.dwMousePosition.X,
                Y = mouseEvent.dwMousePosition.Y - Console.WindowTop
            };
        }
        else if (peekedRecord.EventType == EventType.KEY_EVENT && Console.KeyAvailable)
        {
            return new KeyInput
            {
                KeyInfo = Console.ReadKey(true)
            };
        }
        else if (!peekedRecord.Equals(default(INPUT_RECORD)) && GetNumberOfConsoleInputEvents(inputHandle, out uint numberOfEvents) && numberOfEvents > 0)
        {
            ReadConsoleInput(inputHandle, out _, 1, out _);
        }
        return null;
    }

    public void Initialize(nint inputHandle)
    {
        this.inputHandle = inputHandle;
    }

    public void StartInputListening()
    {
        ConsoleInputModes mode = ConsoleInputModes.ENABLE_MOUSE_INPUT | ConsoleInputModes.ENABLE_INSERT_MODE | ConsoleInputModes.ENABLE_PROCESSED_INPUT;
        SetConsoleInputMode(inputHandle, mode);
    }

    public void StopInputListening()
    {
    }
}
