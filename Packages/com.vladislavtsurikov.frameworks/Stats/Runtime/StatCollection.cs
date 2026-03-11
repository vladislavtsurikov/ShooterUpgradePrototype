using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
{
    [CreateAssetMenu(menuName = "ActionFlow/Stats/Stat Collection", fileName = "StatCollection")]
    public sealed class StatCollection : SerializedScriptableObject
    {
        [OdinSerialize]
        private List<Stat> _stats = new();

        public IReadOnlyList<Stat> Stats => _stats;

        public bool AddStat(Stat stat)
        {
            if (stat == null || _stats.Contains(stat))
            {
                return false;
            }

            _stats.Add(stat);
            return true;
        }

        public bool RemoveStat(Stat stat) => _stats.Remove(stat);

        public void Clear() => _stats.Clear();
    }
}
