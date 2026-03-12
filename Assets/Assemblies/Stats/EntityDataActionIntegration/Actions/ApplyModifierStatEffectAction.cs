using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [RequiresData(typeof(StatsEntityData), typeof(ArmyClash.Battle.Data.ModifiersData))]
    [Name("Stats/Apply Modifier Stat Effect")]
    public sealed class ApplyModifierStatEffectAction : EntityMonoBehaviourAction
    {
        private IDisposable _subscription;

        protected override void Awake()
        {
            var data = Get<ArmyClash.Battle.Data.ModifiersData>();
            _subscription?.Dispose();
            IObservable<Unit> added = data.Effects.ObserveAdd().Select(_ => Unit.Default);
            IObservable<Unit> removed = data.Effects.ObserveRemove().Select(_ => Unit.Default);
            IObservable<Unit> reset = data.Effects.ObserveReset().Select(_ => Unit.Default);
            _subscription = added.Merge(removed, reset)
                .Subscribe(_ => ApplyAll(data));
        }

        protected override void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected override UniTask<bool> Run(CancellationToken token)
        {
            var data = Get<ArmyClash.Battle.Data.ModifiersData>();
            ApplyAll(data);

            return UniTask.FromResult(true);
        }

        private void ApplyAll(ArmyClash.Battle.Data.ModifiersData data)
        {
            StatsEntityData stats = Get<StatsEntityData>();
            stats.RebuildFromCollection();

            foreach (var t in data.Effects)
            {
                ApplyEffect(stats, t);
            }
        }

        private static void ApplyEffect(StatsEntityData stats, ModifierStatEffect effect)
        {
            var entries = effect.Entries;
            for (int i = 0; i < entries.Count; i++)
            {
                Stat stat = entries[i].Stat;
                stats.AddStatValue(stat, entries[i].Delta);
            }
        }
    }
}
