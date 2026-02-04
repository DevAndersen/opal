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

form.ChildControls.Add(new GroupBox
    {
        PosX = 80,
        PosY = 2,
        Width = 20,
        Height = 8,
        Text = "GroupBox",
        ChildControl = new Button
        {
            Text = "Btn",
            BorderColor = ConsoleColor.DarkRed,
            BorderColorHighlight = ConsoleColor.Red,
            OnClick = new(async cancellationToken =>
            {
                throw new Exception("This is a test");
            })
        }
    });

form.ChildControls.Add(new DragBox
    {
        PosX = 30,
        PosY = 0,
    });

OpalManager manager = new OpalManager();
await manager.StartAsync(form);

public class TestForm : ConsoleForm
{
    private readonly LinkedList<IConsoleInput> _inputs = [];

    public override async Task HandleKeyInputAsync(KeyInput keyEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        await base.HandleKeyInputAsync(keyEvent, consoleState, cancellationToken);
        if (keyEvent.Handled)
        {
            return;
        }

        AddInput(keyEvent);
    }

    public override async Task HandleMouseButtonInputAsync(MouseButtonInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        await base.HandleMouseButtonInputAsync(mouseEvent, consoleState, cancellationToken);
        if (mouseEvent.Handled)
        {
            return;
        }

        AddInput(mouseEvent);
    }

    public override async Task HandleMouseMoveInputAsync(MouseMoveInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        await base.HandleMouseMoveInputAsync(mouseEvent, consoleState, cancellationToken);
        if (mouseEvent.Handled)
        {
            return;
        }

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
        return false;
    }
}

public class DragBox : IControl, IDragControl
{
    public int PosX { get; set; }

    public int PosY { get; set; }

    public ConsoleEventHandler<DragInput> OnDragStart { get; }

    public ConsoleEventHandler<DragInput> OnDragMove { get; }

    public ConsoleEventHandler<DragInput> OnDragStop { get; }

    private bool _isDragged;

    public DragBox()
    {
        OnDragMove += Move;
        OnDragStart += _ => _isDragged = true;
        OnDragStop += _ => _isDragged = false;
    }

    public Rect GetDesiredSize(int width, int height)
    {
        return new Rect(PosX, PosY, 9, 3);
    }

    public void Render(IConsoleGrid grid)
    {
        grid.DrawBox(0, 0, grid.Width, grid.Height, new ConsoleChar { ForegroundColor = _isDragged ? ConsoleColor.Green : default });
        grid.DrawString(1, 1, "Drag me");
    }

    private void Move(DragInput input)
    {
        PosX = input.X - input.DragStartX;
        PosY = input.Y - input.DragStartY;
    }
}
