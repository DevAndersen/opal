using Opal;
using Opal.Forms;
using Opal.Forms.Controls;

TestForm form = new TestForm();

form.Controls.Add(
    new Button
    {
        PosX = 5,
        PosY = 3,
        Height = 2,
        Width = 10,
        Text = "Asdf"
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
}
