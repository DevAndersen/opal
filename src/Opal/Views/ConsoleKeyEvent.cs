namespace Opal.Views;

public struct ConsoleKeyEvent
{
    public ConsoleKeyInfo KeyInfo { get; }

    public ConsoleKey Key => KeyInfo.Key;

    public ConsoleModifiers Modifiers => KeyInfo.Modifiers;

    public char KeyChar => KeyInfo.KeyChar;

    public ConsoleKeyEvent(ConsoleKeyInfo keyInfo)
    {
        KeyInfo = keyInfo;
    }
}
