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
    }
}
