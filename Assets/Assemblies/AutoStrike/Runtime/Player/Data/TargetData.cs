using OdinSerializer;
using UniRx;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Data
{
    [Name("AutoStrike/Data/Target")]
    public sealed class TargetData : ComponentData
    {
        [OdinSerialize]
        private ReactiveProperty<EntityMonoBehaviour> _target = new();

        public ReactiveProperty<EntityMonoBehaviour> Target
        {
            get
            {
                _target ??= new ReactiveProperty<EntityMonoBehaviour>();
                return _target;
            }
        }
    }
}
