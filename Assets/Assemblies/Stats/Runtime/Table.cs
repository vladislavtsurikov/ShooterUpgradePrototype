using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
{
    [CreateAssetMenu(menuName = "ActionFlow/Stats/Table", fileName = "Table")]
    public sealed class Table : SerializedScriptableObject
    {
        public enum ProgressionMode
        {
            Manual = 0,
            Linear = 1
        }

        [OdinSerialize]
        private ProgressionMode _mode = ProgressionMode.Manual;

        [OdinSerialize]
        private List<float> _values = new() { 0f };

        [OdinSerialize]
        private int _linearMaxLevel = 10;

        [OdinSerialize]
        private float _linearBaseValue;

        [OdinSerialize]
        private float _linearIncrementPerLevel = 1f;

        public ProgressionMode Mode
        {
            get => _mode;
            set => _mode = value;
        }

        public IList<float> Values => _values;

        public int LinearMaxLevel
        {
            get => _linearMaxLevel;
            set => _linearMaxLevel = Mathf.Max(0, value);
        }

        public float LinearBaseValue
        {
            get => _linearBaseValue;
            set => _linearBaseValue = value;
        }

        public float LinearIncrementPerLevel
        {
            get => _linearIncrementPerLevel;
            set => _linearIncrementPerLevel = value;
        }

        public int MaxLevel => Mode == ProgressionMode.Linear
            ? Mathf.Max(0, _linearMaxLevel)
            : Mathf.Max(0, _values.Count - 1);

        public float GetValue(int level)
        {
            int clampedLevel = ClampLevel(level);
            if (Mode == ProgressionMode.Linear)
            {
                return _linearBaseValue + _linearIncrementPerLevel * clampedLevel;
            }

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
            if (Mode == ProgressionMode.Linear)
            {
                if (Mathf.Approximately(_linearIncrementPerLevel, 0f))
                {
                    return value >= _linearBaseValue ? MaxLevel : 0;
                }

                float rawLevel = (value - _linearBaseValue) / _linearIncrementPerLevel;
                int level = _linearIncrementPerLevel > 0f
                    ? Mathf.FloorToInt(rawLevel)
                    : Mathf.CeilToInt(rawLevel);
                return ClampLevel(level);
            }

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
    }
}
