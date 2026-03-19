using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace LevelProgression.Runtime.ProgressionTables
{
    [Serializable]
    public sealed class ManualProgressionTable : ProgressionTable
    {
        [OdinSerialize]
        private List<float> _values = new() { 0f };

        public override string DisplayName => "Manual Progression";
        public override string Description => "Each level value is authored by hand.";
        public override bool CanEditValuesDirectly => true;

        public override IReadOnlyList<float> BuildValues()
        {
            return EnsureValues(_values);
        }

        public override void SetValues(IReadOnlyList<float> values)
        {
            _values = EnsureValues(values);
        }

        public override void SetValuesCount(int count)
        {
            _values = ResizeValues(_values, count);
        }

        public override void SetValue(int level, float value)
        {
            _values = EnsureValues(_values);
            _values[Mathf.Clamp(level, 0, _values.Count - 1)] = value;
        }
    }
}
