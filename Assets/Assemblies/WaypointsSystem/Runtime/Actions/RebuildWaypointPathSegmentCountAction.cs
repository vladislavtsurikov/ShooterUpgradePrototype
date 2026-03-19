using Nody.Runtime.Core;
using UniRx;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace WaypointsSystem.Runtime
{
    [RequiresData(typeof(WaypointPathSegmentCountData), typeof(LoopData))]
    [Group("Waypoints")]
    [Name("Waypoints/Actions/Rebuild Segment Count")]
    public sealed class RebuildWaypointPathSegmentCountAction : EntityMonoBehaviourAction
    {
        private CompositeDisposable _subscriptions = new();

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            WaypointPath path = (WaypointPath)EntityMonoBehaviour;
            WaypointPathSegmentCountData segmentCountData = Entity.GetData<WaypointPathSegmentCountData>();
            LoopData loopData = Entity.GetData<LoopData>();

            segmentCountData.Rebuild(path);

            loopData.Loop
                .DistinctUntilChanged()
                .Subscribe(_ => segmentCountData.Rebuild(path))
                .AddTo(_subscriptions);

            path.PointsChanged
                .Subscribe(_ => segmentCountData.Rebuild(path))
                .AddTo(_subscriptions);
        }

        protected override void OnDisable()
        {
            _subscriptions?.Clear();
        }
    }
}
