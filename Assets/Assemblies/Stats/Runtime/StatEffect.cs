using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
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

        private void OnEnable()
        {
            _componentStack ??= new StatsComponentStack();
            _componentStack.Setup(true, new object[] { this });
        }

        private void OnDisable() => _componentStack?.OnDisable();
    }
}
