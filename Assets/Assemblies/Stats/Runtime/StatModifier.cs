using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace Stats.Runtime
{
    [CreateAssetMenu(menuName = "ActionFlow/Stats/Stat Modifier", fileName = "StatModifier")]
    public sealed class StatModifier : SerializedScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public Stat Definition;
            public float Delta;
        }

        [OdinSerialize]
        private List<Entry> _entries = new();

        public IReadOnlyList<Entry> Entries => _entries;
    }
}
