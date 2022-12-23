namespace DevAndersen.Opal.Views;

public interface IKeyboardInputHandler
{
    public void HandleKeyInput(ConsoleKeyEvent keyEvent);

    public bool AcceptsKeyboardInput() => true;
}
