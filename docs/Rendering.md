# Rendering

Opal's rendering relies on [ANSI escape sequences](https://en.wikipedia.org/wiki/ANSI_escape_code). These are special sequences that the console interprets and uses to modify its state, for example to change the colors it is writing in.

## Expected sequence support

Opal expects that the console being used supports the following escape sequences:

- Resets: `\e[0m`, `\e[39m`, `\e[49m`
- Basic coloring: `\e[{30-37}m`, `\e[{40-47}m`, `\e[{90-97}m`, `\e[{100-107}m`
- 24-bit RGB coloring: `\e[38;2;{r};{g};{b}m`, `\e[48;2;{r};{g};{b}m`
- Italics: `\e[3m`, `\e[23m`
- Underscore: `\e[4m`, `\e[24m`
- Underscore: `\e[21m`, `\e[24m`
- Strike: `\e[9m`, `\e[29m`
- Blinking: `\e[5m`, `\e[25m`
- Cursor positioning: `\m[{x};{y}H`
- Mouse reporting: `\e[?1003h`, `\e[?1006h`, `\e[?1003l`, `\e[?1006l`
- Alternate buffer: `\e[?1049h` `\e[?1049l`

On Windows, these should all be supported as of [Windows 10 Insiders Build #14931](https://devblogs.microsoft.com/commandline/24-bit-color-in-the-windows-console/), both when using the Windows Console and Windows Terminal.

Windows Terminal has sequence processing enabled by default, however that is not the case for Windows Console. In the case of the latter, the console mode needs to be changed to enable the `ENABLE_VIRTUAL_TERMINAL_PROCESSING` flag, which necessitates P/Invoke to Win32's [`SetConsoleMode`](https://learn.microsoft.com/en-us/windows/console/setconsolemode) function. For implementation details, refer to the [`WindowsConsoleHandler`](../src/Opal/ConsoleHandlers/WindowsConsoleHandler.cs) class, as well as [`Kernel32`](../src/Opal/Native/Windows/Kernel32.cs) class

## ConsoleRenderer

The [`ConsoleRenderer`](../src/Opal/Rendering/ConsoleRenderer.cs) class is responsible for rendering a [`ConsoleGrid`](../src/Opal/Rendering/ConsoleGrid.cs) to the console. It does so by going through the grid, line by line, and appends the necessary character and styling for each [`ConsoleChar`](../src/Opal/Rendering/ConsoleChar.cs) to a `StringBuilder`. It will not apply styling to characters if the character before it has the same styling, in order to reduce the amount of data being appended to the `StringBuilder`. Finally, it passed the `StringBuilder` to a platform dependent [`IConsoleHandler`](../src/Opal/ConsoleHandlers/IConsoleHandler.cs) which then writes to the console.
