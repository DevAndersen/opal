# ConsoleGrid

The [`IConsoleGrid`](../src/Opal/Rendering/IConsoleGrid.cs) interface represents the console (or a region of it) as a two-dimensional array of [`ConsoleChar`](../src/Opal/Rendering/ConsoleChar.cs) structs.

On each render call, the current view's render method is invoked and passed a blank console grid, which will subsequently be rendered to the console.

Console grids provides simple out-of-bounds handling. Attempts to access a `ConsoleChar` out of bounds will simply return `default(ConsoleChar)`, and attempting to set/change out of bounds cells will be silently ignored.

## Subgrids

The [`ConsoleGrid`](../src/Opal/Rendering/ConsoleGrid.cs) class represents the entire console, however it is common to only wish to work on a specific part of the console at a time, for example the part that should show a specific component.

For this, the `IConsoleGrid.CreateSubgrid` method can be used. It creates a new `IConsoleGrid`, which represents the requested two-dimensional region of the console, with its coordinates being relative to its position. This way, a subgrid can be drawn onto without needing to know the absolute position of the subgrid (though this information is also available).

Subgrids can also be created from subgrids.

Each cell in a subgrid points directly to the corresponding cell in the original `ConsoleGrid`.

Attempts to access cells outside the subgrid will be ignored, even if such a cell exists in the original `ConsoleGrid`.

Subgrids therefore work somewhat similar to `Span<T>`, in that they provide a view over a subset of a larger whole, and can themselves be cut up into even smaller views. It is however important to note that the provided implementations of `IConsoleGrid` are classes, not ref structs as is the case for `Span<T>`.
