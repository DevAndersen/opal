# Rendering

Opal's rendering relies on [ANSI escape sequences](https://en.wikipedia.org/wiki/ANSI_escape_code). These are special sequences that the console interprets and uses to modify its state, for example to change the colors it is writing in.

The [`ConsoleRenderer`](../src/Opal/Rendering/ConsoleRenderer.cs) class is responsible for rendering a [`ConsoleGrid`](../src/Opal/Rendering/ConsoleGrid.cs) to the console. It does so by going through the grid, line by line, and appends the necessary character and styling for each [`ConsoleChar`](../src/Opal/Rendering/ConsoleChar.cs) to a `StringBuilder`. It will not apply styling to characters if the character before it has the same styling, in order to reduce the amount of data being appended to the `StringBuilder`. Finally, it passed the `StringBuilder` to a platform dependent [`IConsoleHandler`](../src/Opal/ConsoleHandlers/IConsoleHandler.cs) which then writes to the console.
