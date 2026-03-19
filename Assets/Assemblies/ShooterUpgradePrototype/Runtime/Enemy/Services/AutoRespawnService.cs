using System;
using System.Threading;
using AutoStrike.Spawning.WaypointPathEntitySpawner.Runtime;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime;
using UniRx;

namespace ShooterUpgradePrototype.Runtime
{
    public sealed class AutoRespawnService : IDisposable
    {
        private readonly EnemyRegistryService _registry;
        private readonly EnemySpawnConfig _config;
        private readonly WaypointPathEntitySpawner _spawner;
        private readonly CompositeDisposable _subscriptions = new();

        private CancellationTokenSource _cts;

        public AutoRespawnService(
            EnemyRegistryService registry,
            EnemySpawnConfig config,
            WaypointPathEntitySpawner spawner)
        {
            _registry = registry;
            _config = config;
            _spawner = spawner;

            _registry.Enemies
                .ObserveCountChanged()
                .Where(count => count < _config.MaxMobCount)
                .Subscribe(_ => Run())
                .AddTo(_subscriptions);

            _registry.Enemies
                .ObserveCountChanged()
                .Where(count => count >= _config.MaxMobCount)
                .Subscribe(_ => Stop())
                .AddTo(_subscriptions);
        }

        public void Dispose()
        {
            _subscriptions?.Dispose();
            Stop();
        }

        public void Run()
        {
            if (_registry.Enemies.Count >= _config.MaxMobCount)
            {
                return;
            }

            if (_cts != null)
            {
                return;
            }

            _cts = new CancellationTokenSource();
            Run(_cts.Token).Forget();
        }

        private void Stop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async UniTaskVoid Run(CancellationToken token)
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_config.RespawnDelay), cancellationToken: token);

                _spawner.SpawnOne();

                if (_registry.Enemies.Count >= _config.MaxMobCount)
                {
                    Stop();
                    return;
                }
            }
        }
    }
}
