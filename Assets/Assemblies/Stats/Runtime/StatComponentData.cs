using OdinSerializer;
using Stats.EntityDataActionIntegration;
using VladislavTsurikov.Nody.Runtime.Core;

namespace Stats.Runtime
{
    public abstract class StatComponentData : ComponentData
    {
        [OdinSerialize] private bool _save;

        public bool Save => _save;

        public void SetSave(bool save)
        {
            _save = save;
        }

        public abstract RuntimeStatData CreateRuntimeComponent();
    }
}
