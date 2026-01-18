using Opal.Drawing;
using Opal.Rendering;
using Opal.Views;

namespace Opal.Forms.Controls;

public class TextBox : SelectableControl, IKeyInputHandler
{
    private int _cursor;
    private int _offset;

    public int Width { get; set; }

    public int Height { get; set; }

    public string Text { get; set; } = string.Empty;

    public void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
    {
        if (keyEvent.Key == ConsoleKey.LeftArrow && _cursor > 0)
        {
            _cursor--;

            if (_offset > 0 && _cursor - _offset <= 0)
            {
                _offset--;
            }
        }
        else if (keyEvent.Key == ConsoleKey.RightArrow && _cursor < Text.Length)
        {
            _cursor++;

            if (_cursor - _offset >= Width - 1)
            {
                _offset++;
            }
        }
        else if (keyEvent.Key == ConsoleKey.Backspace && _cursor > 0)
        {
            ReadOnlySpan<char> span = Text.AsSpan();
            ReadOnlySpan<char> before = span[..(_cursor - 1)];
            ReadOnlySpan<char> after = span[_cursor..];
            Text = before.ToString() + after.ToString();

            _cursor--;

            if (_offset > 0 && _cursor - _offset < Width)
            {
                _offset--;
            }
        }
        else if (keyEvent.Key == ConsoleKey.Delete && _cursor < Text.Length)
        {
            ReadOnlySpan<char> span = Text.AsSpan();
            ReadOnlySpan<char> before = span[.._cursor];
            ReadOnlySpan<char> after = span[(_cursor + 1)..];
            Text = before.ToString() + after.ToString();
        }
        else if (char.IsLetterOrDigit(keyEvent.KeyChar) || char.IsPunctuation(keyEvent.KeyChar) || char.IsWhiteSpace(keyEvent.KeyChar))
        {
            Text = Text.Insert(_cursor, keyEvent.KeyChar.ToString());
            _cursor++;

            if (_cursor - _offset >= Width - 1)
            {
                _offset++;
            }
        }

        keyEvent.Handled = true;
    }

    public override void Render(IConsoleGrid grid)
    {
        // Draw the box.
        grid.DrawBox(PosX, PosY, Width, Height, DrawStyle.StandardDrawStyle, new ConsoleChar
        {
            ForegroundSimple = IsSelected ? ConsoleColor.White : ConsoleColor.DarkGray
        });

        // Draw the text.
        int spanLength = int.Min(Text.Length - _offset, Width - 2);
        ReadOnlySpan<char> span = Text.AsSpan().Slice(_offset, spanLength);
        grid.DrawString(PosX + 1, PosY + 1, span, new ConsoleChar
        {
            ForegroundRgb = 0x9edaff
        });

        // Draw the text cursor.
        int visibleCursorPos = _cursor - _offset;
        if (IsSelected && visibleCursorPos < Width - 2)
        {
            grid[PosX + 1 + visibleCursorPos, PosY + 1] = grid[PosX + 1 + visibleCursorPos, PosY + 1] with
            {
                ForegroundRgb = 0x9edaff,
                Modes = ConsoleCharModes.Underscore
            };
        }
    }
}
