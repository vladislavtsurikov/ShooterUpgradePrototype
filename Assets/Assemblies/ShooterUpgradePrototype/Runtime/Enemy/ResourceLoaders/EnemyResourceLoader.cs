#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;
using VladislavTsurikov.Core.Runtime;
using Zenject;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    [GlobalFilter]
    public sealed class EnemyResourceLoader : BindableResourceLoader
    {
        public EnemyResourceLoader(DiContainer container) : base(container)
        {
        }

        public EnemySpawnConfig EnemySpawnConfig { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token)
        {
            EnemySpawnConfig = await LoadAndBind<EnemySpawnConfig>(token, "EnemySpawnConfig");
        }
    }
}
#endif
#endif
