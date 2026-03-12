using UniRx;

namespace AutoStrike.Services
{
    public sealed class KillCounterService
    {
        private readonly ReactiveProperty<int> _kills = new(0);

        public IReadOnlyReactiveProperty<int> Kills => _kills;


        public void Increment() => _kills.Value++;

        public void Reset() => _kills.Value = 0;
    }
}
