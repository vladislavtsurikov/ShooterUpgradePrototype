using OdinSerializer;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
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
            RestoreDefaultsInternal();

            if (!ShouldSave || _runtimeStat == null)
            {
                return;
            }

            RestoreInternal();
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

            SaveInternal();
        }

        protected string StatId => _runtimeStat?.StatId;

        protected RuntimeStat RuntimeStat => _runtimeStat;

        protected virtual void RestoreDefaultsInternal()
        {
        }

        protected virtual void RestoreInternal()
        {
        }

        protected virtual void SaveInternal()
        {
        }
    }
}
