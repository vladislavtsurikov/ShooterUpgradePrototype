using AutoStrike.Entities;
using UniRx;

namespace AutoStrike.Services
{
    public sealed class EnemyRegistryService
    {
        private readonly ReactiveCollection<EnemyEntity> _enemies = new();

        public IReadOnlyReactiveCollection<EnemyEntity> Enemies => _enemies;

        public void Register(EnemyEntity enemy)
        {
            if (_enemies.Contains(enemy))
            {
                return;
            }

            _enemies.Add(enemy);
        }

        public void Unregister(EnemyEntity enemy)
        {
            _enemies.Remove(enemy);
        }
    }
}
