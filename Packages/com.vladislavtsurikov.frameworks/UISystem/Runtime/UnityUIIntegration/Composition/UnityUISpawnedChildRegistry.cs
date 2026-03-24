#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public sealed class UnityUISpawnedChildRegistry
    {
        private readonly List<(GameObject instance, PrefabAssetLoader loader)> _entries = new();

        public void Register(GameObject instance, PrefabAssetLoader loader) =>
            _entries.Add((instance, loader));

        public async UniTask DestroyAsync(bool unload, CancellationToken cancellationToken)
        {
            if (unload)
            {
                foreach ((GameObject _, PrefabAssetLoader loader) in _entries)
                {
                    await loader.Unload(cancellationToken);
                }
            }

            _entries.Clear();
        }

        public void Dispose() => _entries.Clear();
    }
}
#endif
