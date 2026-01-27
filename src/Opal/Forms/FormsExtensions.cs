using Opal.Forms.Controls;

namespace Opal.Forms;

public static class FormsExtensions
{
    extension(IControlSingleParent singleParent)
    {
        public IEnumerable<IControl> GetNestedControls()
        {
            return GetNestedControls(singleParent.ChildControl);
        }

        public IEnumerable<(IControl, Rect)> GetNestedChildControlAreas(int width, int height)
        {
            (IControl, Rect)? controlRect = singleParent.GetChildControlArea(width, height);

            if (controlRect == null)
            {
                return [];
            }

            return GetNestedChildControlAreas(controlRect.Value);
        }
    }

    extension(IControlMultiParent multiParent)
    {
        public IEnumerable<IControl> GetNestedControls()
        {
            return GetNestedControls(multiParent.ChildControls);
        }

        public IEnumerable<(IControl, Rect)> GetNestedChildControlAreas(int width, int height)
        {
            return GetNestedChildControlAreas(multiParent.GetChildControlAreas(width, height));
        }
    }

    private static IEnumerable<IControl> GetNestedControls(params IEnumerable<IControl?> controls)
    {
        foreach (IControl? control in controls)
        {
            if (control == null)
            {
                yield break;
            }

            yield return control;

            if (control is IControlSingleParent nestedSingleParent)
            {
                foreach (IControl nestedChild in nestedSingleParent.GetNestedControls())
                {
                    if (nestedChild != null)
                    {
                        yield return nestedChild;
                    }
                }
            }

            if (control is IControlMultiParent nestedMultiParent)
            {
                foreach (IControl nestedChild in nestedMultiParent.GetNestedControls())
                {
                    if (nestedChild != null)
                    {
                        yield return nestedChild;
                    }
                }
            }
        }
    }

    private static IEnumerable<(IControl, Rect)> GetNestedChildControlAreas(params IEnumerable<(IControl, Rect)> controlRects)
    {
        foreach ((IControl control, Rect rect) in controlRects)
        {
            if (control == null || rect == default)
            {
                yield break;
            }

            yield return (control, rect);

            if (control is IControlSingleParent nestedSingleParent)
            {
                foreach ((IControl nestedControl, Rect nestedRect) in nestedSingleParent.GetNestedChildControlAreas(rect.Width, rect.Height))
                {
                    if (nestedControl != null && nestedRect != default)
                    {
                        yield return (nestedControl, nestedRect with
                        {
                            PosX = nestedRect.PosX + rect.PosX,
                            PosY = nestedRect.PosY + rect.PosY
                        });
                    }
                }
            }

            if (control is IControlMultiParent nestedMultiParent)
            {
                foreach ((IControl nestedControl, Rect nestedRect) in nestedMultiParent.GetNestedChildControlAreas(rect.Width, rect.Height))
                {
                    if (nestedControl != null && nestedRect != default)
                    {
                        yield return (nestedControl, nestedRect with
                        {
                            PosX = nestedRect.PosX + rect.PosX,
                            PosY = nestedRect.PosY + rect.PosY
                        });
                    }
                }
            }
        }
    }
}
