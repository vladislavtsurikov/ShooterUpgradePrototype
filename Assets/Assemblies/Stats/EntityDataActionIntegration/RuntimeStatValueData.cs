using System;
using OdinSerializer;
using UniRx;
using UnityEngine;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [Serializable]
    public sealed class RuntimeStatValueData : RuntimeStatData
    {
        [OdinSerialize] private float _baseValue;
        [OdinSerialize] private bool _clampEnabled;
        [OdinSerialize] private float _maxValue;
        [OdinSerialize] private float _minValue;
        [OdinSerialize] private bool _useMax;
        [OdinSerialize] private bool _useMin;
        [OdinSerialize] private ReactiveProperty<float> _value;

        public RuntimeStatValueData()
        {
        }

        public RuntimeStatValueData(float baseValue, bool save, bool clampEnabled, bool useMin, float minValue, bool useMax, float maxValue)
            : base(save)
        {
            _clampEnabled = clampEnabled;
            _useMin = useMin;
            _minValue = minValue;
            _useMax = useMax;
            _maxValue = maxValue;
            _baseValue = ApplyClamp(baseValue);
            Value.Value = _baseValue;
        }

        public float BaseValue => _baseValue;
        public ReactiveProperty<float> Value => _value ??= new ReactiveProperty<float>();
        public float CurrentValue
        {
            get => Value.Value;
            set => SetValue(value);
        }

        public bool SetValue(float value)
        {
            float clampedValue = ApplyClamp(value);
            if (Mathf.Approximately(CurrentValue, clampedValue))
            {
                return false;
            }

            Value.Value = clampedValue;
            Save();
            return true;
        }

        public bool AddValue(float delta)
        {
            return SetValue(CurrentValue + delta);
        }

        public void ResetToBaseValue()
        {
            Value.Value = _baseValue;
        }

        public float ApplyClamp(float value)
        {
            if (_clampEnabled)
            {
                if (_useMin)
                {
                    value = Mathf.Max(value, _minValue);
                }

                if (_useMax)
                {
                    value = Mathf.Min(value, _maxValue);
                }
            }

            return value;
        }

        protected override void RestoreDefaultsValue()
        {
            ResetToBaseValue();
        }

        protected override void RestoreValue()
        {
            if (!PlayerPrefs.HasKey(GetValueKey(StatId)))
            {
                return;
            }

            Value.Value = ApplyClamp(PlayerPrefs.GetFloat(GetValueKey(StatId)));
        }

        protected override void SaveValue()
        {
            PlayerPrefs.SetFloat(GetValueKey(StatId), CurrentValue);
            PlayerPrefs.Save();
        }

        public static string GetValueKey(string statId) => $"{statId}Value";
    }
}
