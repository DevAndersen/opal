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

    public IList<IControl> ChildControls => _controls;

    public ISelectable? Selected { get; protected set; }

    public override void Initialize()
    {
        _controls.CollectionChanged += OnControlsChange;
    }

    public virtual async Task HandleKeyInputAsync(KeyInput keyEvent, IConsoleState consoleState, CancellationToken cancellationToken)
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
            await selectedKeyInputHandler.HandleKeyInputAsync(keyEvent, consoleState, cancellationToken);
        }
    }

    public virtual async Task HandleMouseButtonInputAsync(MouseButtonInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        foreach ((IControl control, Rect rect) in this.GetNestedChildControlAreas(consoleState.Width, consoleState.Height))
        {
            if (!rect.IsCoordinateWithinRect(mouseEvent.X, mouseEvent.Y))
            {
                continue;
            }

            if (control is IMouseButtonInputHandler mouseButtonInputHandler)
            {
                await mouseButtonInputHandler.HandleMouseButtonInputAsync(mouseEvent, consoleState, cancellationToken);
            }
            
            if (control is IMouseButtonControl mouseButtonControl)
            {
                if (mouseEvent.IsPressed)
                {
                    mouseButtonControl.OnMouseDown?.Invoke(mouseEvent);

                    if (mouseButtonControl.OnMouseDownAsync != null)
                    {
                        await mouseButtonControl.OnMouseDownAsync.Invoke(mouseEvent, cancellationToken);
                    }
                }
                else
                {
                    mouseButtonControl.OnMouseUp?.Invoke(mouseEvent);

                    if (mouseButtonControl.OnMouseUpAsync != null)
                    {
                        await mouseButtonControl.OnMouseUpAsync.Invoke(mouseEvent, cancellationToken);
                    }
                }
            }
        }
    }

    public virtual async Task HandleMouseMoveInputAsync(MouseMoveInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken)
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
        foreach ((IControl control, Rect rect) in GetChildControlAreas(grid.Width, grid.Height))
        {
            IConsoleGrid controlSubgrid = grid.CreateSubgrid(rect.PosX, rect.PosY, rect.Width, rect.Height);
            control.Render(controlSubgrid);
        }
    }

    public virtual IEnumerable<(IControl, Rect)> GetChildControlAreas(int width, int height)
    {
        foreach (IControl control in ChildControls)
        {
            yield return (control, control.GetDesiredSize(width, height));
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
