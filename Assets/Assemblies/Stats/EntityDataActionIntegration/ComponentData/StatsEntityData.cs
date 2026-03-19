using OdinSerializer;
using Stats.Runtime;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace Stats.EntityDataActionIntegration
{
    [Name("Stats/StatsEntity")]
    public sealed class StatsEntityData : ComponentData
    {
        [OdinSerialize] private StatsEntitySourceType _sourceType;
        [OdinSerialize] private StatsEntityState _localStats = new();
        [OdinSerialize] private StatsEntityConfig _globalConfig;

        public StatsEntitySourceType SourceType
        {
            get => _sourceType;
            set
            {
                if (_sourceType == value)
                {
                    return;
                }

                _sourceType = value;
                ResolveData().EnsureInitialized();
                MarkDirty();
            }
        }

        public StatsEntityConfig GlobalConfig
        {
            get => _globalConfig;
            set
            {
                if (_globalConfig == value)
                {
                    return;
                }

                _globalConfig = value;
                ResolveData().EnsureInitialized();
                MarkDirty();
            }
        }

        public StatsEntityState LocalStats => _localStats ??= new StatsEntityState();

        public StatsEntityState Data => ResolveData();

        public bool UsesGlobalConfig => _sourceType == StatsEntitySourceType.Global;

        public StatCollection Collection
        {
            get => Data.Collection;
            set
            {
                ResolveWritableData().Collection = value;
                MarkDirty();
            }
        }

        public System.Collections.Generic.IReadOnlyDictionary<string, RuntimeStat> Stats => Data.Stats;

        protected override void SetupComponent(object[] setupData = null)
        {
            Data.EnsureInitialized();
        }

        public void RebuildFromCollection() => ResolveWritableData().RebuildFromCollection();

        internal RuntimeStat GetRuntimeStatById(string id) => Data.GetRuntimeStatById(id);

        private StatsEntityState ResolveData()
        {
            if (_sourceType == StatsEntitySourceType.Global)
            {
                return _globalConfig.Stats;
            }

            return LocalStats;
        }

        private StatsEntityState ResolveWritableData()
        {
            if (_sourceType == StatsEntitySourceType.Global)
            {
                return _globalConfig.Stats;
            }

            return LocalStats;
        }
    }
}
