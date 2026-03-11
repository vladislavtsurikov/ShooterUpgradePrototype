using UnityEngine.UIElements;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public interface IUIToolkitRootProvider
    {
        VisualElement RootElement { get; }
    }
}
