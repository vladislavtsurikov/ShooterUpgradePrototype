using System;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer;
using UnityEngine;

namespace Stats.Runtime
{
    [CreateAssetMenu(menuName = "Stats/Effect", fileName = "StatEffect")]
    public class StatEffect : SerializedScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public Stat Stat;
            public float Delta;
        }

        [OdinSerialize, HideInInspector]
        private StatsComponentStack _componentStack = new();

        [OdinSerialize]
        private List<Entry> _entries = new();

        public IReadOnlyList<Entry> Entries => _entries;
        public StatsComponentStack ComponentStack => _componentStack;

        public void SetEntries(IEnumerable<Entry> entries)
        {
            _entries.Clear();

            if (entries == null)
            {
                return;
            }

            _entries.AddRange(entries.ToList());
        }

        private void OnEnable()
        {
            _componentStack ??= new StatsComponentStack();
            _componentStack.Setup(true, new object[] { this });
        }

        private void OnDisable() => _componentStack?.OnDisable();
    }
}
