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

### Character definition helper script

The character mappings in `CharLib` were generated based on the definitions found on https://www.unicode.org/charts/nameslist/

To help generate the C# mappings, copy some text from the Unicode charts list into the clipboard, for example:

```
2195	 ↕ 	Up Down Arrow
2196	 ↖ 	North West Arrow
2197	 ↗ 	North East Arrow
2198	 ↘ 	South East Arrow
2199	 ↙ 	South West Arrow
```

Then run:

```powershell
$data = Get-Clipboard
$text = $data | % {
    $splits = $_ -split "`t"
    $fullName = [char]::ToUpper($splits[2].Trim()[0]) + $splits[2].Trim().SubString(1).ToLower()
    $formattedName = [cultureinfo]::InvariantCulture.TextInfo.ToTitleCase($splits[2]).Replace("Box Drawings", "").Replace(" ", "").Trim()
    $name = [regex]::Replace($formattedName, "[^A-z]", "")
    $char = $splits[1].Trim()
    $hex = $splits[0].Trim()
    $dec = [System.Convert]::ToInt32($hex, 16)
"
/// <summary>
/// $fullName (Hex <c>$hex</c>, Dec <c>$dec</c>).
/// </summary>
public const char $name = '$char';"
} ; $text | Set-Clipboard
```

This grabs the text from the clipboard, formats it, and places the formatted C# code back into the clipboard.

Note: This script is not perfect, and the output might need additional sanitizing.
