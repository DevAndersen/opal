# Views

Opal is primarily intended for view-based rendering. Views effectively treat the console as a canvas that it renders characters onto, with an update loop running in the background, and optional support for keyboard- and mouse input.

A view can be created by simply inheriting from the [`ConsoleView`](../src/Opal/Views/ConsoleView.cs) class. It comes with three basic methods to manage the view:

- `Initialize`: Gets called when the view is used for the first time.
- `Render`: Used to render the view onto the provided [`IConsoleGrid`](../src/Opal/Rendering/IConsoleGrid.cs), which will then be rendered to the console.
- `Update`: Perform non-rendering logic.

Alternatively, the [`AsyncConsoleView`](../src/Opal/Views/AsyncConsoleView.cs) class can be inheriting from instead of `ConsoleView`, which provides methods to `ConsoleView` except the initialize- and update methods can perform async logic and will be waited on. Do note that the `Render` method remains synchronous, as it is only intended to handle rendering and nothing else.

## Rendering

Rendering is called on a loop at a regular interval, or if the console's size is changed by the user.

## Updating

The `Update` and `UpdateAsync` methods ...

The update methods are called with an [`IConsoleState`](../src/Opal/Views/IConsoleState.cs) parameter, which allows the view to request moving to a different view, or to exit Opal entirely.

Note: Rendering is called after the `Update` method is called.

## Input handling

Views can implement various interfaces, which lets them receive and handle user input.

- [`IKeyInputHandler`](../src/Opal/Events/IKeyInputHandler.cs): Keyboard input.
- [`IMouseButtonInputHandler`](../src/Opal/Events/IMouseButtonInputHandler.cs): Mouse button input.
- [`IMouseMoveInputHandler`](../src/Opal/Events/IMouseMoveInputHandler.cs): Mouse movement input.
- [`ICancellationRequestHandler`](../src/Opal/Views/ICancellationRequestHandler.cs): Intercept and optionally cancel user requests to exit Opal via `Ctrl+C`.

Note: Input handling gets executed before the `Update` method is called.
