using System;
using System.Collections.Generic;
using System.Linq;
using Stats.EntityDataActionIntegration;
using Stats.Runtime.StatComponents;
using UniRx;
using UnityEngine;

namespace ShooterUpgradePrototype.Runtime
{
    public sealed class PlayerStatsService : IDisposable
    {
        public const string ExpStatId = "EXP";

        private readonly StatsEntityConfig _statsConfig;
        private readonly ReactiveProperty<int> _availableExp = new(0);
        private readonly CompositeDisposable _disposables = new();

        [NonSerialized] private int _previousLevel;

        public PlayerStatsService(StatsEntityConfig statsConfig)
        {
            _statsConfig = statsConfig ?? throw new ArgumentNullException(nameof(statsConfig));

            _statsConfig.Stats.EnsureInitialized();
            BindExp();
        }

        public StatsEntityState Stats => _statsConfig.Stats;
        public IReadOnlyReactiveProperty<int> AvailableExp => _availableExp;

        public void Dispose() => _disposables.Dispose();

        public IReadOnlyList<string> GetUpgradeWindowStatIds()
        {
            var statIds = new List<(string Id, int Order)>();

            foreach (KeyValuePair<string, RuntimeStat> pair in Stats.Stats)
            {
                UpgradeWindowStatComponent component =
                    pair.Value?.Stat?.ComponentStack?.GetElement<UpgradeWindowStatComponent>();

                if (component == null)
                {
                    continue;
                }

                statIds.Add((pair.Key, component.Order));
            }

            return statIds
                .OrderBy(item => item.Order)
                .ThenBy(item => item.Id, StringComparer.Ordinal)
                .Select(item => item.Id)
                .ToList();
        }

        public int GetAppliedLevel(string statId)
        {
            return GetLevelData(statId).AppliedLevel.Value;
        }

        public int GetMaxLevel(string statId)
        {
            return GetLevelData(statId).LevelProgressionTable.MaxLevel;
        }

        public float GetCurrentValue(string statId) => GetValueData(statId).CurrentValue;

        public float GetCumulativeValue(string statId, int level)
        {
            return GetLevelData(statId).LevelProgressionTable.GetCumulativeValue(level);
        }

        public float GetCurrentMaxValue(string statId)
        {
            RuntimeStat runtimeStat = GetRuntimeStat(statId);
            return runtimeStat.Runtime().Data<RuntimeStatValueData>().MaxValue;
        }

        public ReactiveProperty<float> GetValueProperty(string statId) => GetValueData(statId).Value;

        public ReactiveProperty<int> GetLevelProperty(string statId)
        {
            return GetLevelData(statId).AppliedLevel;
        }

        public bool AddExp(int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            return GetValueData(ExpStatId).AddValue(amount);
        }

        public bool TrySpendExp(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (_availableExp.Value < amount)
            {
                return false;
            }

            return GetValueData(ExpStatId).AddValue(-amount);
        }

        public bool CanUpgrade(string statId, int levelDelta = 1)
        {
            if (levelDelta <= 0)
            {
                return true;
            }

            if (_availableExp.Value < levelDelta)
            {
                return false;
            }

            RuntimeStatLevelData levelData = GetLevelData(statId);
            int targetLevel = levelData.AppliedLevel.Value + levelDelta;
            return targetLevel <= levelData.LevelProgressionTable.MaxLevel;
        }

        public bool TryApplyUpgrades(IReadOnlyDictionary<string, int> upgrades)
        {
            if (upgrades == null || upgrades.Count == 0)
            {
                return false;
            }

            int totalCost = 0;
            foreach (KeyValuePair<string, int> pair in upgrades)
            {
                if (pair.Value <= 0)
                {
                    continue;
                }

                if (!CanUpgradeIgnoringExp(pair.Key, pair.Value))
                {
                    return false;
                }

                totalCost += pair.Value;
            }

            if (totalCost <= 0 || _availableExp.Value < totalCost)
            {
                return false;
            }

            if (!TrySpendExp(totalCost))
            {
                return false;
            }

            foreach (KeyValuePair<string, int> pair in upgrades)
            {
                if (pair.Value <= 0)
                {
                    continue;
                }

                RuntimeStatLevelData levelData = GetLevelData(pair.Key);
                levelData.SetLevel(levelData.AppliedLevel.Value + pair.Value);
            }

            return true;
        }

        private void BindExp()
        {
            _disposables.Clear();

            RuntimeStatValueData expData = GetValueData(ExpStatId);
            expData.Value
                .Select(value => Mathf.Max(0, Mathf.FloorToInt(value)))
                .Subscribe(value => _availableExp.Value = value)
                .AddTo(_disposables);
        }

        private bool CanUpgradeIgnoringExp(string statId, int levelDelta)
        {
            if (levelDelta <= 0)
            {
                return true;
            }

            RuntimeStatLevelData levelData = GetLevelData(statId);
            int targetLevel = levelData.AppliedLevel.Value + levelDelta;
            return targetLevel <= levelData.LevelProgressionTable.MaxLevel;
        }

        private RuntimeStat GetRuntimeStat(string statId)
        {
            if (string.IsNullOrEmpty(statId))
            {
                throw new ArgumentException("Stat id is null or empty.", nameof(statId));
            }

            if (!Stats.Stats.TryGetValue(statId, out RuntimeStat runtimeStat))
            {
                throw new KeyNotFoundException($"Player stat `{statId}` was not found.");
            }

            return runtimeStat;
        }

        private RuntimeStatValueData GetValueData(string statId)
        {
            RuntimeStat runtimeStat = GetRuntimeStat(statId);

            if (!runtimeStat.Runtime().TryData(out RuntimeStatValueData valueData))
            {
                throw new InvalidOperationException($"Stat `{statId}` does not contain RuntimeStatValueData.");
            }

            return valueData;
        }

        private RuntimeStatLevelData GetLevelData(string statId)
        {
            if (!TryGetLevelData(statId, out RuntimeStatLevelData levelData))
            {
                throw new InvalidOperationException($"Stat `{statId}` does not contain RuntimeStatLevelData.");
            }

            return levelData;
        }

        private bool TryGetLevelData(string statId, out RuntimeStatLevelData levelData)
        {
            RuntimeStat runtimeStat = GetRuntimeStat(statId);
            return runtimeStat.Runtime().TryData(out levelData);
        }
    }
}
