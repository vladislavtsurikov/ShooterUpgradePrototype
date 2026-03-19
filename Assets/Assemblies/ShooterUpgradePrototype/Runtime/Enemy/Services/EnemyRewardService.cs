using UniRx;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    public sealed class EnemyRewardService
    {
        private readonly PlayerStatsService _playerStatsService;

        public EnemyRewardService(PlayerStatsService playerStatsService)
        {
            _playerStatsService = playerStatsService;
        }

        public IReadOnlyReactiveProperty<int> AvailablePoints => _playerStatsService.AvailableExp;

        public void Grant(int points)
        {
            _playerStatsService.AddExp(points);
        }

        public bool TrySpend(int points)
        {
            return _playerStatsService.TrySpendExp(points);
        }

        public void Reset()
        {
        }
    }
}
