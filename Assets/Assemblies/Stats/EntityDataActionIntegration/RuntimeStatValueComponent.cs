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
            EnsureValue().Value = _baseValue;
        }

        public float BaseValue => _baseValue;
        public ReactiveProperty<float> Value => EnsureValue();
        public float CurrentValue
        {
            get => Value.Value;
            set => Value.Value = ApplyClamp(value);
        }

        public bool SetValue(float value)
        {
            float clampedValue = ApplyClamp(value);
            if (Mathf.Approximately(CurrentValue, clampedValue))
            {
                return false;
            }

            Value.Value = clampedValue;
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

        public override void Restore(RuntimeStatBuildContext context)
        {
            ResetToBaseValue();

            if (!Save || !PlayerPrefs.HasKey(GetValueKey(context.StatId)))
            {
                return;
            }

            CurrentValue = PlayerPrefs.GetFloat(GetValueKey(context.StatId));
        }

        public override void Persist(RuntimeStatBuildContext context)
        {
            if (!Save)
            {
                return;
            }

            PlayerPrefs.SetFloat(GetValueKey(context.StatId), CurrentValue);
            PlayerPrefs.Save();
        }

        private ReactiveProperty<float> EnsureValue()
        {
            if (_value == null)
            {
                _value = new ReactiveProperty<float>();
            }

            return _value;
        }

        public static string GetValueKey(string statId) => $"{statId}Value";
    }
}
