using DevAndersen.Opal.ConsoleHandlers;

namespace DevAndersen.Opal.Rendering;

public class ConsoleRenderer
{
    private static readonly ConsoleCharModes[] modes = Enum.GetValues<ConsoleCharModes>();

    private readonly IConsoleHandler consoleHandler;
    private readonly StringBuilder stringBuilder;
    private int sbCap;

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
            ConsoleChar? previousConsoleChar = null;

            int start = 0;
            while (start < grid.Grid.Length)
            {
                int end = 0;
                while (start + end < grid.Grid.Length && grid.Grid.Span[start].HasSameStylingAs(grid.Grid.Span[start + end]))
                {
                    end++;
                }
                Span<ConsoleChar> slice = grid.Grid.Span.Slice(start, end);
                AppendNew(stringBuilder, slice, previousConsoleChar);
                start += end;
                previousConsoleChar = grid.Grid.Span[start - 1];
            }

            SequenceProvider.AppendWrapped(stringBuilder, SequenceProvider.SetCursorPosition(0, 0), "H");
            consoleHandler.Print(stringBuilder);

            if (stringBuilder.Length > sbCap)
            {
                sbCap = (int)(stringBuilder.Length * 1.1F);
                stringBuilder.Capacity = sbCap;
            }
        }
    }

    private static void AppendNew(StringBuilder sb, Span<ConsoleChar> consoleChars, ConsoleChar? previousConsoleChar)
    {
        ConsoleChar consoleChar = consoleChars[0];
        int start = sb.Length;
        bool firstEdit = true;

        // Foreground
        if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet))
        {
            if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundRgb))
            {
                if (previousConsoleChar?.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == false || consoleChar.ForegroundRgb != previousConsoleChar?.ForegroundRgb)
                {
                    AppendStyling(sb, SequenceProvider.ForegroundRgb(consoleChar.ForegroundRed, consoleChar.ForegroundGreen, consoleChar.ForegroundBlue), ref firstEdit);
                }
            }
            else if (consoleChar.ForegroundSimple != previousConsoleChar?.ForegroundSimple)
            {
                AppendStyling(sb, SequenceProvider.ForegroundSimple(consoleChar.ForegroundSimple), ref firstEdit);
            }
        }
        else if (previousConsoleChar?.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == true)
        {
            AppendStyling(sb, SequenceProvider.ResetForeground(), ref firstEdit);
        }

        // Background
        if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet))
        {
            if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundRgb))
            {
                if (previousConsoleChar?.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == false || consoleChar.BackgroundRgb != previousConsoleChar?.BackgroundRgb)
                {
                    AppendStyling(sb, SequenceProvider.BackgroundRgb(consoleChar.BackgroundRed, consoleChar.BackgroundGreen, consoleChar.BackgroundBlue), ref firstEdit);
                }
            }
            else if (consoleChar.BackgroundSimple != previousConsoleChar?.BackgroundSimple)
            {
                AppendStyling(sb, SequenceProvider.BackgroundSimple(consoleChar.BackgroundSimple), ref firstEdit);
            }
        }
        else if (previousConsoleChar?.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == true)
        {
            AppendStyling(sb, SequenceProvider.ResetBackground(), ref firstEdit);
        }

        // Modes
        foreach (ConsoleCharModes mode in modes)
        {
            bool? state = GetModeStylingState(consoleChar, previousConsoleChar, mode);
            if (state != null)
            {
                AppendModeStyling(sb, mode, state.Value, ref firstEdit);
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

        //sb.Append(SequenceProvider.Wrap(SequenceProvider.Reset()));
    }

    private static void AppendStyling(StringBuilder sb, string str, ref bool firstEdit)
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

    //[Obsolete]
    //private static void Append(StringBuilder stringBuilder, ConsoleChar conChar, ConsoleChar? previousConChar)
    //{
    //    bool firstEdit = true;

    //    // Foreground
    //    if (conChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet))
    //    {
    //        if (conChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundRgb))
    //        {
    //            if (previousConChar?.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == false || conChar.ForegroundRgb != previousConChar?.ForegroundRgb)
    //            {
    //                AppendStyling(stringBuilder, SequenceProvider.ForegroundRgb(conChar.ForegroundRed, conChar.ForegroundGreen, conChar.ForegroundBlue), ref firstEdit);
    //            }
    //        }
    //        else if (conChar.ForegroundSimple != previousConChar?.ForegroundSimple)
    //        {
    //            AppendStyling(stringBuilder, SequenceProvider.ForegroundSimple(conChar.ForegroundSimple), ref firstEdit);
    //        }
    //    }
    //    else if (previousConChar?.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == true)
    //    {
    //        AppendStyling(stringBuilder, SequenceProvider.ResetForeground(), ref firstEdit);
    //    }

    //    // Background
    //    if (conChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet))
    //    {
    //        if (conChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundRgb))
    //        {
    //            if (previousConChar?.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == false || conChar.BackgroundRgb != previousConChar?.BackgroundRgb)
    //            {
    //                AppendStyling(stringBuilder, SequenceProvider.BackgroundRgb(conChar.BackgroundRed, conChar.BackgroundGreen, conChar.BackgroundBlue), ref firstEdit);
    //            }
    //        }
    //        else if (conChar.BackgroundSimple != previousConChar?.BackgroundSimple)
    //        {
    //            AppendStyling(stringBuilder, SequenceProvider.BackgroundSimple(conChar.BackgroundSimple), ref firstEdit);
    //        }
    //    }
    //    else if (previousConChar?.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == true)
    //    {
    //        AppendStyling(stringBuilder, SequenceProvider.ResetBackground(), ref firstEdit);
    //    }

    //    // Modes
    //    foreach (ConsoleCharModes mode in modes)
    //    {
    //        bool? state = GetModeStylingState(conChar, previousConChar, mode);
    //        if (state != null)
    //        {
    //            AppendModeStyling(stringBuilder, mode, state.Value, ref firstEdit);
    //            firstEdit = false;
    //        }
    //    }

    //    char character = conChar.Character == 0
    //        ? ' '
    //        : conChar.Character;

    //    if (!firstEdit)
    //    {
    //        stringBuilder.Append('m');
    //    }

    //    stringBuilder.Append(character);
    //}

    private static bool? GetModeStylingState(ConsoleChar consoleChar, ConsoleChar? previousConsoleChar, ConsoleCharModes mode)
    {
        bool currentHasFlag = (consoleChar.Modes & mode) == mode;

        if (previousConsoleChar == null)
        {
            return currentHasFlag
                ? true
                : null;
        }
        else
        {
            bool previousHasFlag = (previousConsoleChar?.Modes & mode) == mode;
            return currentHasFlag == previousHasFlag
                ? null
                : currentHasFlag;
        }
    }

    private static void AppendModeStyling(StringBuilder sb, ConsoleCharModes mode, bool state, ref bool firstEdit)
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

        AppendStyling(sb, value, ref firstEdit);
    }
}
