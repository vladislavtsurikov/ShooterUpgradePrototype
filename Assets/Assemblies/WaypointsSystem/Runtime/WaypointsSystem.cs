using System.Collections.Generic;
using UnityEngine;

namespace ArmyClash.WaypointsSystem.Runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("ArmyClash/Waypoints/Waypoints System")]
    public sealed class WaypointsSystem : MonoBehaviour
    {
        [SerializeField]
        private List<WaypointPath> _paths = new();

        public IReadOnlyList<WaypointPath> Paths => _paths;
        public int Count => _paths.Count;

        public void AddPath(WaypointPath path)
        {
            if (path == null || _paths.Contains(path))
            {
                return;
            }

            _paths.Add(path);
        }

        public void RemovePath(WaypointPath path)
        {
            if (path == null)
            {
                return;
            }

            _paths.Remove(path);
        }

        public void RebuildFromScene()
        {
            _paths.Clear();
            WaypointPath[] scenePaths = FindScenePaths();
            for (int i = 0; i < scenePaths.Length; i++)
            {
                AddPath(scenePaths[i]);
            }
        }

        private static WaypointPath[] FindScenePaths()
        {
#if UNITY_2023_1_OR_NEWER
            return FindObjectsByType<WaypointPath>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            return UnityEngine.Object.FindObjectsOfType<WaypointPath>();
#endif
        }

        private void OnValidate()
        {
            _paths ??= new List<WaypointPath>();
            _paths.RemoveAll(p => p == null);
        }
    }
}
