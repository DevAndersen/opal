# Opal

Opal is a console rendering library and TUI (Terminal User Interface) framework, letting you treat a console/terminal as a text-based canvas for rendering and drawing custom views and user interfaces onto.

Opal is designed to be simple to use. In just a few lines of code, you can have full control over each character in the console, including its color and styling.

## Features

### Capabilities

- Full 24-bit RGB color support, plus additional styling.
- Keyboard and mouse input.
- Handles resizing of the console window.

### Views, forms, and controls

- Use views to encapsulate drawing- and update logic.
- Enhance views with event listeners to act on user input.
- Take over the full console window, or run in-line between regular console output, both without overwriting existing console output.
- Use forms, a subset of views, to create event-driven, user interfaces using input controls.
- Or use Opal without views, and simply let it handle the raw rendering work.

### High performance

- Opal is designed to run efficiently, with minimal CPU and memory usage, and without unnecessary heap allocations.

### Compatibility

- Runs on Windows, macOS (untested), and Linux, and will work with most modern console hosts/terminal emulators. See [Requirements](./docs/Requirements.md) for technical details.
- Native AOT compatible.
- Works well with [dependency injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/basics). See the [dependency injection demo](./demos/Opal.Demos.DependencyInjection/Program.cs) for an example.
- No dependencies other than .NET itself.

## Documentation

For documentation, see the [documentation README](./docs/README.md) and the associated markdown files.

## License

Opal is licensed under the [MIT license](./LICENSE).
