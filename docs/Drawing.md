# Drawing

Opal comes with a number of utility classes which provide general purpose drawing functionality.

## Drawing helper

The [`DrawingHelper`](../src/Opal/Drawing/DrawingHelper.cs) class contain methods which provide general purpose drawing functionality, such as drawing lines and squares.

These methods are provided as extension methods to [`IConsoleGrid`](../src/Opal/Rendering/IConsoleGrid.cs).

These methods often provide have a `ConsoleChar template` parameter, used to define the basis for how the drawing should be styled, for example in order to set the fore- or background color.

## Drawing styles

[`DrawStyle`](../src/Opal/Drawing/DrawStyle.cs) is a record type which defines a number of general purpose character mappings for drawing boxes and lines.

The `DrawStyle` class also provides several static pre-defined styles.

## Character library

The [`CharLib`](../src/Opal/Drawing/CharLib.cs) class (and its nested classes) contains a large number of mapped Unicode characters, such as box- and block drawing characters, arrows, and general shapes.
