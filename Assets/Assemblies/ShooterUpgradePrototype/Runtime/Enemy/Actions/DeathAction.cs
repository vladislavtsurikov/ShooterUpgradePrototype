using System.Threading;
using Cysharp.Threading.Tasks;
using Nody.Runtime.Core;
using OdinSerializer;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(StatsEntityData))]
    [Name("AutoStrike/Actions/Death")]
    public sealed class DeathAction : EntityMonoBehaviourAction
    {
        private const string HealthId = "HP";

        [OdinSerialize]
        private EntityActionCollection _deathActionStack = new();

        private CompositeDisposable _subscriptions = new();

        protected override void SetupComponent(object[] setupData = null)
        {
            _deathActionStack ??= new EntityActionCollection();
            _deathActionStack.SetAllowedGroupAttributes(new[] { "Death" });
            _deathActionStack.Setup(true, setupData);
        }

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            StatsEntityData stats = Entity.GetData<StatsEntityData>();
            stats.Stat(HealthId).RuntimeData<RuntimeStatValueData>().Value
                .Where(currentValue => currentValue == 0f)
                .Take(1)
                .Subscribe(_ => HandleDeath().Forget())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable() => _subscriptions?.Clear();

        protected override void OnDisableElement()
        {
            _deathActionStack?.OnDisable();
        }

        private async UniTaskVoid HandleDeath()
        {
            CancellationToken cancellationToken = EntityMonoBehaviour.GetCancellationTokenOnDestroy();
            await _deathActionStack.Run(cancellationToken);

            Object.Destroy(EntityMonoBehaviour.gameObject);
        }
    }
}
