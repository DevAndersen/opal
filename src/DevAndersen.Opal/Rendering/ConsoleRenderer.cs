using DevAndersen.Opal.ConsoleHandlers;

namespace DevAndersen.Opal.Rendering;

public class ConsoleRenderer
{
    private static readonly ConsoleCharModes[] modes = Enum.GetValues<ConsoleCharModes>();

    private readonly IConsoleHandler consoleHandler;
    private readonly StringBuilder stringBuilder;
    private int sbCap;
    private bool firstEdit;

    public ConsoleRenderer(IConsoleHandler consoleHandler)
    {
        this.consoleHandler = consoleHandler;
        stringBuilder = new StringBuilder();
    }

    public void Render(ConsoleGrid grid)
    {
        lock (this)
        {
            stringBuilder.Clear();

            SequenceProvider.AppendWrapped(stringBuilder, SequenceProvider.Reset());
            SequenceProvider.AppendWrapped(stringBuilder, SequenceProvider.SetCursorPosition(consoleHandler.BufferWidthOffset, consoleHandler.BufferHeightOffset), "H");

            ConsoleChar previousConsoleChar = default;

            int start = 0;
            while (start < grid.Grid.Length)
            {
                int end = 0;

                while (start + end < grid.Grid.Length
                    && grid.Grid.Span[start].HasSameStylingAs(grid.Grid.Span[start + end]) // While chars are all the same style
                    && (end == 0 || (start + end) % consoleHandler.Width != 0)) // While not at the end of a line
                {
                    end++;
                }

                Span<ConsoleChar> slice = grid.Grid.Span.Slice(start, end);
                AppendNew(stringBuilder, slice, previousConsoleChar);

                int x = (start + end) / consoleHandler.Width;
                if (end != 0 && (start + end) % consoleHandler.Width == 0 && x < consoleHandler.Height)
                {
                    SequenceProvider.AppendWrapped(stringBuilder, SequenceProvider.SetCursorPosition(consoleHandler.BufferWidthOffset, consoleHandler.BufferHeightOffset + x), "H");
                    //stringBuilder.Append($"{(char)27}[32m+");
                    //stringBuilder.Append($"{(char)27}[0;0H");
                }
                start += end;
                previousConsoleChar = grid.Grid.Span[start - 1];

                //stringBuilder.Append($"{(char)27}[?25l");
                //SequenceProvider.AppendWrapped(stringBuilder, SequenceProvider.SetCursorPosition(0, 0), "H");
            }

            //SequenceProvider.AppendWrapped(stringBuilder, SequenceProvider.SetCursorPosition(0, 0), "H");
            //SequenceProvider.AppendWrapped(stringBuilder, SequenceProvider.SetCursorPosition(consoleHandler.BufferWidthOffset, consoleHandler.BufferHeightOffset), "H");
            SequenceProvider.AppendWrapped(stringBuilder, SequenceProvider.Reset());
            consoleHandler.Print(stringBuilder);

            if (stringBuilder.Length > sbCap)
            {
                sbCap = (int)(stringBuilder.Length * 1.1F);
                stringBuilder.Capacity = sbCap;
            }
        }
    }

    private void AppendNew(StringBuilder sb, Span<ConsoleChar> consoleChars, ConsoleChar previousConsoleChar)
    {
        ConsoleChar consoleChar = consoleChars[0];
        int start = sb.Length;
        firstEdit = true;

        // Foreground
        if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet))
        {
            if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundRgb))
            {
                if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == false || consoleChar.ForegroundRgb != previousConsoleChar.ForegroundRgb)
                {
                    AppendStyling(sb, SequenceProvider.ForegroundRgb(consoleChar.ForegroundRed, consoleChar.ForegroundGreen, consoleChar.ForegroundBlue));
                }
            }
            else if (consoleChar.ForegroundSimple != previousConsoleChar.ForegroundSimple)
            {
                AppendStyling(sb, SequenceProvider.ForegroundSimple(consoleChar.ForegroundSimple));
            }
        }
        else if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == true)
        {
            AppendStyling(sb, SequenceProvider.ResetForeground());
        }

        // Background
        if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet))
        {
            if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundRgb))
            {
                if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == false || consoleChar.BackgroundRgb != previousConsoleChar.BackgroundRgb)
                {
                    AppendStyling(sb, SequenceProvider.BackgroundRgb(consoleChar.BackgroundRed, consoleChar.BackgroundGreen, consoleChar.BackgroundBlue));
                }
            }
            else if (consoleChar.BackgroundSimple != previousConsoleChar.BackgroundSimple)
            {
                AppendStyling(sb, SequenceProvider.BackgroundSimple(consoleChar.BackgroundSimple));
            }
        }
        else if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == true)
        {
            AppendStyling(sb, SequenceProvider.ResetBackground());
        }

        // Modes
        foreach (ConsoleCharModes mode in modes)
        {
            StyleApplyMode state = GetModeStylingState(consoleChar, previousConsoleChar, mode);
            if (state != StyleApplyMode.Keep)
            {
                AppendModeStyling(sb, mode, state == StyleApplyMode.Enable);
                firstEdit = false;
            }
        }

        if (start != sb.Length)
        {
            sb.Append('m');
        }

        foreach (ConsoleChar item in consoleChars)
        {
            if (item.Character == default)
            {
                sb.Append(' ');
            }
            else
            {
                sb.Append(item.Character);
            }
        }
    }

    private void AppendStyling(StringBuilder sb, string str)
    {
        if (firstEdit)
        {
            sb.Append(SequenceProvider.Escape).Append('[').Append(str);
            firstEdit = false;
        }
        else
        {
            sb.Append(';').Append(str);
        }
    }

    private static StyleApplyMode GetModeStylingState(ConsoleChar consoleChar, ConsoleChar previousConsoleChar, ConsoleCharModes mode)
    {
        bool currentHasFlag = (consoleChar.Modes & mode) == mode;
        bool previousHasFlag = (previousConsoleChar.Modes & mode) == mode;

        if (currentHasFlag == previousHasFlag)
        {
            return StyleApplyMode.Keep;
        }
        else if (currentHasFlag)
        {
            return StyleApplyMode.Enable;
        }
        else
        {
            return StyleApplyMode.Disable;
        }
    }

    private void AppendModeStyling(StringBuilder sb, ConsoleCharModes mode, bool state)
    {
        string value = mode switch
        {
            ConsoleCharModes.Italic => SequenceProvider.Italic(state),
            ConsoleCharModes.Underscore => SequenceProvider.Underscore(state),
            ConsoleCharModes.DoubleUnderscore => SequenceProvider.DoubleUnderscore(state),
            ConsoleCharModes.Strike => SequenceProvider.Strike(state),
            ConsoleCharModes.Blinking => SequenceProvider.Blinking(state),
            _ => string.Empty
        };

        AppendStyling(sb, value);
    }

    private enum StyleApplyMode : byte
    {
        Enable,
        Disable,
        Keep
    }
}
