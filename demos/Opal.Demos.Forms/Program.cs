using Opal;
using Opal.Drawing;
using Opal.Forms;
using Opal.Forms.Controls;
using Opal.Rendering;
using Opal.Views;

TestForm form = new TestForm();

form.Controls.Add(
    new Button
    {
        PosX = 5,
        PosY = 3,
        Height = 3,
        Width = 10,
        Text = "Button 1"
    });

form.Controls.Add(
    new Button
    {
        PosX = 5,
        PosY = 6,
        Height = 3,
        Width = 10,
        Text = "Button 2"
    });

form.Controls.Add(
    new Button
    {
        PosX = 5,
        PosY = 9,
        Height = 3,
        Width = 10,
        Text = "Button 3",
        Index = 0
    });

form.Controls.Add(
    new Button
    {
        PosX = 5,
        PosY = 12,
        Height = 3,
        Width = 10,
        Text = "Button 4",
    });

form.Controls.Add(
    new TextBox
    {
        PosX = 5,
        PosY = 15,
        Height = 3,
        Width = 10
    });

form.Controls.Add(
    new Label
    {
        PosX = 2,
        PosY = 1,
        Text = "This is a test"
    });

OpalController controller = new OpalController();
await controller.StartAsync(form);

public class TestForm : ConsoleForm
{
    public override void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
    {
        base.HandleKeyInput(keyEvent, consoleState);
        if (keyEvent.Handled)
        {
            return;
        }

        //Controls.RemoveAt(0);
    }

    public override void Render(IConsoleGrid grid)
    {
        base.Render(grid);
        RenderTextAlignmentTest(grid);
    }

    private static void RenderTextAlignmentTest(IConsoleGrid grid)
    {
        grid.DrawTable(40, 2, 3, 2, 8, 1, (cellGrid, x, y) =>
        {
            string text = y == 0 ? "Hello" : "Hello, World!";
            cellGrid.DrawString(0, 0, cellGrid.Width, text, (HorizontalAlignment)x);
        });
    }
}
