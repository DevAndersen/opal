using Opal.Events;
using Opal.Forms.Controls;
using Opal.Rendering;
using Opal.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Opal.Forms;

public class ConsoleForm : ConsoleView,
    IControlMultiParent,
    IKeyInputHandler,
    IMouseButtonInputHandler,
    IMouseMoveInputHandler,
    ICancellationRequestHandler,
    IDisposable
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
            }
            else
            {
                SelectNext();
            }
            keyEvent.Handled = true;
        }
        else if (Selected is IKeyInputHandler selectedKeyInputHandler)
        {
            selectedKeyInputHandler.HandleKeyInput(keyEvent, consoleState);
        }
    }

    public virtual void HandleMouseButtonInput(MouseButtonInput mouseEvent, IConsoleState consoleState)
    {
    }

    public virtual void HandleMouseMoveInput(MouseMoveInput mouseEvent, IConsoleState consoleState)
    {
    }

    public void SelectControl(ISelectable newSelected)
    {
        Selected?.SelectionChange(false);
        newSelected.SelectionChange(true);
        Selected = newSelected;
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

            SelectControl(selectablesArray[nextIndex].Item);

            return true;
        }
        else if (initialSelectFunc(selectables).Item is { } initialSelectable)
        {
            // If there is no current selection, select the initial selectable.
            SelectControl(initialSelectable);
            return true;
        }
        return false;
    }

    public override void Render(IConsoleGrid grid)
    {
        foreach (IControl control in Controls)
        {
            Rect rect = control.GetDesiredSize(grid);
            IConsoleGrid controlSubgrid = grid.CreateSubgrid(rect.PosX, rect.PosY, rect.Width, rect.Height);
            control.Render(controlSubgrid);
        }
    }

    private void OnControlsChange(object? sender, NotifyCollectionChangedEventArgs args)
    {
        // Todo: Change selected control if the current control is no longer contained in the form (or its children).
    }

    public virtual bool PreventCancellationRequest()
    {
        if (Selected is ICancellationRequestHandler cancellationRequestHandler)
        {
            return cancellationRequestHandler.PreventCancellationRequest();
        }

        return false;
    }

    public virtual void Dispose()
    {
        _controls.CollectionChanged -= OnControlsChange;

        foreach (IControl control in this.GetNestedControls())
        {
            if (control is IDisposable disposableControl)
            {
                disposableControl.Dispose();
            }
        }

        GC.SuppressFinalize(this);
    }
}
