#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class UIToolkitSpawnedChildRegistry
    {
        private readonly List<(VisualElement instance, UIToolkitLayoutLoader loader)> _entries = new();

        public void Register(VisualElement instance, UIToolkitLayoutLoader loader) =>
            _entries.Add((instance, loader));

        public async UniTask DestroyAsync(bool unload, CancellationToken cancellationToken)
        {
            if (unload)
            {
                foreach ((VisualElement _, UIToolkitLayoutLoader loader) in _entries)
                {
                    await loader.Unload(cancellationToken);
                }
            }

            foreach ((VisualElement instance, UIToolkitLayoutLoader _) in _entries)
            {
                instance?.RemoveFromHierarchy();
            }

            _entries.Clear();
        }

        public void Dispose() => _entries.Clear();
    }
}
#endif
