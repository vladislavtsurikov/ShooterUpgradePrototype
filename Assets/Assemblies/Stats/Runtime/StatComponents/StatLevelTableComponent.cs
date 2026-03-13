using Nody.Runtime.Core;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.LevelProgression;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
{
    [Name("Stats/Level Table")]
    [Group("Stats")]
    public sealed class StatLevelTableComponent : StatComponentData
    {
        [OdinSerialize]
        private LevelProgressionTable _levelProgressionTable;

        [OdinSerialize]
        private int _initialLevel;

        public LevelProgressionTable LevelProgressionTable => _levelProgressionTable;
        public int InitialLevel => _initialLevel;

        public void Configure(LevelProgressionTable levelProgressionTable, int initialLevel, bool save)
        {
            SetSave(save);
            _levelProgressionTable = levelProgressionTable;
            _initialLevel = levelProgressionTable != null
                ? levelProgressionTable.ClampLevel(initialLevel)
                : Mathf.Max(0, initialLevel);
        }

        public override RuntimeStatData CreateRuntimeComponent()
        {
            return new RuntimeStatLevelData(_levelProgressionTable, _initialLevel, Save);
        }
    }
}
