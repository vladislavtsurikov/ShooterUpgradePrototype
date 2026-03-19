using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace WaypointsSystem.Runtime
{
    public static class WaypointPathExtensions
    {
        public static bool TryMoveAlongPath(this WaypointPath path, EntityMonoBehaviour entity, float deltaNormalized)
        {
            if (path == null || entity == null || path.Count <= 0)
            {
                return false;
            }

            Transform targetTransform = entity.transform;
            if (!WaypointPathNormalizedPositionUtility.TryGetNormalizedPosition(path, targetTransform.position, out float currentNormalized))
            {
                return false;
            }

            WaypointPathDirectionData waypointPathDirectionData = entity.GetData<WaypointPathDirectionData>();
            bool loop = path.GetData<LoopData>().Loop.Value;
            int requestedDirection = waypointPathDirectionData.Direction >= 0 ? 1 : -1;

            float nextNormalized = WaypointPathNormalizedMovementUtility.GetNextNormalized(
                currentNormalized,
                Mathf.Abs(deltaNormalized),
                loop,
                requestedDirection,
                out int nextDirection);

            waypointPathDirectionData.Direction = nextDirection;

            Vector3 nextPosition = WaypointPathNormalizedPositionUtility.GetWorldPoint(path, nextNormalized);
            nextPosition.y = targetTransform.position.y;
            targetTransform.position = nextPosition;
            return true;
        }
    }
}
