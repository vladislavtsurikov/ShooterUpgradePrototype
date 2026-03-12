using ArmyClash.WaypointsSystem.Runtime.Spawning;
using UnityEngine;

namespace ArmyClash.WaypointsSystem.Runtime
{
    public static class WaypointPathNormalizedPositionUtility
    {
        private readonly struct PathMetricsContext
        {
            public readonly WaypointPath Path;
            public readonly WaypointPathMetricsData MetricsData;
            public readonly int PointCount;
            public readonly int SegmentCount;
            public readonly float TotalLength;
            public readonly bool IsLoop;

            public PathMetricsContext(WaypointPath path, WaypointPathMetricsData metricsData, int pointCount, int segmentCount, float totalLength)
            {
                Path = path;
                MetricsData = metricsData;
                PointCount = pointCount;
                SegmentCount = segmentCount;
                TotalLength = totalLength;
                IsLoop = pointCount > 1 && segmentCount == pointCount;
            }

            public static bool TryBuildContext(WaypointPath path, out PathMetricsContext context)
            {
                WaypointPathMetricsData metricsData = path.GetData<WaypointPathMetricsData>();
                float totalLength = metricsData.GetTotalLength();
                int pointCount = path.Count;
                int segmentCount = metricsData.SegmentCount;

                if (segmentCount <= 0)
                {
                    context = default;
                    return false;
                }

                context = new PathMetricsContext(path, metricsData, pointCount, segmentCount, totalLength);
                return true;
            }
        }

        public static Vector3 GetWorldPoint(WaypointPath path, float normalizedPosition)
        {
            if (path == null || path.Count == 0)
            {
                return path != null ? path.transform.position : Vector3.zero;
            }

            if (path.Count == 1)
            {
                return path.GetWorldPoint(0);
            }

            if (!PathMetricsContext.TryBuildContext(path, out PathMetricsContext context))
            {
                return path.GetWorldPoint(0);
            }

            if (context.TotalLength <= Mathf.Epsilon)
            {
                return path.GetWorldPoint(0);
            }

            float normalized = Normalize(normalizedPosition, context.IsLoop);
            float targetDistance = context.TotalLength * normalized;

            int segmentIndex = context.MetricsData.FindSegmentIndex(targetDistance);
            float segmentStartDistance = context.MetricsData.GetSegmentStartDistance(segmentIndex);
            float segmentLength = context.MetricsData.GetSegmentLength(segmentIndex);

            GetSegmentPoints(context, segmentIndex, out Vector3 from, out Vector3 to);
            if (segmentLength <= Mathf.Epsilon)
            {
                return from;
            }

            float t = Mathf.Clamp01((targetDistance - segmentStartDistance) / segmentLength);
            return Vector3.Lerp(from, to, t);
        }

        public static bool TryGetSegmentPoints(WaypointPath path, float normalizedPosition, out Vector3 from, out Vector3 to)
        {
            from = Vector3.zero;
            to = Vector3.zero;

            if (path == null || path.Count <= 1)
            {
                return false;
            }

            if (!PathMetricsContext.TryBuildContext(path, out PathMetricsContext context))
            {
                return false;
            }

            if (context.TotalLength <= Mathf.Epsilon)
            {
                return false;
            }

            float normalized = Normalize(normalizedPosition, context.IsLoop);
            float targetDistance = context.TotalLength * normalized;
            int segmentIndex = context.MetricsData.FindSegmentIndex(targetDistance);
            GetSegmentPoints(context, segmentIndex, out from, out to);
            return true;
        }

        public static bool TryGetSegmentPointsByWorldPosition(WaypointPath path, Vector3 worldPosition, out Vector3 from, out Vector3 to)
        {
            from = Vector3.zero;
            to = Vector3.zero;

            if (!TryGetNormalizedPosition(path, worldPosition, out float normalizedPosition))
            {
                return false;
            }

            return TryGetSegmentPoints(path, normalizedPosition, out from, out to);
        }

        public static bool TryGetNormalizedPosition(WaypointPath path, Vector3 worldPosition, out float normalizedPosition)
        {
            if (path == null || path.Count == 0)
            {
                normalizedPosition = 0f;
                return false;
            }

            if (path.Count == 1)
            {
                normalizedPosition = 0f;
                return true;
            }

            if (!PathMetricsContext.TryBuildContext(path, out PathMetricsContext context))
            {
                normalizedPosition = 0f;
                return false;
            }

            if (context.TotalLength <= Mathf.Epsilon)
            {
                normalizedPosition = 0f;
                return true;
            }

            float closestDistanceSqr = float.MaxValue;
            float closestDistanceAlongPath = 0f;

            for (int segmentIndex = 0; segmentIndex < context.SegmentCount; segmentIndex++)
            {
                GetSegmentPoints(context, segmentIndex, out Vector3 from, out Vector3 to);

                Vector3 segment = to - from;
                float segmentLengthSqr = segment.sqrMagnitude;
                float t = segmentLengthSqr <= Mathf.Epsilon
                    ? 0f
                    : Mathf.Clamp01(Vector3.Dot(worldPosition - from, segment) / segmentLengthSqr);

                Vector3 projectedPoint = from + segment * t;
                float distanceSqr = (worldPosition - projectedPoint).sqrMagnitude;
                if (distanceSqr >= closestDistanceSqr)
                {
                    continue;
                }

                closestDistanceSqr = distanceSqr;
                float segmentStartDistance = context.MetricsData.GetSegmentStartDistance(segmentIndex);
                float segmentLength = context.MetricsData.GetSegmentLength(segmentIndex);
                closestDistanceAlongPath = segmentStartDistance + segmentLength * t;
            }

            normalizedPosition = Normalize(closestDistanceAlongPath / context.TotalLength, context.IsLoop);
            return true;
        }

        private static float Normalize(float normalizedPosition, bool loop)
        {
            return loop
                ? Mathf.Repeat(normalizedPosition, 1f)
                : Mathf.Clamp01(normalizedPosition);
        }

        private static void GetSegmentPoints(PathMetricsContext context, int segmentIndex, out Vector3 from, out Vector3 to)
        {
            int fromIndex = segmentIndex;
            int toIndex = context.IsLoop
                ? (segmentIndex + 1) % context.PointCount
                : segmentIndex + 1;

            from = context.Path.GetWorldPoint(fromIndex);
            to = context.Path.GetWorldPoint(toIndex);
        }
    }
}
