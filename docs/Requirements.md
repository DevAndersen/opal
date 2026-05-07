# Requirements

Opal's rendering relies on [ANSI escape sequences](https://en.wikipedia.org/wiki/ANSI_escape_code), which means it may not work fully or at all with consoles that do not support the necessary sequences.

For a full list of sequence requirements, see [below](#expected-sequence-support). 

## Windows

On Windows, all the required sequences are supported by both Windows Terminal and Windows Console, and should work on any version of Windows 11 or newer (while unverified, support may go as far back as [Windows 10 Insiders Build #14931](https://devblogs.microsoft.com/commandline/24-bit-color-in-the-windows-console/)).

## macOS

macOS support is currently untested.

## Linux

On Linux, support for these depends on the terminal emulator being used.

Opal should work on most of the popular terminal emulators, and has been verified to work as expected with the following: 

- [Konsole](https://apps.kde.org/konsole/) (26.04.0)
- [GNOME console](https://apps.gnome.org/Console/) (3.60.0)

Note: Opal does not work correctly on the Linux virtual console, as it lacks support for many escape sequences.

## Expected sequence support

Opal expects that the console being used to supports the following escape sequences:

- Resets: `\e[0m`, `\e[39m`, `\e[49m`
- Standard coloring: `\e[{30-37}m`, `\e[{40-47}m`, `\e[{90-97}m`, `\e[{100-107}m`
- 24-bit RGB coloring: `\e[38;2;{r};{g};{b}m`, `\e[48;2;{r};{g};{b}m`
- Italics: `\e[3m`, `\e[23m`
- Underscore: `\e[4m`, `\e[24m`
- Underscore: `\e[21m`, `\e[24m`
- Strike: `\e[9m`, `\e[29m`
- Blinking: `\e[5m`, `\e[25m`
- Cursor positioning: `\m[{x};{y}H`
- Mouse reporting: `\e[?1003h`, `\e[?1006h`, `\e[?1003l`, `\e[?1006l`
- Alternate buffer: `\e[?1049h` `\e[?1049l`
