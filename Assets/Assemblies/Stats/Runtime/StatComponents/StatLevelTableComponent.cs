using OdinSerializer;
using UnityEngine;
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
        private Table _table;

        [OdinSerialize]
        private int _initialLevel;

        public Table Table => _table;
        public int InitialLevel => _initialLevel;

        public override RuntimeStatData CreateRuntimeComponent(RuntimeStatBuildContext context)
        {
            return new RuntimeStatLevelData(_table, _initialLevel, Save);
        }
    }
}
