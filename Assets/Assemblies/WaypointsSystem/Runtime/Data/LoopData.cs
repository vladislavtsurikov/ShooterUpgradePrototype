using Nody.Runtime.Core;
using OdinSerializer;
using UniRx;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace WaypointsSystem.Runtime
{
    [Persistent]
    [Name("Waypoints/Spawning/Loop Data")]
    [Group("Waypoints")]
    public sealed class LoopData : ComponentData
    {
        [OdinSerialize]
        private ReactiveProperty<bool> _loop = new(true);

        public ReactiveProperty<bool> Loop
        {
            get
            {
                _loop ??= new ReactiveProperty<bool>(true);
                return _loop;
            }
        }
    }
}
