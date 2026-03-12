using UnityEngine;

namespace AutoStrike.Config
{
    [CreateAssetMenu(menuName = "Configs/AutoStrike/EnemySpawnConfig", fileName = "EnemySpawnConfig")]
    public sealed class EnemySpawnConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Min(0f)]
        public float RespawnDelay { get; private set; } = 3f;

        [field: SerializeField]
        [field: Min(1)]
        public int MaxMobCount { get; private set; } = 10;

        public bool IsValid()
        {
            if (RespawnDelay <= 0)
            {
                Debug.LogError("RespawnDelay must be greater than 0");
                return false;
            }

            if (MaxMobCount <= 0)
            {
                Debug.Log("MaxMobCount must be greater than 0");
                return false;
            }

            return true;
        }
    }
}
