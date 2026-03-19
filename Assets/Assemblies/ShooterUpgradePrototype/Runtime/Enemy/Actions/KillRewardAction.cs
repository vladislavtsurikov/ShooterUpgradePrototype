using System.Threading;
using Cysharp.Threading.Tasks;
using Nody.Runtime.Core;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Zenject;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    [Name("AutoStrike/Actions/KillReward")]
    [Group("Death")]
    public sealed class KillRewardAction : EntityMonoBehaviourAction
    {
        [SerializeField]
        [Min(0)]
        private int _killRewardPoints = 1;

        [Inject]
        private EnemyRegistryService _registry;

        [Inject]
        private EnemyRewardService _rewardService;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (EntityMonoBehaviour is not EnemyEntity enemy)
            {
                return UniTask.FromResult(false);
            }

            _registry.Unregister(enemy);
            _rewardService.Grant(_killRewardPoints);

            return UniTask.FromResult(true);
        }
    }
}
