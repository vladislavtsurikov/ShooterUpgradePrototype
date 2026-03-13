using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.LevelProgression
{
    [CreateAssetMenu(menuName = "ActionFlow/Level Progression/Table", fileName = "LevelProgressionTable")]
    public sealed class LevelProgressionTable : SerializedScriptableObject
    {
        [OdinSerialize]
        private List<float> _values = new() { 0f };

        [OdinSerialize]
        private ProgressionTable _progression = new ManualProgressionTable();

        public ProgressionTable Progression => EnsureProgression();
        public IReadOnlyList<float> Values => _values;
        public int MaxLevel => Mathf.Max(0, _values.Count - 1);

        private void OnEnable()
        {
            EnsureProgression();
            Progression.Initialize(_values);
        }

        public void SetProgression(ProgressionTable progression)
        {
            _progression = progression ?? new ManualProgressionTable();
            _progression.Initialize(_values);
        }

        public void ConfigureLinear(int maxLevel, float baseValue, float incrementPerLevel)
        {
            var progression = new LinearProgressionTable
            {
                MaxLevel = maxLevel,
                BaseValue = baseValue,
                IncrementPerLevel = incrementPerLevel
            };

            SetProgression(progression);
            RebuildValues();
        }

        public void ConfigureManual(IReadOnlyList<float> values)
        {
            SetProgression(new ManualProgressionTable());

            if (values == null || values.Count == 0)
            {
                SetValuesCount(1);
                SetValue(0, 0f);
                return;
            }

            SetValuesCount(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                SetValue(i, values[i]);
            }
        }

        public void RebuildValues()
        {
            EnsureProgression().RebuildValues(_values);
        }

        public void SetValuesCount(int count)
        {
            EnsureProgression().SetValuesCount(_values, count);
        }

        public void SetValue(int level, float value)
        {
            EnsureProgression().SetValue(_values, level, value);
        }

        public float GetValue(int level)
        {
            EnsureProgression();
            int clampedLevel = ClampLevel(level);
            if (_values.Count == 0)
            {
                return 0f;
            }

            return _values[clampedLevel];
        }

        public float GetDelta(int level)
        {
            int clampedLevel = ClampLevel(level);
            if (clampedLevel <= 0)
            {
                return GetValue(clampedLevel);
            }

            return GetValue(clampedLevel) - GetValue(clampedLevel - 1);
        }

        public int GetLevel(float value)
        {
            if (_values.Count == 0)
            {
                return 0;
            }

            for (int i = _values.Count - 1; i >= 0; i--)
            {
                if (value >= _values[i])
                {
                    return i;
                }
            }

            return 0;
        }

        public float GetPreviousValue(int level)
        {
            int clampedLevel = ClampLevel(level);
            return GetValue(Mathf.Max(0, clampedLevel - 1));
        }

        public float GetNextValue(int level)
        {
            int clampedLevel = ClampLevel(level);
            return GetValue(Mathf.Min(MaxLevel, clampedLevel + 1));
        }

        public float GetProgress01(float value)
        {
            if (MaxLevel <= 0)
            {
                return 1f;
            }

            int level = GetLevel(value);
            if (level >= MaxLevel)
            {
                return 1f;
            }

            float previousValue = GetValue(level);
            float nextValue = GetNextValue(level);
            if (Mathf.Approximately(previousValue, nextValue))
            {
                return 1f;
            }

            return Mathf.Clamp01((value - previousValue) / (nextValue - previousValue));
        }

        public int ClampLevel(int level) => Mathf.Clamp(level, 0, MaxLevel);

        private ProgressionTable EnsureProgression()
        {
            if (_progression != null)
            {
                return _progression;
            }

            _progression = new ManualProgressionTable();
            _progression.Initialize(_values);
            return _progression;
        }
    }
}
