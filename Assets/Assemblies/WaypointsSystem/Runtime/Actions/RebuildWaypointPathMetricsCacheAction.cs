using ArmyClash.WaypointsSystem.Runtime.Spawning;
using Nody.Runtime.Core;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace ArmyClash.WaypointsSystem.Runtime.Actions
{
    [RequiresData(typeof(WaypointPathMetricsData), typeof(LoopData))]
    [Group("Waypoints")]
    [Name("Waypoints/Actions/Rebuild Metrics Cache")]
    public sealed class RebuildWaypointPathMetricsCacheAction : EntityMonoBehaviourAction
    {
        private CompositeDisposable _subscriptions = new();

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private Vector3 _lastLossyScale;

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            WaypointPath path = (WaypointPath)EntityMonoBehaviour;
            WaypointPathMetricsData metricsData = Entity.GetData<WaypointPathMetricsData>();
            LoopData loopData = Entity.GetData<LoopData>();

            metricsData.Rebuild(path);
            CaptureTransform(path.transform);

            loopData.Loop
                .DistinctUntilChanged()
                .Subscribe(_ => metricsData.Rebuild(path))
                .AddTo(_subscriptions);

            path.PointsChanged
                .Subscribe(_ => metricsData.Rebuild(path))
                .AddTo(_subscriptions);

            Observable.EveryLateUpdate()
                .Where(_ => HasTransformChanged(path.transform))
                .Subscribe(_ =>
                {
                    metricsData.Rebuild(path);
                    CaptureTransform(path.transform);
                })
                .AddTo(_subscriptions);
        }

        protected override void OnDisable()
        {
            _subscriptions?.Clear();
        }

        private bool HasTransformChanged(Transform transform)
        {
            return transform.position != _lastPosition ||
                   transform.rotation != _lastRotation ||
                   transform.lossyScale != _lastLossyScale;
        }

        private void CaptureTransform(Transform transform)
        {
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
            _lastLossyScale = transform.lossyScale;
        }
    }
}
