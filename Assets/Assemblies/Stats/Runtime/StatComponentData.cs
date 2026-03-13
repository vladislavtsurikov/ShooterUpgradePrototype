using OdinSerializer;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
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
