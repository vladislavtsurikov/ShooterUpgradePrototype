using System.Collections.Generic;
using OdinSerializer;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [Name("Stats/StatsEntity")]
    public sealed class StatsEntityData : ComponentData
    {
        [OdinSerialize] private StatCollection _collection;
        [OdinSerialize] private readonly Dictionary<string, RuntimeStat> _stats = new();

        public StatCollection Collection
        {
            get => _collection;
            set
            {
                if (_collection == value)
                {
                    return;
                }

                _collection = value;
                RebuildFromCollection();
                MarkDirty();
            }
        }

        public IReadOnlyDictionary<string, RuntimeStat> Stats => _stats;

        protected override void SetupComponent(object[] setupData = null)
        {
            RebuildFromCollection();
        }

        public void RebuildFromCollection()
        {
            _stats.Clear();

            if (_collection == null)
            {
                return;
            }

            var sourceStats = _collection.Stats;
            if (sourceStats == null)
            {
                return;
            }

            for (int i = 0; i < sourceStats.Count; i++)
            {
                Stat stat = sourceStats[i];
                if (stat == null)
                {
                    continue;
                }

                float value = GetDefaultValue(stat);
                _stats[stat.Id] = new RuntimeStat(stat, value);
            }
        }

        public bool SetStatValue(Stat stat, float value)
        {
            RuntimeStat runtimeStat = GetRuntimeStatById(stat.Id);
            float previous = runtimeStat.CurrentValue;
            runtimeStat.CurrentValue = ApplyClamp(stat, value);
            if (previous.Equals(runtimeStat.CurrentValue))
            {
                return false;
            }

            MarkDirty();
            return true;
        }

        public bool AddStatValue(Stat stat, float delta)
        {
            RuntimeStat runtimeStat = GetRuntimeStatById(stat.Id);
            float previous = runtimeStat.CurrentValue;
            runtimeStat.CurrentValue = ApplyClamp(stat, runtimeStat.CurrentValue + delta);
            if (previous.Equals(runtimeStat.CurrentValue))
            {
                return false;
            }

            MarkDirty();
            return true;
        }

        public bool AddStatValueById(string id, float delta)
        {
            RuntimeStat runtimeStat = GetRuntimeStatById(id);
            float previous = runtimeStat.CurrentValue;
            runtimeStat.CurrentValue = ApplyClamp(runtimeStat.Stat, runtimeStat.CurrentValue + delta);
            if (previous.Equals(runtimeStat.CurrentValue))
            {
                return false;
            }

            MarkDirty();
            return true;
        }

        public bool SetStatValueById(string id, float value)
        {
            RuntimeStat runtimeStat = GetRuntimeStatById(id);
            float previous = runtimeStat.CurrentValue;
            runtimeStat.CurrentValue = ApplyClamp(runtimeStat.Stat, value);
            if (previous.Equals(runtimeStat.CurrentValue))
            {
                return false;
            }

            MarkDirty();
            return true;
        }

        public float GetStatValueById(string id) => GetRuntimeStatById(id).CurrentValue;

        public RuntimeStat GetRuntimeStatById(string id) => _stats[id];

        private float GetDefaultValue(Stat stat)
        {
            StatValueComponent valueComponent = stat.ComponentStack.GetElement<StatValueComponent>();
            return valueComponent != null ? valueComponent.BaseValue : 0f;
        }

        private float ApplyClamp(Stat stat, float value)
        {
            StatValueComponent valueComponent = stat.ComponentStack.GetElement<StatValueComponent>();
            return valueComponent != null ? valueComponent.ApplyClamp(value) : value;
        }
    }
}
