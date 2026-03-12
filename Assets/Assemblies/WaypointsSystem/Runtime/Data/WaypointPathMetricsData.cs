using System;
using Nody.Runtime.Core;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace ArmyClash.WaypointsSystem.Runtime.Spawning
{
    [Persistent]
    [Name("Waypoints/Spawning/Metrics Cache Data")]
    [Group("Waypoints")]
    public sealed class WaypointPathMetricsData : ComponentData
    {
        private static readonly float[] Empty = Array.Empty<float>();

        private float[] _cumulativeDistances = Empty;
        private float _totalLength;

        public float[] GetCumulativeDistances()
        {
            return _cumulativeDistances;
        }

        public float GetTotalLength()
        {
            return _totalLength;
        }

        public int SegmentCount => _cumulativeDistances.Length;

        public int FindSegmentIndex(float targetDistance)
        {
            if (_cumulativeDistances.Length == 0 || targetDistance <= 0f)
            {
                return 0;
            }

            int index = Array.BinarySearch(_cumulativeDistances, targetDistance);
            if (index >= 0)
            {
                return index;
            }

            index = ~index;
            if (index >= _cumulativeDistances.Length)
            {
                return _cumulativeDistances.Length - 1;
            }

            return index;
        }

        public float GetSegmentStartDistance(int segmentIndex)
        {
            if (segmentIndex <= 0 || _cumulativeDistances.Length == 0)
            {
                return 0f;
            }

            if (segmentIndex >= _cumulativeDistances.Length)
            {
                return _cumulativeDistances[_cumulativeDistances.Length - 1];
            }

            return _cumulativeDistances[segmentIndex - 1];
        }

        public float GetSegmentLength(int segmentIndex)
        {
            if (segmentIndex < 0 || segmentIndex >= _cumulativeDistances.Length)
            {
                return 0f;
            }

            float segmentEnd = _cumulativeDistances[segmentIndex];
            float segmentStart = GetSegmentStartDistance(segmentIndex);
            return segmentEnd - segmentStart;
        }

        public void Rebuild(WaypointPath path)
        {
            bool loop = path.GetData<LoopData>().Loop.Value;
            int segmentCount = path.GetData<WaypointPathSegmentCountData>().SegmentCount;
            int pointCount = path.Count;
            if (segmentCount <= 0)
            {
                SetMetrics(Empty, 0f);
                return;
            }

            float[] cumulativeDistances = new float[segmentCount];
            float totalLength = 0f;

            for (int i = 0; i < segmentCount; i++)
            {
                int fromIndex = i;
                int toIndex = loop ? (i + 1) % pointCount : i + 1;

                Vector3 from = path.GetWorldPoint(fromIndex);
                Vector3 to = path.GetWorldPoint(toIndex);

                totalLength += Vector3.Distance(from, to);
                cumulativeDistances[i] = totalLength;
            }

            SetMetrics(cumulativeDistances, totalLength);
        }

        public void SetMetrics(float[] cumulativeDistances, float totalLength)
        {
            float[] safeCumulativeDistances = cumulativeDistances ?? Empty;
            float safeTotalLength = totalLength < 0f ? 0f : totalLength;

            _cumulativeDistances = safeCumulativeDistances;
            _totalLength = safeTotalLength;
        }
    }
}
