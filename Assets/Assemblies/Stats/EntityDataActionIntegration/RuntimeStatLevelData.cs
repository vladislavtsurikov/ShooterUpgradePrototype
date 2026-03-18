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

        [NonSerialized] private int _previousLevel;

        public LevelProgressionTable LevelProgressionTable => _levelProgressionTable;
        public ReactiveProperty<int> AppliedLevel => _appliedLevel ??= new ReactiveProperty<int>();
        public int PreviousLevel => _previousLevel;

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

            _previousLevel = AppliedLevel.Value;

            AppliedLevel.Value = clampedLevel;
            Save();
            return true;
        }

        protected override void RestoreDefaultsValue()
        {
            int value = _levelProgressionTable != null
                ? _levelProgressionTable.ClampLevel(_initialLevel)
                : _initialLevel;

            _previousLevel = value;
            AppliedLevel.Value = value;
        }

        protected override void RestoreValue()
        {
            if (!PlayerPrefs.HasKey(GetLevelKey(StatId)))
            {
                return;
            }

            int level = PlayerPrefs.GetInt(GetLevelKey(StatId), _initialLevel);
            int clamped = _levelProgressionTable != null ? _levelProgressionTable.ClampLevel(level) : level;

            _previousLevel = clamped;
            AppliedLevel.Value = clamped;
        }

        protected override void SaveValue()
        {
            PlayerPrefs.SetInt(GetLevelKey(StatId), AppliedLevel.Value);
            PlayerPrefs.Save();
        }

        public static string GetLevelKey(string statId) => $"{statId}Level";
    }
}
