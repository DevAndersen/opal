using DevAndersen.Opal.ConsoleHandlers;

namespace DevAndersen.Opal.Rendering;

public class ConsoleRenderer
{
    private static readonly ConsoleCharModes[] modes = Enum.GetValues<ConsoleCharModes>();

    private readonly IConsoleHandler consoleHandler;
    private readonly StringBuilder stringBuilder;
    private int sbCap;
    private int charsToSkip;
    private bool firstEdit;
    private object lockObject = new object();

    public ConsoleRenderer(IConsoleHandler consoleHandler)
    {
        this.consoleHandler = consoleHandler;
        stringBuilder = new StringBuilder();
    }

    public void Render(ConsoleGrid grid)
    {
        lock (lockObject)
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
            int end = 0;
            while (start < grid.Grid.Length)
            {
                end = 0;

                while (CanCharsBeGroupedTogether(grid, start, start + end))
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
                    charsToSkip = 0;
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

    /// <summary>
    /// Asserts if the <c><see cref="ConsoleChar"/></c>s at <c><paramref name="startPosition"/></c> and <c><paramref name="currentPosition"/></c> should be grouped together.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="startPosition">The index of the start character.</param>
    /// <param name="currentPosition">The index of the current character.</param>
    /// <returns></returns>
    private bool CanCharsBeGroupedTogether(ConsoleGrid grid, int startPosition, int currentPosition)
    {
        return currentPosition < grid.Grid.Length // Is the position outside of the bounds of the grid?
            && grid.Grid.Span[startPosition].HasSameStylingAs(grid.Grid.Span[currentPosition]) // Do the chars have the same mode?
            && (currentPosition == startPosition || currentPosition % consoleHandler.Width != 0); // Has the end of the line been reached?
    }

    /// <summary>
    /// Appends <paramref name="consoleChars"/> to the <c>StringBuilder</c>.
    /// </summary>
    /// <param name="consoleChars"></param>
    /// <param name="previousConsoleChar"></param>
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
            ModeApplyMode state = GetModeStylingState(consoleChar, previousConsoleChar, mode);
            if (state != ModeApplyMode.Keep)
            {
                AppendModeSequence(mode, state == ModeApplyMode.Enable);
                firstEdit = false;
            }
        }

        if (!firstEdit)
        {
            stringBuilder.AppendSGREnding();
        }

        foreach (ConsoleChar item in consoleChars)
        {
            if (charsToSkip > 1)
            {
                charsToSkip--;
                continue;
            }

            if (item.RenderAsString())
            {
                charsToSkip = ConsoleCharStringCache.AppendFromCache(stringBuilder, item.Character);
            }
            else
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
    }

    /// <summary>
    /// Compares the state of <paramref name="mode"/> on <paramref name="consoleChar"/> and <paramref name="previousConsoleChar"/>, returning a <see cref="ModeApplyMode"/>.
    /// </summary>
    /// <param name="consoleChar">The current console character.</param>
    /// <param name="previousConsoleChar">The previous console character.</param>
    /// <param name="mode">The mode to be compared.</param>
    /// <returns></returns>
    private static ModeApplyMode GetModeStylingState(ConsoleChar consoleChar, ConsoleChar previousConsoleChar, ConsoleCharModes mode)
    {
        bool currentHasFlag = (consoleChar.Modes & mode) == mode;
        bool previousHasFlag = (previousConsoleChar.Modes & mode) == mode;

        if (currentHasFlag == previousHasFlag)
        {
            return ModeApplyMode.Keep;
        }
        else if (currentHasFlag)
        {
            return ModeApplyMode.Enable;
        }
        else
        {
            return ModeApplyMode.Disable;
        }
    }

    /// <summary>
    /// Appends the sequence that enables <paramref name="mode"/> if <paramref name="state"/> is <c>true</c>, or disable it if<paramref name="state"/> is <c>false</c>, to the <c>StringBuilder</c>.
    /// </summary>
    /// <param name="mode">The mode to be applied.</param>
    /// <param name="state"><c>true</c> enables the mode, <c>false</c> disables the mode.</param>
    /// <returns></returns>
    private StringBuilder AppendModeSequence(ConsoleCharModes mode, bool state)
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

    private enum ModeApplyMode : byte
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        Keep,

        /// <summary>
        /// Enable the mode.
        /// </summary>
        Enable,

        /// <summary>
        /// Disable the mode.
        /// </summary>
        Disable
    }
}
