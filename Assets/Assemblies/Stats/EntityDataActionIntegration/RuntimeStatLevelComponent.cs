using System;
using OdinSerializer;
using UniRx;
using VladislavTsurikov.ActionFlow.Runtime.Stats;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [Serializable]
    public sealed class RuntimeStatLevelData : RuntimeStatData
    {
        [OdinSerialize] private Table _table;
        [OdinSerialize] private int _initialLevel;
        [OdinSerialize] private ReactiveProperty<int> _appliedLevel;

        public Table Table => _table;
        public ReactiveProperty<int> AppliedLevel => EnsureAppliedLevel();

        public RuntimeStatLevelData()
        {
        }

        public RuntimeStatLevelData(Table table, int initialLevel, bool save)
            : base(save)
        {
            _table = table;
            _initialLevel = table != null ? table.ClampLevel(initialLevel) : initialLevel;
            SetLevel(_initialLevel);
        }

        public bool SetLevel(int level)
        {
            int clampedLevel = _table != null ? _table.ClampLevel(level) : level;
            if (AppliedLevel.Value == clampedLevel)
            {
                return false;
            }

            AppliedLevel.Value = clampedLevel;
            return true;
        }

        private ReactiveProperty<int> EnsureAppliedLevel()
        {
            if (_appliedLevel == null)
            {
                _appliedLevel = new ReactiveProperty<int>();
            }

            return _appliedLevel;
        }

        public override void Restore(RuntimeStatBuildContext context)
        {
            int level = _initialLevel;
            if (Save && PlayerPrefs.HasKey(GetLevelKey(context.StatId)))
            {
                level = PlayerPrefs.GetInt(GetLevelKey(context.StatId), _initialLevel);
            }

            SetLevel(level);
        }

        public override void Persist(RuntimeStatBuildContext context)
        {
            if (!Save)
            {
                return;
            }

            PlayerPrefs.SetInt(GetLevelKey(context.StatId), AppliedLevel.Value);
            PlayerPrefs.Save();
        }

        public static string GetLevelKey(string statId) => $"{statId}Level";
    }
}
