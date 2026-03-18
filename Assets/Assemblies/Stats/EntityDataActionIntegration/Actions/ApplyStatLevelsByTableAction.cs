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
                    .Subscribe(_ => ApplyLevel(runtimeStat,  levelData))
                    .AddTo(_subscriptions);
            }
        }

        private void ApplyLevels()
        {
            StatsEntityData statsEntityData = Get<StatsEntityData>();

            foreach (RuntimeStat runtimeStat in statsEntityData.Stats.Values)
            {
                if (!runtimeStat.Runtime().TryData(out RuntimeStatLevelData levelData))
                {
                    continue;
                }

                RuntimeStatValueData valueData = runtimeStat.Runtime().Data<RuntimeStatValueData>();

                int level = levelData.AppliedLevel.Value;

                float total = 0f;

                for (int i = 1; i <= level; i++)
                {
                    total += levelData.LevelProgressionTable.GetValue(i);
                }

                valueData.AddValue(total);
            }
        }

        private void ApplyLevel(RuntimeStat runtimeStat, RuntimeStatLevelData levelData)
        {
            int previousLevel = levelData.PreviousLevel;
            int currentLevel = levelData.AppliedLevel.Value;

            float previousValue = levelData.LevelProgressionTable.GetCumulativeValue(previousLevel);
            float currentValue = levelData.LevelProgressionTable.GetCumulativeValue(currentLevel);

            float delta = currentValue - previousValue;

            runtimeStat.Runtime().Data<RuntimeStatValueData>().AddValue(delta);
        }
    }
}
