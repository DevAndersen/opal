using Opal.Rendering;
using Opal.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Opal.Forms;

public class ConsoleForm : ConsoleView, IControlMultiParent, IKeyInputHandler, IMouseInputHandler, IDisposable
{
    private readonly ObservableCollection<IControl> _controls = [];

    public IList<IControl> Controls => _controls;

    public ISelectable? Selected { get; protected set; }

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
        return SelectDirectional(
            (currentIndex, length) => currentIndex == length - 1 ? 0 : currentIndex + 1,
            x => x.FirstOrDefault());
    }

    public virtual bool SelectPrevious()
    {
        return SelectDirectional(
            (currentIndex, length) => currentIndex == 0 ? length - 1 : currentIndex - 1,
            x => x.LastOrDefault());
    }

    private bool SelectDirectional(
        Func<int, int, int> nextIndexFunc,
        Func<IEnumerable<(int Index, ISelectable Item)>, (int Index, ISelectable Item)> initialSelectFunc)
    {
        // Enumerable over all selectables, grouped with their index, ordered by [not null -> specified index -> implicit index].
        IEnumerable<(int Index, ISelectable Item)> selectables = this.GetNestedControls()
            .OfType<ISelectable>()
            .Index()
            .OrderBy(x => x.Item.Index == null)
            .ThenBy(x => x.Item.Index)
            .ThenBy(x => x.Index);

        if (Selected != null)
        {
            (int Index, ISelectable Item)[] selectablesArray = selectables.ToArray();

            // If the only selectable is already selected, do nothing.
            if (selectablesArray is [(_, ISelectable onlySelectable)] && onlySelectable == Selected)
            {
                return false;
            }

            // Cycle to the next selectable.
            int currentIndex = Array.FindIndex(selectablesArray, x => x.Item == Selected);
            int nextIndex = nextIndexFunc(currentIndex, selectablesArray.Length);

            SetSelected(selectablesArray[nextIndex].Item, selectablesArray[currentIndex].Item);

            return true;
        }
        else if (initialSelectFunc(selectables).Item is { } initialSelectable)
        {
            // If there is no current selection, select the initial selectable.
            SetSelected(initialSelectable, null);
            return true;
        }
        return false;
    }

    private void SetSelected(ISelectable newSelected, ISelectable? oldSelected)
    {
        oldSelected?.SelectionChange(false);
        newSelected.SelectionChange(true);
        Selected = newSelected;
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
