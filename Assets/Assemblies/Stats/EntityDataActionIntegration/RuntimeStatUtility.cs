namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    public static class RuntimeStatUtility
    {
        public static RuntimeStatFluent Runtime(this RuntimeStat runtimeStat) => new(runtimeStat);
    }

    public readonly struct RuntimeStatFluent
    {
        private readonly RuntimeStat _runtimeStat;

        public RuntimeStatFluent(RuntimeStat runtimeStat)
        {
            _runtimeStat = runtimeStat;
        }

        public T Data<T>() where T : RuntimeStatData => _runtimeStat.GetRuntimeData<T>();

        public bool TryData<T>(out T data) where T : RuntimeStatData => _runtimeStat.TryGetRuntimeData(out data);

        public RuntimeStatFluent Restore()
        {
            _runtimeStat.RestoreRuntimeData();
            return this;
        }

        public RuntimeStatFluent Persist()
        {
            _runtimeStat.PersistRuntimeData();
            return this;
        }
    }
}
