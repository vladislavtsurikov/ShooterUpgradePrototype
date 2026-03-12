using OdinSerializer;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    public abstract class RuntimeStatData
    {
        [OdinSerialize] private bool _save;

        protected RuntimeStatData()
        {
        }

        protected RuntimeStatData(bool save)
        {
            _save = save;
        }

        public bool Save => _save;

        public virtual void Restore(RuntimeStatBuildContext context)
        {
        }

        public virtual void Persist(RuntimeStatBuildContext context)
        {
        }
    }
}
