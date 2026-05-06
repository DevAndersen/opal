using Opal.Forms.Controls;

namespace Opal.Forms;

public static class FormsExtensions
{
    // Single-parent extension methods.
    extension(IControlSingleParent singleParent)
    {
        /// <summary>
        /// Enumerates over all nested controls.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IControl> GetNestedControls()
        {
            return GetNestedControls(singleParent.ChildControl);
        }

        /// <summary>
        /// Enumerates over all nested <typeparamref name="TControl"/>s.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TControl> GetNestedControls<TControl>()
        {
            return singleParent.GetNestedControls().OfType<TControl>();
        }

        /// <summary>
        /// Enumerates over all nested controls and areas.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(IControl, Rect)> GetNestedChildControlAreas(int width, int height)
        {
            (IControl, Rect)? controlRect = singleParent.GetChildControlArea(width, height);

            if (controlRect == null)
            {
                return [];
            }

            return GetNestedChildControlAreas(controlRect.Value);
        }

        /// <summary>
        /// Enumerates over all nested <typeparamref name="TControl"/>s and areas.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(TControl, Rect)> GetNestedChildControlAreas<TControl>(int width, int height)
        {
            return singleParent.GetNestedChildControlAreas(width, height).OfType<(TControl, Rect)>();
        }
    }

    // Multi-parent extension methods.
    extension(IControlMultiParent multiParent)
    {
        /// <summary>
        /// Enumerates over all nested controls.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IControl> GetNestedControls()
        {
            return GetNestedControls(multiParent.ChildControls);
        }

        /// <summary>
        /// Enumerates over all nested <typeparamref name="TControl"/>s.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TControl> GetNestedControls<TControl>()
        {
            return multiParent.GetNestedControls().OfType<TControl>();
        }

        /// <summary>
        /// Enumerates over all nested controls and areas.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(IControl, Rect)> GetNestedChildControlAreas(int width, int height)
        {
            return GetNestedChildControlAreas(multiParent.GetChildControlAreas(width, height));
        }

        /// <summary>
        /// Enumerates over all nested <typeparamref name="TControl"/>s and areas.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(TControl, Rect)> GetNestedChildControlAreas<TControl>(int width, int height)
        {
            return multiParent.GetNestedChildControlAreas(width, height).OfType<(TControl, Rect)>();
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

            // Iterate over the children of single parent controls.
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

            // Iterate over the children of multi parent controls.
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

            // Iterate over the children of single parent controls.
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

            // Iterate over the children of multi parent controls.
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
