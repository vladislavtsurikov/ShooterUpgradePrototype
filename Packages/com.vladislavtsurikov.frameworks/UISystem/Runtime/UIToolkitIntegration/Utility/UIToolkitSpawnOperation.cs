#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class UIToolkitSpawnOperation
    {
        private string _name;
        private VisualElement _parent;
        private bool _visible;

        public UIToolkitSpawnOperation Visible(bool visible)
        {
            _visible = visible;
            return this;
        }

        public UIToolkitSpawnOperation WithParent(VisualElement parent)
        {
            _parent = parent;
            return this;
        }

        public UIToolkitSpawnOperation WithName(string name)
        {
            _name = name;
            return this;
        }

        public async UniTask<VisualElement> Execute(
            UIToolkitLayoutLoader layoutLoader,
            UIToolkitElementBinder elementBinder,
            CancellationToken cancellationToken)
        {
            VisualTreeAsset layout = await layoutLoader.LoadLayoutIfNotLoaded(cancellationToken);
            if (layout == null)
            {
                Debug.LogError(
                    $"[UIToolkitSpawnOperation] Layout is null for loader: {layoutLoader.GetType().Name} (Address: {layoutLoader.LayoutAddress})");
                return null;
            }

            if (_parent == null)
            {
                Debug.LogError(
                    $"[UIToolkitSpawnOperation] Parent element is null for loader: {layoutLoader.GetType().Name}");
                return null;
            }

            TemplateContainer templateContainer = layout.CloneTree();
            VisualElement instance = ExtractRootElement(templateContainer, layoutLoader);

            if (!string.IsNullOrEmpty(_name))
            {
                instance.name = _name;
            }

            instance.style.display = _visible ? StyleKeyword.Null : DisplayStyle.None;
            _parent.Add(instance);

            elementBinder.BindElementsFrom(instance);

            return instance;
        }

        private static VisualElement ExtractRootElement(
            TemplateContainer templateContainer,
            UIToolkitLayoutLoader layoutLoader)
        {

            if (templateContainer.childCount != 1)
            {
                Debug.LogError(
                    $"[UIToolkitSpawnOperation] Layout `{layoutLoader.LayoutAddress}` must contain exactly one root element.");
                return null;
            }

            VisualElement rootElement = templateContainer.ElementAt(0);
            rootElement.RemoveFromHierarchy();

            for (int i = 0; i < templateContainer.styleSheets.count; i++)
            {
                StyleSheet styleSheet = templateContainer.styleSheets[i];
                rootElement.styleSheets.Add(styleSheet);
            }

            return rootElement;
        }
    }
}
#endif
