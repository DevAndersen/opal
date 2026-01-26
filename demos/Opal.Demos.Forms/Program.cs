using Opal;
using Opal.Drawing;
using Opal.Events;
using Opal.Forms;
using Opal.Forms.Controls;
using Opal.Rendering;
using Opal.Views;

TestForm form = new TestForm();

form.ChildControls.Add(
    new Button
    {
        PosX = 5,
        PosY = 3,
        Height = 3,
        Width = 10,
        Text = "Button 1"
    });

form.ChildControls.Add(
    new Button
    {
        PosX = 5,
        PosY = 6,
        Height = 3,
        Width = 10,
        Text = "Button 2"
    });

form.ChildControls.Add(
    new Button
    {
        PosX = 5,
        PosY = 9,
        Height = 3,
        Width = 10,
        Text = "Button 3",
        Index = 0
    });

form.ChildControls.Add(
    new Button
    {
        PosX = 5,
        PosY = 12,
        Height = 3,
        Width = 10,
        Text = "Button 4",
    });

form.ChildControls.Add(
    new TextBox
    {
        PosX = 5,
        PosY = 15,
        Height = 3,
        Width = 10
    });

form.ChildControls.Add(
    new Label
    {
        PosX = 2,
        PosY = 1,
        Text = "This is a label"
    });

OpalManager manager = new OpalManager();
await manager.StartAsync(form);

public class TestForm : ConsoleForm
{
    private readonly LinkedList<IConsoleInput> _inputs = [];

    public override void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
    {
        base.HandleKeyInput(keyEvent, consoleState);
        if (keyEvent.Handled)
        {
            return;
        }

        AddInput(keyEvent);
    }

    public override void HandleMouseButtonInput(MouseButtonInput mouseEvent, IConsoleState consoleState)
    {
        AddInput(mouseEvent);
    }

    public override void HandleMouseMoveInput(MouseMoveInput mouseEvent, IConsoleState consoleState)
    {
        AddInput(mouseEvent);
    }

    private void AddInput(IConsoleInput input)
    {
        _inputs.AddFirst(input);
        while (_inputs.Count > 20)
        {
            _inputs.RemoveLast();
        }
    }

    public override void Render(IConsoleGrid grid)
    {
        base.Render(grid);
        RenderTextAlignmentTest(grid);

        int i = 0;
        foreach (IConsoleInput input in _inputs)
        {
            grid.DrawString(30, 10 + i++, input.ToString());
        }
    }

    private static void RenderTextAlignmentTest(IConsoleGrid grid)
    {
        grid.DrawTable(40, 2, 3, 2, 8, 1, (cellGrid, x, y) =>
        {
            string text = y == 0 ? "Hello" : "Hello, World!";
            cellGrid.DrawString(0, 0, cellGrid.Width, text, (HorizontalAlignment)x);
        });
    }

    public override bool PreventCancellationRequest()
    {
        return true;
    }
}
