using Opal.Rendering;
using Opal.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Opal.Forms;

public class ConsoleForm : ConsoleView, IControlMultiParent, IKeyInputHandler, IMouseInputHandler, IDisposable
{
    private readonly ObservableCollection<IControl> _controls = [];

    public IList<IControl> Controls => _controls;

    public override void Initialize()
    {
        _controls.CollectionChanged += OnControlsChange;
    }

    public virtual void HandleKeyInput(KeyInput keyEvent, IConsoleState consoleState)
    {
        if (keyEvent.Key == ConsoleKey.Tab)
        {
            if (keyEvent.Modifiers == ConsoleModifiers.Shift)
            {
                SelectPrevious();
                keyEvent.Handled = true;
            }
            else
            {
                SelectNext();
                keyEvent.Handled = true;
            }
        }
    }

    public virtual void HandleMouseInput(MouseInput mouseEvent, IConsoleState consoleState)
    {
    }

    public virtual bool SelectControl(IControl control)
    {
        // Todo: Select the specified control.
        return false;
    }

    public virtual bool SelectNext()
    {
        // Todo: Select the next control.
        return false;
    }

    public virtual bool SelectPrevious()
    {
        // Todo: Select the previous control.
        return false;
    }

    public override void Render(IConsoleGrid grid)
    {
        foreach (IControl control in Controls)
        {
            IConsoleGrid controlSubgrid = grid.CreateSubgrid(control.PosX, control.PosY, 100, 100); // Todo
            control.Render(controlSubgrid);
        }
    }

    private void OnControlsChange(object? sender, NotifyCollectionChangedEventArgs args)
    {
        // Todo: Change selected control if the current control is no longer contained in the form (or its children).
    }

    public virtual void Dispose()
    {
        _controls.CollectionChanged -= OnControlsChange;
        GC.SuppressFinalize(this);
    }
}
