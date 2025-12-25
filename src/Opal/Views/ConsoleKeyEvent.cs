namespace Opal.Views;

public readonly struct ConsoleKeyEvent
{
    public ConsoleKeyInfo KeyInfo { get; }

    public readonly ConsoleKey Key => KeyInfo.Key;

    public readonly ConsoleModifiers Modifiers => KeyInfo.Modifiers;

    public readonly char KeyChar => KeyInfo.KeyChar;

    public ConsoleKeyEvent(ConsoleKeyInfo keyInfo)
    {
        KeyInfo = keyInfo;
    }
}
