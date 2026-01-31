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
                await SelectPreviousAsync(cancellationToken);
            }
            else
            {
                await SelectNextAsync(cancellationToken);
            }
            keyEvent.Handled = true;
            return;
        }

        if (Selected is IKeyInputHandler selectedKeyInputHandler)
        {
            await selectedKeyInputHandler.HandleKeyInputAsync(keyEvent, consoleState, cancellationToken);
        }

        if (Selected is IKeyControl keyControl)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await keyControl.OnKeyDown.InvokeAsync(keyEvent, cancellationToken);
                }
                catch (Exception e)
                {
                    consoleState.Exit(e);
                }
            }, cancellationToken);
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

            // Create a copy of the event, with coordinates relative to the clicked control.
            MouseButtonInput relativeEvent = mouseEvent with
            {
                X = mouseEvent.X - rect.PosX,
                Y = mouseEvent.Y - rect.PosY
            };

            if (control is IMouseButtonInputHandler mouseButtonInputHandler)
            {
                await mouseButtonInputHandler.HandleMouseButtonInputAsync(relativeEvent, consoleState, cancellationToken);
            }

            if (!relativeEvent.Handled && control is ISelectable selectable)
            {
                await SelectControlAsync(selectable, cancellationToken);
            }

            if (!relativeEvent.Handled && control is IMouseButtonControl mouseButtonControl)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (relativeEvent.IsPressed)
                        {
                            await mouseButtonControl.OnMouseDown.InvokeAsync(relativeEvent, cancellationToken);
                        }
                        else
                        {
                            await mouseButtonControl.OnMouseUp.InvokeAsync(relativeEvent, cancellationToken);
                        }
                    }
                    catch (Exception e)
                    {
                        consoleState.Exit(e);
                    }
                }, cancellationToken);

                relativeEvent.Handled = true;
            }

            // If the relative event has been mark as handled, mark the input event as handled and return.
            if (relativeEvent.Handled)
            {
                mouseEvent.Handled = true;
                return;
            }
        }
    }

    public virtual async Task HandleMouseMoveInputAsync(MouseMoveInput mouseEvent, IConsoleState consoleState, CancellationToken cancellationToken)
    {
        foreach ((IControl control, Rect rect) in this.GetNestedChildControlAreas(consoleState.Width, consoleState.Height))
        {
            bool isHovering = rect.IsCoordinateWithinRect(mouseEvent.X, mouseEvent.Y);

            // Create a copy of the event, with coordinates relative to the hovered control.
            MouseMoveInput relativeEvent = mouseEvent with
            {
                X = mouseEvent.X - rect.PosX,
                Y = mouseEvent.Y - rect.PosY
            };

            if (isHovering && control is IMouseMoveInputHandler mouseMoveInputHandler)
            {
                await mouseMoveInputHandler.HandleMouseMoveInputAsync(relativeEvent, consoleState, cancellationToken);
            }

            if (!relativeEvent.Handled && control is IMouseHoverControl mouseHoverControl)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (isHovering && !mouseHoverControl.IsHovered)
                        {
                            mouseHoverControl.IsHovered = true;
                            await mouseHoverControl.OnMouseEnter.InvokeAsync(relativeEvent, cancellationToken);
                        }
                        else if (!isHovering && mouseHoverControl.IsHovered)
                        {
                            mouseHoverControl.IsHovered = false;
                            await mouseHoverControl.OnMouseLeave.InvokeAsync(relativeEvent, cancellationToken);
                        }
                    }
                    catch (Exception e)
                    {
                        consoleState.Exit(e);
                    }
                }, cancellationToken);
            }

            // If the relative event has been mark as handled, mark the input event as handled and return.
            if (relativeEvent.Handled)
            {
                mouseEvent.Handled = true;
                return;
            }
        }
    }

    public async Task SelectControlAsync(ISelectable newSelected, CancellationToken cancellationToken)
    {
        if (Selected?.OnUnselect != null)
        {
            Selected.IsSelected = false;
            await Selected.OnUnselect.InvokeAsync(cancellationToken);
        }

        newSelected.IsSelected = true;
        await newSelected.OnSelect.InvokeAsync(cancellationToken);

        Selected = newSelected;
    }

    public virtual async Task<bool> SelectNextAsync(CancellationToken cancellationToken)
    {
        return await SelectDirectionalAsync(
            (currentIndex, length) => currentIndex == length - 1 ? 0 : currentIndex + 1,
            x => x.FirstOrDefault(),
            cancellationToken);
    }

    public virtual async Task<bool> SelectPreviousAsync(CancellationToken cancellationToken)
    {
        return await SelectDirectionalAsync(
            (currentIndex, length) => currentIndex == 0 ? length - 1 : currentIndex - 1,
            x => x.LastOrDefault(),
            cancellationToken);
    }

    private async Task<bool> SelectDirectionalAsync(
        Func<int, int, int> nextIndexFunc,
        Func<IEnumerable<(int Index, ISelectable Item)>, (int Index, ISelectable Item)> initialSelectFunc,
        CancellationToken cancellationToken)
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

            await SelectControlAsync(selectablesArray[nextIndex].Item, cancellationToken);

            return true;
        }
        else if (initialSelectFunc(selectables).Item is { } initialSelectable)
        {
            // If there is no current selection, select the initial selectable.
            await SelectControlAsync(initialSelectable, cancellationToken);
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
