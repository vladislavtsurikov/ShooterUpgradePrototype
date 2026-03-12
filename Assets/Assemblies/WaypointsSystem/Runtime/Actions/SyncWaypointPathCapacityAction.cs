using ArmyClash.WaypointsSystem.Runtime.Spawning;
using Nody.Runtime.Core;
using UniRx;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace ArmyClash.WaypointsSystem.Runtime.Actions
{
    [RequiresData(typeof(LoopData), typeof(CapacityData))]
    [Group("Waypoints")]
    [Name("Waypoints/Actions/Sync Loop Capacity")]
    public sealed class SyncWaypointPathCapacityAction : EntityMonoBehaviourAction
    {
        private CompositeDisposable _subscriptions = new();
        private LoopData _loopData;
        private CapacityData _capacityData;

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            _loopData = Entity.GetData<LoopData>();
            _capacityData = Entity.GetData<CapacityData>();

            _loopData.Loop
                .DistinctUntilChanged()
                .Subscribe(_ => ChangeLoopAndCapacityData())
                .AddTo(_subscriptions);

            ChangeLoopAndCapacityData();
        }

        protected override void OnDisable()
        {
            _subscriptions?.Clear();
        }

        private void ChangeLoopAndCapacityData()
        {
            bool loop = _loopData.Loop.Value;

            if (loop)
            {
                return;
            }

            _capacityData.Mode = CapacityData.CapacityMode.Limited;
            _capacityData.Capacity = 1;
        }
    }
}
