using Opal.Forms.Controls;

namespace Opal.Forms;

public static class FormsExtensions
{
    extension (IControlSingleParent singleParent)
    {
        public IEnumerable<IControl> GetNestedControls()
        {
            return GetNestedControlsRecursively(singleParent.ChildControl);
        }
    }

    extension (IControlMultiParent multiParent)
    {
        public IEnumerable<IControl> GetNestedControls()
        {
            return GetNestedControlsRecursively(multiParent.ChildControls);
        }
    }

    private static IEnumerable<IControl> GetNestedControlsRecursively(params IEnumerable<IControl> controls)
    {
        foreach (IControl control in controls)
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
}
