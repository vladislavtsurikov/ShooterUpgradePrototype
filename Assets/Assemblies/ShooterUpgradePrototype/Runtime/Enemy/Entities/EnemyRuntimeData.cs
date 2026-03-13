using OdinSerializer;
using UniRx;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace ShooterUpgradePrototype.Enemy.Entities
{
    [Name("ShooterUpgradePrototype/Enemy/Data/Runtime")]
    public sealed class EnemyRuntimeData : ComponentData
    {
        [OdinSerialize]
        private ReactiveProperty<bool> _isDead = new(false);

        [OdinSerialize]
        private ReactiveProperty<float> _spawnedMaxHealth = new(0f);

        [OdinSerialize]
        private ReactiveProperty<int> _killRewardPoints = new(1);

        public ReactiveProperty<bool> IsDead
        {
            get
            {
                _isDead ??= new ReactiveProperty<bool>(false);
                return _isDead;
            }
        }

        public ReactiveProperty<float> SpawnedMaxHealth
        {
            get
            {
                _spawnedMaxHealth ??= new ReactiveProperty<float>(0f);
                return _spawnedMaxHealth;
            }
        }

        public ReactiveProperty<int> KillRewardPoints
        {
            get
            {
                _killRewardPoints ??= new ReactiveProperty<int>(1);
                return _killRewardPoints;
            }
        }

        public void Initialize(float spawnedMaxHealth, int killRewardPoints)
        {
            SpawnedMaxHealth.Value = Mathf.Max(0f, spawnedMaxHealth);
            KillRewardPoints.Value = Mathf.Max(0, killRewardPoints);
            IsDead.Value = false;
            MarkDirty();
        }

        public bool TryMarkDead()
        {
            if (IsDead.Value)
            {
                return false;
            }

            IsDead.Value = true;
            MarkDirty();
            return true;
        }
    }
}
