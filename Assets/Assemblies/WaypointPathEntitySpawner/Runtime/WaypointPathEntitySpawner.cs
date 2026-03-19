using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using WaypointsSystem.Runtime;
using Zenject;

namespace AutoStrike.Spawning.WaypointPathEntitySpawner.Runtime
{
    [DisallowMultipleComponent]
    public class WaypointPathEntitySpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _enemyPrefab;

        [SerializeField]
        private float _spawnHeightOffset;

        [Inject]
        private WaypointsSystem.Runtime.WaypointsSystem _waypoints;

        protected virtual void OnAfterSpawn(GameObject instance, WaypointPath path, float normalizedPosition)
        {
        }

        public void SpawnMaxCountEntity(int currentCount, int maxMobCount)
        {
            int countToSpawn = maxMobCount - currentCount;

            for (int i = 0; i < countToSpawn; i++)
            {
                if (!SpawnOne())
                {
                    break;
                }
            }
        }

        public bool SpawnOne()
        {
            if (_enemyPrefab == null || _waypoints == null || _waypoints.Count == 0)
            {
                return false;
            }

            if (!TryFindWaypointPathForSpawn(out WaypointPath bestPath, out List<EntityMonoBehaviour> bestPathEntities))
            {
                return false;
            }

            float spawnT = FindSpawnPosition(bestPath, bestPathEntities);
            Vector3 spawnPosition = WaypointPathNormalizedPositionUtility.GetWorldPoint(bestPath, spawnT);
            spawnPosition.y += _spawnHeightOffset;

            GameObject instance = Instantiate(_enemyPrefab);
            instance.transform.position = spawnPosition;

            OnAfterSpawn(instance, bestPath, spawnT);
            return true;
        }

        private bool TryFindWaypointPathForSpawn(out WaypointPath bestPath, out List<EntityMonoBehaviour> bestPathEntities)
        {
            bestPath = null;
            bestPathEntities = null;

            for (int i = 0; i < _waypoints.Count; i++)
            {
                WaypointPath path = _waypoints.Paths[i];
                if (path == null || !path.IsValid())
                {
                    continue;
                }

                CapacityData capacityData = path.Data.GetElement<CapacityData>();
                List<EntityMonoBehaviour> pathEntities = GetPathEntities(path);
                int currentCount = pathEntities.Count;
                if (capacityData != null &&
                    capacityData.Mode == CapacityData.CapacityMode.Limited &&
                    currentCount >= capacityData.Capacity)
                {
                    continue;
                }

                if (currentCount == 0)
                {
                    bestPath = path;
                    bestPathEntities = pathEntities;
                    return true;
                }

                if (bestPath == null)
                {
                    bestPath = path;
                    bestPathEntities = pathEntities;
                    continue;
                }

                if (bestPathEntities.Count > currentCount)
                {
                    bestPath = path;
                    bestPathEntities = pathEntities;
                }
            }

            return bestPath != null && bestPathEntities != null;
        }

        private static List<EntityMonoBehaviour> GetPathEntities(WaypointPath path)
        {
            List<EntityMonoBehaviour> pathEntities = new();
            EntityMonoBehaviour[] entities = FindObjectsByType<EntityMonoBehaviour>(FindObjectsSortMode.None);

            for (int i = 0; i < entities.Length; i++)
            {
                EntityMonoBehaviour entity = entities[i];
                if (entity == null)
                {
                    continue;
                }

                WaypointPathData pathData = entity.Data.GetElement<WaypointPathData>();
                if (pathData == null || pathData.Path != path)
                {
                    continue;
                }

                pathEntities.Add(entity);
            }

            return pathEntities;
        }

        private static float FindSpawnPosition(WaypointPath path, List<EntityMonoBehaviour> pathEntities)
        {
            List<float> occupiedPositions = new();

            for (int i = 0; i < pathEntities.Count; i++)
            {
                EntityMonoBehaviour entity = pathEntities[i];
                if (entity == null)
                {
                    continue;
                }

                if (WaypointPathNormalizedPositionUtility.TryGetNormalizedPosition(path, entity.transform.position, out float normalizedPosition))
                {
                    occupiedPositions.Add(Mathf.Repeat(normalizedPosition, 1f));
                }
            }

            if (occupiedPositions.Count == 0)
            {
                return 0f;
            }

            occupiedPositions.Sort();

            float bestGap = -1f;
            float bestStart = occupiedPositions[0];
            int count = occupiedPositions.Count;

            for (int i = 0; i < count; i++)
            {
                float start = occupiedPositions[i];
                float end = i == count - 1
                    ? occupiedPositions[0] + 1f
                    : occupiedPositions[i + 1];

                float gap = end - start;
                if (gap > bestGap)
                {
                    bestGap = gap;
                    bestStart = start;
                }
            }

            return Mathf.Repeat(bestStart + bestGap * 0.5f, 1f);
        }
    }
}
