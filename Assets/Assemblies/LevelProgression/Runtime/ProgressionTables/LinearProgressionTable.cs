using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.LevelProgression
{
    [Serializable]
    public sealed class LinearProgressionTable : ProgressionTable
    {
        [OdinSerialize] private int _maxLevel = 10;
        [OdinSerialize] private float _baseValue;
        [OdinSerialize] private float _incrementPerLevel = 1f;

        public override string DisplayName => "Linear Progression";
        public override string Description => "Each level grows by a constant increment.";

        public int MaxLevel
        {
            get => _maxLevel;
            set => _maxLevel = Mathf.Max(0, value);
        }

        public float BaseValue
        {
            get => _baseValue;
            set => _baseValue = value;
        }

        public float IncrementPerLevel
        {
            get => _incrementPerLevel;
            set => _incrementPerLevel = value;
        }

        public override void Initialize(List<float> values)
        {
            RebuildValues(values);
        }

        public override void RebuildValues(List<float> values)
        {
            ResizeValues(values, MaxLevel + 1);

            for (int level = 0; level < values.Count; level++)
            {
                values[level] = BaseValue + IncrementPerLevel * level;
            }
        }
    }
}
