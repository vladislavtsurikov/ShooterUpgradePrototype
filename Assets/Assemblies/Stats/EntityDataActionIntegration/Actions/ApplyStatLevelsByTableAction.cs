using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [RequiresData(typeof(StatsEntityData))]
    [Name("Stats/Apply Stat Levels By Table")]
    public sealed class ApplyStatLevelsByTableAction : EntityMonoBehaviourAction
    {
        private CompositeDisposable _subscriptions = new();

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            RebindLevelSubscriptions();
        }

        protected override void OnDisable()
        {
            _subscriptions?.Clear();
        }

        protected override UniTask<bool> Run(CancellationToken token)
        {
            ApplyLevels();
            return UniTask.FromResult(true);
        }

        private void RebindLevelSubscriptions()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            StatsEntityData statsEntityData = Get<StatsEntityData>();
            foreach (RuntimeStat runtimeStat in statsEntityData.Stats.Values)
            {
                if (!runtimeStat.Runtime().TryData(out RuntimeStatLevelData levelData))
                {
                    continue;
                }

                levelData.AppliedLevel
                    .Skip(1)
                    .Subscribe(_ => ApplyLevels())
                    .AddTo(_subscriptions);
            }
        }

        public void ApplyLevels()
        {
            StatsEntityData statsEntityData = Get<StatsEntityData>();
            foreach (RuntimeStat runtimeStat in statsEntityData.Stats.Values)
            {
                if (!runtimeStat.Runtime().TryData(out RuntimeStatLevelData levelComponent))
                {
                    continue;
                }

                if (!runtimeStat.Runtime().TryData(out RuntimeStatValueData valueComponent))
                {
                    continue;
                }

                float targetValue = levelComponent.LevelProgressionTable.GetValue(levelComponent.AppliedLevel.Value);
                valueComponent.SetValue(targetValue);
            }
        }
    }
}
