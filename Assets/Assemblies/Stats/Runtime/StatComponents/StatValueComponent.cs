using OdinSerializer;
using Stats.EntityDataActionIntegration;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.ReflectionUtility;

namespace Stats.Runtime.StatComponents
{
    [Persistent]
    [Name("Stats/Value")]
    [global::Nody.Runtime.Core.GroupAttribute("Stats")]
    public sealed class StatValueComponent : StatComponentData
    {
        [OdinSerialize]
        private float _baseValue;

        [OdinSerialize]
        private bool _clampEnabled;

        [OdinSerialize]
        [ShowIf(nameof(UseMax), true)]
        private float _maxValue;

        [OdinSerialize]
        [ShowIf(nameof(UseMin), true)]
        private float _minValue;

        [OdinSerialize]
        [ShowIf(nameof(ClampEnabled), true)]
        private bool _useMax;

        [OdinSerialize]
        [ShowIf(nameof(ClampEnabled), true)]
        private bool _useMin;

        public float BaseValue => _baseValue;
        public bool ClampEnabled => _clampEnabled;
        public bool UseMin => _useMin;
        public float MinValue => _minValue;
        public bool UseMax => _useMax;
        public float MaxValue => _maxValue;

        public float ApplyClamp(float value)
        {
            if (ClampEnabled)
            {
                if (UseMin)
                {
                    value = Mathf.Max(value, MinValue);
                }

                if (UseMax)
                {
                    value = Mathf.Min(value, MaxValue);
                }
            }

            return value;
        }

        public void SetBaseValue(float value) => _baseValue = ApplyClamp(value);

        public void Configure(
            float baseValue,
            bool save,
            bool clampEnabled,
            bool useMin,
            float minValue,
            bool useMax,
            float maxValue)
        {
            SetSave(save);
            _clampEnabled = clampEnabled;
            _useMin = clampEnabled && useMin;
            _minValue = minValue;
            _useMax = clampEnabled && useMax;
            _maxValue = maxValue;
            SetBaseValue(baseValue);
        }

        public override RuntimeStatData CreateRuntimeComponent()
        {
            return new RuntimeStatValueData(_baseValue, Save, _clampEnabled, _useMin, _minValue, _useMax, _maxValue);
        }
    }
}
