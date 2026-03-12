using System.Collections.Generic;

namespace ShooterUpgradePrototype.Progression.Models
{
    public sealed class PendingUpgradeState
    {
        private readonly Dictionary<string, int> _levels;

        public PendingUpgradeState(Dictionary<string, int> levels, int availablePoints)
        {
            _levels = levels ?? new Dictionary<string, int>();
            AvailablePoints = availablePoints;
        }

        public int AvailablePoints { get; private set; }

        public int GetLevel(string statId) => _levels.TryGetValue(statId, out int level) ? level : 0;

        public void SetLevel(string statId, int level) => _levels[statId] = level;

        public void SetAvailablePoints(int availablePoints) => AvailablePoints = availablePoints;
    }
}
