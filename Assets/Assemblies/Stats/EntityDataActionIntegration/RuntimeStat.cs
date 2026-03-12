using System;
using OdinSerializer;
using UniRx;
using VladislavTsurikov.ActionFlow.Runtime.Stats;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [Serializable]
    public sealed class RuntimeStat
    {
        [OdinSerialize] private Stat _stat;
        [OdinSerialize] private ReactiveProperty<float> _value;

        public Stat Stat => _stat;

        public ReactiveProperty<float> Value => EnsureValue();

        public float CurrentValue
        {
            get => Value.Value;
            set => Value.Value = value;
        }

        public RuntimeStat()
        {
        }

        public RuntimeStat(Stat stat, float value)
        {
            _stat = stat;
            EnsureValue().Value = value;
        }

        private ReactiveProperty<float> EnsureValue()
        {
            if (_value == null)
            {
                _value = new ReactiveProperty<float>();
            }

            return _value;
        }
    }
}
