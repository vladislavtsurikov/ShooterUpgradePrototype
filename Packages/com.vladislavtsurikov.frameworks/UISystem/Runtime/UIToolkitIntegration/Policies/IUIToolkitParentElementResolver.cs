using UnityEngine.UIElements;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public interface IUIToolkitParentElementResolver
    {
        VisualElement Resolve(UIToolkitUIHandler handler);
    }
}
