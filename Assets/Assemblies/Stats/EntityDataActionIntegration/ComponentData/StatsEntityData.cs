using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.Nody.Runtime.Core;
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
            if (_collection == null)
            {
                _stats.Clear();
                return;
            }

            var sourceStats = _collection.Stats;
            if (sourceStats == null)
            {
                _stats.Clear();
                return;
            }

            var rebuiltStats = new Dictionary<string, RuntimeStat>(sourceStats.Count);

            for (int i = 0; i < sourceStats.Count; i++)
            {
                Stat stat = sourceStats[i];
                if (stat == null)
                {
                    continue;
                }

                RuntimeStat runtimeStat = _stats.TryGetValue(stat.Id, out RuntimeStat existing)
                    ? existing
                    : new RuntimeStat();

                runtimeStat.SetStat(stat);
                runtimeStat.ClearRuntimeData();
                BuildRuntimeComponents(stat, runtimeStat);
                runtimeStat.Runtime().Restore();

                rebuiltStats[stat.Id] = runtimeStat;
            }

            _stats.Clear();
            foreach (KeyValuePair<string, RuntimeStat> pair in rebuiltStats)
            {
                _stats[pair.Key] = pair.Value;
            }
        }

        internal RuntimeStat GetRuntimeStatById(string id) => _stats[id];

        private static void BuildRuntimeComponents(Stat stat, RuntimeStat runtimeStat)
        {
            IList<ComponentData> components = stat.ComponentStack.List;
            if (components == null)
            {
                return;
            }

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is not StatComponentData statComponent)
                {
                    continue;
                }

                RuntimeStatData runtimeData = statComponent.CreateRuntimeComponent();
                runtimeStat.AddRuntimeData(runtimeData);
            }
        }

    }
}
