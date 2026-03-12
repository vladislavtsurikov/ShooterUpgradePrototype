using System;
using System.Collections.Generic;
using ArmyClash.WaypointsSystem.Runtime.Spawning;
using OdinSerializer;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace ArmyClash.WaypointsSystem.Runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("ArmyClash/Waypoints/Waypoint Path")]
    public sealed class WaypointPath : EntityMonoBehaviour
    {
        private static readonly string[] WaypointsGroup = { "Waypoints" };

        [SerializeField]
        [HideInInspector]
        [OdinSerialize]
        private ReactiveCollection<Vector3> _localPoints = new();

        private ReactiveCollection<Vector3> LocalPointsCollection => _localPoints ??= new ReactiveCollection<Vector3>();

        public int Count => LocalPointsCollection.Count;

        public IObservable<Unit> PointsChanged => Observable.Merge(
            LocalPointsCollection.ObserveAdd().AsUnitObservable(),
            LocalPointsCollection.ObserveRemove().AsUnitObservable(),
            LocalPointsCollection.ObserveReplace().AsUnitObservable(),
            LocalPointsCollection.ObserveReset().AsUnitObservable());

        protected override Type[] ComponentDataTypesToCreate() =>
            new[]
            {
                typeof(LoopData),
                typeof(CapacityData),
                typeof(WaypointPathMetricsData),
                typeof(WaypointPathSegmentCountData)
            };

        protected override Type[] ActionTypesToCreate() =>
            new[]
            {
                typeof(Actions.SyncWaypointPathCapacityAction),
                typeof(Actions.RebuildWaypointPathSegmentCountAction),
                typeof(Actions.RebuildWaypointPathMetricsCacheAction)
            };

        protected override void BeforeOnSetupEntity()
        {
            Data.SetAllowedGroupAttributes(WaypointsGroup);
            Actions.SetAllowedGroupAttributes(WaypointsGroup);
        }

        public Vector3 GetWorldPoint(int index)
        {
            return transform.TransformPoint(LocalPointsCollection[index]);
        }

        public Vector3 GetLocalPoint(int index)
        {
            return LocalPointsCollection[index];
        }

        public void SetLocalPoint(int index, Vector3 value)
        {
            LocalPointsCollection[index] = value;
        }

        public void AddPoint(Vector3 localPoint)
        {
            LocalPointsCollection.Add(localPoint);
        }

        public void RemovePoint(int index)
        {
            LocalPointsCollection.RemoveAt(index);
        }

        public bool IsValid()
        {
            return LocalPointsCollection.Count != 0;
        }
    }
}
