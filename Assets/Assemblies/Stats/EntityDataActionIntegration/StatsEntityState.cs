using System;
using System.Collections.Generic;
using OdinSerializer;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [Serializable]
    public sealed class StatsEntityState
    {
        [OdinSerialize]
        private StatCollection _collection;

        [NonSerialized]
        private Dictionary<string, RuntimeStat> _stats = new();

        [NonSerialized]
        private bool _isInitialized;

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
            }
        }

        public IReadOnlyDictionary<string, RuntimeStat> Stats => _stats ??= new Dictionary<string, RuntimeStat>();

        public void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            RebuildFromCollection();
        }

        public void RebuildFromCollection()
        {
            _isInitialized = true;
            _stats ??= new Dictionary<string, RuntimeStat>();

            if (_collection == null)
            {
                _stats.Clear();
                return;
            }

            var sourceStats = _collection.Stats;
            var rebuiltStats = new Dictionary<string, RuntimeStat>(sourceStats.Count);

            for (int i = 0; i < sourceStats.Count; i++)
            {
                Stat stat = sourceStats[i];
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
