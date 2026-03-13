namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    public static class StatsEntityDataUtility
    {
        public static StatsEntityStatFluent Stat(this StatsEntityData statsEntityData, string statId) =>
            new(statsEntityData, statId);
    }

    public readonly struct StatsEntityStatFluent
    {
        private readonly StatsEntityData _statsEntityData;
        private readonly string _statId;

        public StatsEntityStatFluent(StatsEntityData statsEntityData, string statId)
        {
            _statsEntityData = statsEntityData;
            _statId = statId;
        }

        public T RuntimeData<T>() where T : RuntimeStatData => _statsEntityData.GetRuntimeStatById(_statId).Runtime().Data<T>();

        public bool TryData<T>(out T data) where T : RuntimeStatData =>
            _statsEntityData.GetRuntimeStatById(_statId).Runtime().TryData(out data);

        public StatsEntityStatFluent Persist()
        {
            _statsEntityData.PersistStat(_statId);
            return this;
        }
    }
}
