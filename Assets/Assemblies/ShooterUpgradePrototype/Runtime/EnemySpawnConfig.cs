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
        [field: Min(1)]
        public int MinStartingHealth { get; private set; } = 1;

        [field: SerializeField]
        [field: Min(1)]
        public int MaxStartingHealth { get; private set; } = 10;

        [field: SerializeField]
        [field: Min(0)]
        public int KillRewardPoints { get; private set; } = 1;

        public int GetRandomStartingHealth()
        {
            int minHealth = Mathf.Max(1, Mathf.Min(MinStartingHealth, MaxStartingHealth));
            int maxHealth = Mathf.Max(minHealth, Mathf.Max(MinStartingHealth, MaxStartingHealth));

            return Random.Range(minHealth, maxHealth + 1);
        }

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

            if (MinStartingHealth <= 0 || MaxStartingHealth <= 0)
            {
                Debug.LogError("Enemy starting health must be greater than 0");
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
