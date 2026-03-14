using System;
using OdinSerializer;
using UniRx;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.LevelProgression;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [Serializable]
    public sealed class RuntimeStatLevelData : RuntimeStatData
    {
        [OdinSerialize] private LevelProgressionTable _levelProgressionTable;
        [OdinSerialize] private int _initialLevel;
        [OdinSerialize] private ReactiveProperty<int> _appliedLevel;

        public LevelProgressionTable LevelProgressionTable => _levelProgressionTable;
        public ReactiveProperty<int> AppliedLevel => _appliedLevel ??= new ReactiveProperty<int>();

        public RuntimeStatLevelData()
        {
        }

        public RuntimeStatLevelData(LevelProgressionTable levelProgressionTable, int initialLevel, bool save)
            : base(save)
        {
            _levelProgressionTable = levelProgressionTable;
            _initialLevel = levelProgressionTable != null ? levelProgressionTable.ClampLevel(initialLevel) : initialLevel;
            SetLevel(_initialLevel);
        }

        public bool SetLevel(int level)
        {
            int clampedLevel = _levelProgressionTable != null ? _levelProgressionTable.ClampLevel(level) : level;
            if (AppliedLevel.Value == clampedLevel)
            {
                return false;
            }

            AppliedLevel.Value = clampedLevel;
            Save();
            return true;
        }

        protected override void RestoreDefaultsValue()
        {
            AppliedLevel.Value = _levelProgressionTable != null
                ? _levelProgressionTable.ClampLevel(_initialLevel)
                : _initialLevel;
        }

        protected override void RestoreValue()
        {
            if (!PlayerPrefs.HasKey(GetLevelKey(StatId)))
            {
                return;
            }

            int level = PlayerPrefs.GetInt(GetLevelKey(StatId), _initialLevel);
            AppliedLevel.Value = _levelProgressionTable != null ? _levelProgressionTable.ClampLevel(level) : level;
        }

        protected override void SaveValue()
        {
            PlayerPrefs.SetInt(GetLevelKey(StatId), AppliedLevel.Value);
            PlayerPrefs.Save();
        }

        public static string GetLevelKey(string statId) => $"{statId}Level";
    }
}
