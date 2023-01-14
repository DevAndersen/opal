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
            stringBuilder
                .Clear()
                .AppendEscapeBracket()
                .AppendReset()
                .AppendSGREnding()
                .AppendEscapeBracket()
                .AppendSetCursorPosition(consoleHandler.BufferWidthOffset, consoleHandler.BufferHeightOffset)
                .Append('H');

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
                if (slice.Length > 0)
                {
                    AppendNew(slice, previousConsoleChar);
                }

                int x = (start + end) / consoleHandler.Width;
                if (end != 0 && (start + end) % consoleHandler.Width == 0 && x < consoleHandler.Height)
                {
                    stringBuilder
                        .AppendEscapeBracket()
                        .AppendSetCursorPosition(consoleHandler.BufferWidthOffset, consoleHandler.BufferHeightOffset + x)
                        .Append('H');
                }

                start += end;
                previousConsoleChar = grid.Grid.Span[start - 1];
            }

            stringBuilder
                .AppendEscapeBracket()
                .AppendReset()
                .AppendSGREnding();

            consoleHandler.Print(stringBuilder);

            if (stringBuilder.Length > sbCap)
            {
                sbCap = (int)(stringBuilder.Length * 1.1F);
                stringBuilder.Capacity = sbCap;
            }
        }
    }

    private void AppendNew(Span<ConsoleChar> consoleChars, ConsoleChar previousConsoleChar)
    {
        ConsoleChar consoleChar = consoleChars[0];
        firstEdit = true;

        // Foreground
        if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet))
        {
            if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundRgb))
            {
                if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == false || consoleChar.ForegroundRgb != previousConsoleChar.ForegroundRgb)
                {
                    stringBuilder
                        .AppendStart(ref firstEdit)
                        .AppendForegroundRgb(consoleChar.ForegroundRed, consoleChar.ForegroundGreen, consoleChar.ForegroundBlue);
                }
            }
            else if (consoleChar.ForegroundSimple != previousConsoleChar.ForegroundSimple)
            {
                stringBuilder.AppendStart(ref firstEdit)
                    .AppendForegroundSimple(consoleChar.ForegroundSimple);
            }
        }
        else if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == true)
        {
            stringBuilder.AppendStart(ref firstEdit)
                .AppendResetForeground();
        }

        // Background
        if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet))
        {
            if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundRgb))
            {
                if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == false || consoleChar.BackgroundRgb != previousConsoleChar.BackgroundRgb)
                {
                    stringBuilder
                        .AppendStart(ref firstEdit)
                        .AppendBackgroundRgb(consoleChar.BackgroundRed, consoleChar.BackgroundGreen, consoleChar.BackgroundBlue);
                }
            }
            else if (consoleChar.BackgroundSimple != previousConsoleChar.BackgroundSimple)
            {
                stringBuilder.AppendStart(ref firstEdit)
                    .AppendBackgroundSimple(consoleChar.BackgroundSimple);
            }
        }
        else if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == true)
        {
            stringBuilder.AppendStart(ref firstEdit)
                .AppendResetBackground();
        }

        // Modes
        foreach (ConsoleCharModes mode in modes)
        {
            StyleApplyMode state = GetModeStylingState(consoleChar, previousConsoleChar, mode);
            if (state != StyleApplyMode.Keep)
            {
                AppendModeStyling(mode, state == StyleApplyMode.Enable);
                firstEdit = false;
            }
        }

        if (!firstEdit)
        {
            stringBuilder.AppendSGREnding();
        }

        foreach (ConsoleChar item in consoleChars)
        {
            if (item.Character == default)
            {
                stringBuilder.Append(' ');
            }
            else
            {
                stringBuilder.Append(item.Character);
            }
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

    private StringBuilder AppendModeStyling(ConsoleCharModes mode, bool state)
    {
        stringBuilder.AppendStart(ref firstEdit);
        return mode switch
        {
            ConsoleCharModes.Italic => stringBuilder.AppendItalic(state),
            ConsoleCharModes.Underscore => stringBuilder.AppendUnderscore(state),
            ConsoleCharModes.DoubleUnderscore => stringBuilder.AppendDoubleUnderscore(state),
            ConsoleCharModes.Strike => stringBuilder.AppendStrike(state),
            ConsoleCharModes.Blinking => stringBuilder.AppendBlinking(state),
            _ => stringBuilder
        };
    }

    private enum StyleApplyMode : byte
    {
        Enable,
        Disable,
        Keep
    }
}
