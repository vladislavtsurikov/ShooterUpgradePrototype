using UnityEngine.UIElements;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public interface IBindableUIElement
    {
        string BindingId { get; }
        VisualElement Element { get; }
    }
}
