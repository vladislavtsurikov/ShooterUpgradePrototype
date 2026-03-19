using System.Collections.Generic;
using System.Linq;
using LevelProgression.Runtime.ProgressionTables;
using OdinSerializer;
using UnityEngine;

namespace LevelProgression.Runtime
{
    [CreateAssetMenu(menuName = "ActionFlow/Level Progression/Table", fileName = "LevelProgressionTable")]
    public sealed class LevelProgressionTable : SerializedScriptableObject
    {
        [OdinSerialize]
        private List<float> _values = new() { 0f };

        [OdinSerialize]
        private ProgressionTable _progression;

        public ProgressionTable Progression => _progression;
        public IReadOnlyList<float> Values => _values;
        public int MaxLevel => Mathf.Max(0, _values.Count - 1);

        private void OnEnable()
        {
            if (_values == null || _values.Count == 0)
            {
                _values = new List<float> { 0f };
            }

            _values = BuildValues();
        }

        public void SetProgression(ProgressionTable progression)
        {
            _progression = progression;
            _values = BuildValues();
        }

        public void SetValuesCount(int count)
        {
            _progression.SetValuesCount(count);
            _values = BuildValues();
            _values = ResizeValues(_values, count);
        }

        public void SetValue(int level, float value)
        {
            _progression.SetValue(level, value);

            _values = BuildValues();
            _values[Mathf.Clamp(level, 0, _values.Count - 1)] = value;
        }

        public float GetValue(int level)
        {
            int clampedLevel = ClampLevel(level);
            if (_values.Count == 0)
            {
                return 0f;
            }

            return _values[clampedLevel];
        }

        public float GetCumulativeValue(int level)
        {
            int clampedLevel = ClampLevel(level);

            if (_values.Count == 0)
            {
                return 0f;
            }

            float total = 0f;

            for (int i = 1; i <= clampedLevel; i++)
            {
                total += _values[i];
            }

            return total;
        }

        public int ClampLevel(int level) => Mathf.Clamp(level, 0, MaxLevel);

        private List<float> BuildValues()
        {
            return _progression?.BuildValues()?.ToList();
        }

        private static List<float> ResizeValues(List<float> values, int count)
        {
            int targetCount = Mathf.Max(1, count);

            while (values.Count < targetCount)
            {
                float nextValue = values.Count == 0 ? 0f : values[^1];
                values.Add(nextValue);
            }

            while (values.Count > targetCount)
            {
                values.RemoveAt(values.Count - 1);
            }

            return values;
        }

        public void RebuildValues()
        {
            _values = BuildValues();
        }
    }
}
