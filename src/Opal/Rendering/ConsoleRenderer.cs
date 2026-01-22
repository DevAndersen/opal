using Opal.ConsoleHandlers;

namespace Opal.Rendering;

public class ConsoleRenderer
{
    private static readonly ConsoleCharModes[] _modes = Enum.GetValues<ConsoleCharModes>();

    private readonly IConsoleHandler _consoleHandler;
    private readonly StringBuilder _stringBuilder;
    private readonly Lock _lockObject;
    private int _charsToSkip;
    private bool _firstEdit;

    public ConsoleRenderer(IConsoleHandler consoleHandler)
    {
        _consoleHandler = consoleHandler;
        _stringBuilder = new StringBuilder();
        _lockObject = new Lock();
    }

    public void Render(ConsoleGrid grid)
    {
        lock (_lockObject)
        {
            _stringBuilder
                .Clear()
                .AppendEscapeBracket()
                .AppendReset()
                .AppendSGREnding()
                .AppendEscapeBracket()
                .AppendSetCursorPosition(_consoleHandler.BufferWidthOffset, _consoleHandler.BufferHeightOffset)
                .Append('H');

            ConsoleChar previousConsoleChar = default;
            int start = 0;
            int end = 0;

            while (start < grid.Buffer.Length)
            {
                end = 0;

                while (CanCharsBeGroupedTogether(grid, start, start + end))
                {
                    end++;
                }

                ReadOnlySpan<ConsoleChar> slice = grid.Buffer.Span.Slice(start, end);
                if (slice.Length > 0)
                {
                    AppendNew(slice, previousConsoleChar);
                }

                int x = (start + end) / _consoleHandler.Width;
                if (end != 0 && (start + end) % _consoleHandler.Width == 0 && x < _consoleHandler.Height)
                {
                    _charsToSkip = 0;
                    _stringBuilder
                        .AppendEscapeBracket()
                        .AppendSetCursorPosition(_consoleHandler.BufferWidthOffset, _consoleHandler.BufferHeightOffset + x)
                        .Append('H');
                }

                start += end;
                previousConsoleChar = grid.Buffer.Span[start - 1];
            }

            _stringBuilder
                .AppendEscapeBracket()
                .AppendReset()
                .AppendSGREnding();

            _consoleHandler.Print(_stringBuilder);
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
        return currentPosition < grid.Buffer.Length // Is the position outside of the bounds of the grid?
            && grid.Buffer.Span[startPosition].HasSameStylingAs(grid.Buffer.Span[currentPosition]) // Do the chars have the same mode?
            && (currentPosition == startPosition || currentPosition % _consoleHandler.Width != 0); // Has the end of the line been reached?
    }

    /// <summary>
    /// Appends <paramref name="consoleChars"/> to the <c>StringBuilder</c>.
    /// </summary>
    /// <param name="consoleChars"></param>
    /// <param name="previousConsoleChar"></param>
    private void AppendNew(ReadOnlySpan<ConsoleChar> consoleChars, ConsoleChar previousConsoleChar)
    {
        ConsoleChar consoleChar = consoleChars[0];
        _firstEdit = true;

        // Foreground
        if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet))
        {
            if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundRgb))
            {
                if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == false || consoleChar.ForegroundRgb != previousConsoleChar.ForegroundRgb)
                {
                    _stringBuilder
                        .AppendStart(ref _firstEdit)
                        .AppendForegroundRgb(consoleChar.ForegroundRed, consoleChar.ForegroundGreen, consoleChar.ForegroundBlue);
                }
            }
            else if (consoleChar.ForegroundSimple != previousConsoleChar.ForegroundSimple)
            {
                _stringBuilder.AppendStart(ref _firstEdit)
                    .AppendForegroundSimple(consoleChar.ForegroundSimple);
            }
        }
        else if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.ForegroundSet) == true)
        {
            _stringBuilder.AppendStart(ref _firstEdit)
                .AppendResetForeground();
        }

        // Background
        if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet))
        {
            if (consoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundRgb))
            {
                if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == false || consoleChar.BackgroundRgb != previousConsoleChar.BackgroundRgb)
                {
                    _stringBuilder
                        .AppendStart(ref _firstEdit)
                        .AppendBackgroundRgb(consoleChar.BackgroundRed, consoleChar.BackgroundGreen, consoleChar.BackgroundBlue);
                }
            }
            else if (consoleChar.BackgroundSimple != previousConsoleChar.BackgroundSimple)
            {
                _stringBuilder.AppendStart(ref _firstEdit)
                    .AppendBackgroundSimple(consoleChar.BackgroundSimple);
            }
        }
        else if (previousConsoleChar.Metadata.HasFlag(ConsoleCharMetadata.BackgroundSet) == true)
        {
            _stringBuilder.AppendStart(ref _firstEdit)
                .AppendResetBackground();
        }

        // Modes
        foreach (ConsoleCharModes mode in _modes)
        {
            ModeApplyMode state = GetModeStylingState(consoleChar, previousConsoleChar, mode);
            if (state != ModeApplyMode.Keep)
            {
                AppendModeSequence(mode, state == ModeApplyMode.Enable);
                _firstEdit = false;
            }
        }

        if (!_firstEdit)
        {
            _stringBuilder.AppendSGREnding();
        }

        foreach (ConsoleChar item in consoleChars)
        {
            if (_charsToSkip > 1)
            {
                _charsToSkip--;
                continue;
            }

            if (item.ShouldRenderAsString())
            {
                _charsToSkip = ConsoleCharStringCache.AppendFromCache(_stringBuilder, item.Character);
            }
            else
            {
                if (item.Character == default)
                {
                    _stringBuilder.Append(' ');
                }
                else
                {
                    _stringBuilder.Append(item.Character);
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
        _stringBuilder.AppendStart(ref _firstEdit);
        return mode switch
        {
            ConsoleCharModes.Italic => _stringBuilder.AppendItalic(state),
            ConsoleCharModes.Underscore => _stringBuilder.AppendUnderscore(state),
            ConsoleCharModes.DoubleUnderscore => _stringBuilder.AppendDoubleUnderscore(state),
            ConsoleCharModes.Strike => _stringBuilder.AppendStrike(state),
            ConsoleCharModes.Blinking => _stringBuilder.AppendBlinking(state),
            _ => _stringBuilder
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
