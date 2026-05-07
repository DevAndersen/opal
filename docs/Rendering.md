# Rendering

Opal's rendering relies on [ANSI escape sequences](https://en.wikipedia.org/wiki/ANSI_escape_code). These are special sequences that the console interprets and uses to modify its state, for example to change the colors it is writing in.

Note: Windows Terminal has sequence processing enabled by default, however that is not the case for Windows Console. In the case of the latter, the console mode needs to be changed to enable the `ENABLE_VIRTUAL_TERMINAL_PROCESSING` flag, which necessitates P/Invoke to Win32's [`SetConsoleMode`](https://learn.microsoft.com/en-us/windows/console/setconsolemode) function. For implementation details, refer to the [`WindowsConsoleHandler`](../src/Opal/ConsoleHandlers/WindowsConsoleHandler.cs) class, as well as [`Kernel32`](../src/Opal/Native/Windows/Kernel32.cs) class

## ConsoleRenderer

The [`ConsoleRenderer`](../src/Opal/Rendering/ConsoleRenderer.cs) class is responsible for rendering a [`ConsoleGrid`](../src/Opal/Rendering/ConsoleGrid.cs) to the console. It does so by going through the grid, line by line, and appends the necessary character and styling for each [`ConsoleChar`](../src/Opal/Rendering/ConsoleChar.cs) to a `StringBuilder`. It will not apply styling to characters if the character before it has the same styling, in order to reduce the amount of data being appended to the `StringBuilder`. Finally, it passed the `StringBuilder` to a platform dependent [`IConsoleHandler`](../src/Opal/ConsoleHandlers/IConsoleHandler.cs) which then writes to the console.
