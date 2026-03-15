using UnityEngine;

namespace AutoStrike.Config
{
    [CreateAssetMenu(menuName = "Configs/ShooterUpgradePrototype/EnemySpawnConfig", fileName = "EnemySpawnConfig")]
    public sealed class EnemySpawnConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Min(0f)]
        public float RespawnDelay { get; private set; } = 3f;

        [field: SerializeField]
        [field: Min(1)]
        public int MaxMobCount { get; private set; } = 10;

        [field: SerializeField]
        [field: Min(1f)]
        public float MinStartingHealth { get; private set; } = 75f;

        [field: SerializeField]
        [field: Min(1f)]
        public float MaxStartingHealth { get; private set; } = 125f;

        [field: SerializeField]
        [field: Min(0)]
        public int KillRewardPoints { get; private set; } = 1;

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

            if (MinStartingHealth <= 0f)
            {
                Debug.LogError("MinStartingHealth must be greater than 0");
                return false;
            }

            if (MaxStartingHealth < MinStartingHealth)
            {
                Debug.LogError("MaxStartingHealth must be greater than or equal to MinStartingHealth");
                return false;
            }

            if (KillRewardPoints < 0)
            {
                Debug.LogError("KillRewardPoints must be greater than or equal to 0");
                return false;
            }

            return true;
        }
    }
}
