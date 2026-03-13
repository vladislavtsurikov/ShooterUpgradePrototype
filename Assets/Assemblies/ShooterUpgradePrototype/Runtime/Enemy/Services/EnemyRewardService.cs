using UniRx;

namespace ShooterUpgradePrototype.Enemy.Services
{
    public sealed class EnemyRewardService
    {
        private readonly ReactiveProperty<int> _availablePoints = new(0);

        public IReadOnlyReactiveProperty<int> AvailablePoints => _availablePoints;

        public void Grant(int points)
        {
            if (points <= 0)
            {
                return;
            }

            _availablePoints.Value += points;
        }

        public bool TrySpend(int points)
        {
            if (points <= 0)
            {
                return true;
            }

            if (_availablePoints.Value < points)
            {
                return false;
            }

            _availablePoints.Value -= points;
            return true;
        }

        public void Reset() => _availablePoints.Value = 0;
    }
}
