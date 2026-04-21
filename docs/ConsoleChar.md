# ConsoleChar

The [`ConsoleChar`](../src/Opal/Rendering/ConsoleChar.cs) struct represents a single character in the console, with associated styling.

It can have a foreground and a background, both of which can either be a regular `System.ConsoleColor`, or a 24-bit RGB color.

It can also have text styling, which includes:

- Italic
- Underscore
- Double underscore
- Strike through
- Blinking

Note: While it is possible to specify both underscore and double underscore, console hosts may only choose to render one instead of the other. It is advised that at most one of these is specified.

For implementation details, see [Rendering](./Rendering.md).

## Multi-character glyphs

In addition to being able to display a single `char`, `ConsoleChar` also supports multi-character glyphs, such as emoji or other glyphs that cannot be represented by a single UTF-16 character.

This can be done by using one of the constructors of `ConsoleChar` which accepts a `string` as input, or by setting the `Sequence` property directly.

Under the hood, this stores the string into [`ConsoleCharStringCache`](../src/Opal/Rendering/ConsoleCharStringCache.cs), which acts as a string cache with a 16-bit index. It automatically stores and reuses indexes as appropriate, but will override old entries if it exhausts the 16-bit index space.

This feature is primarily intended for storing short strings, and is only to be used when it is necessary to do so.

Do note that this feature can cause subsequent `ConsoleChar`s to not be rendered. This is because it is expected that console hosts render multi-character glyphs as taking up the same width as the total number of character they consist of. In other words, an emoji which consists of three UTF-16 characters will take up three cells in the console. Opal therefore mimics this behavior, and skips subsequent characters on the same line, as they would otherwise cause other characters to be misaligned. 
