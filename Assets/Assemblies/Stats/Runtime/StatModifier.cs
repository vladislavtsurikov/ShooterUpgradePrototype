using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
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
