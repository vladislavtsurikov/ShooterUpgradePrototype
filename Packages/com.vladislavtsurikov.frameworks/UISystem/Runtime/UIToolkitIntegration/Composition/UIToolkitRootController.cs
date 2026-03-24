#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class UIToolkitRootController
    {
        public VisualElement Root { get; private set; }

        public async UniTask<VisualElement> EnsureSpawnedAsync(
            UIToolkitLayoutLoader loader,
            VisualElement parent,
            string rootName,
            UIToolkitElementBinder elementBinder,
            CancellationToken cancellationToken)
        {
            if (Root != null)
            {
                return Root;
            }

            Root = await new UIToolkitSpawnOperation()
                .WithParent(parent)
                .Visible(true)
                .WithName(rootName)
                .Execute(loader, elementBinder, cancellationToken);

            if (Root != null)
            {
                StretchToParent(Root);
            }

            return Root;
        }

        public void Show()
        {
            if (Root != null)
            {
                Root.style.display = StyleKeyword.Null;
            }
        }

        public void Hide()
        {
            if (Root != null)
            {
                Root.style.display = DisplayStyle.None;
            }
        }

        public async UniTask DestroyAsync(bool unload, UIToolkitLayoutLoader loader, CancellationToken cancellationToken)
        {
            if (Root != null)
            {
                Root.RemoveFromHierarchy();
                Root = null;
            }

            if (unload)
            {
                await loader.Unload(cancellationToken);
            }
        }

        public void Dispose() => Root = null;

        private static void StretchToParent(VisualElement element)
        {
            element.style.position = Position.Absolute;
            element.style.left = 0;
            element.style.top = 0;
            element.style.right = 0;
            element.style.bottom = 0;
            element.style.width = StyleKeyword.Auto;
            element.style.height = StyleKeyword.Auto;
            element.style.flexGrow = 1;
            element.style.flexShrink = 0;
        }
    }
}
#endif
