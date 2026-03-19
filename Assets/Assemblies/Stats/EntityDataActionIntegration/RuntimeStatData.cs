using OdinSerializer;

namespace Stats.EntityDataActionIntegration
{
    public abstract class RuntimeStatData
    {
        [OdinSerialize] private bool _save;
        [System.NonSerialized]
        private RuntimeStat _runtimeStat;

        protected RuntimeStatData()
        {
        }

        protected RuntimeStatData(bool save)
        {
            _save = save;
        }

        public bool ShouldSave => _save;

        public void Restore()
        {
            RestoreDefaultsValue();

            if (!ShouldSave || _runtimeStat == null)
            {
                return;
            }

            RestoreValue();
        }

        internal void BindRuntimeStat(RuntimeStat runtimeStat)
        {
            _runtimeStat = runtimeStat;
        }

        protected void Save()
        {
            if (!ShouldSave || _runtimeStat == null)
            {
                return;
            }

            SaveValue();
        }

        protected string StatId => _runtimeStat?.StatId;

        protected RuntimeStat RuntimeStat => _runtimeStat;

        protected virtual void RestoreDefaultsValue()
        {
        }

        protected virtual void RestoreValue()
        {
        }

        protected virtual void SaveValue()
        {
        }
    }
}
