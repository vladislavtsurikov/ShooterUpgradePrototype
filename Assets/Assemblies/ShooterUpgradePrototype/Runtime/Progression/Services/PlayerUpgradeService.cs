using System;
using System.Collections.Generic;
using ShooterUpgradePrototype.Progression.Configs;
using ShooterUpgradePrototype.Progression.Models;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;

namespace ShooterUpgradePrototype.Progression.Services
{
    public sealed class PlayerUpgradeService : IDisposable
    {
        private const string HealthId = "HP";
        private const string UpgradePointsKey = "UpgradePoints";

        private readonly ReactiveProperty<float> _currentHealth = new(0f);
        private readonly ReactiveProperty<float> _maxHealth = new(0f);
        private readonly List<UpgradeTrackConfig> _tracks = new();

        private IDisposable _healthSubscription;
        private EntityMonoBehaviour _playerEntity;
        private StatsEntityData _statsEntityData;

        public IReadOnlyReactiveProperty<float> CurrentHealth
        {
            get
            {
                TryResolvePlayer();
                return _currentHealth;
            }
        }

        public IReadOnlyReactiveProperty<float> MaxHealth
        {
            get
            {
                TryResolvePlayer();
                return _maxHealth;
            }
        }

        public IReadOnlyList<UpgradeTrackConfig> Tracks
        {
            get
            {
                TryResolvePlayer();
                return _tracks;
            }
        }

        public PendingUpgradeState CreateDraft()
        {
            TryResolvePlayer();

            var levels = new Dictionary<string, int>();
            foreach (UpgradeTrackConfig track in _tracks)
            {
                levels[track.Id] = GetAppliedLevel(track.Id);
            }

            return new PendingUpgradeState(levels, PlayerPrefs.GetInt(UpgradePointsKey, 0));
        }

        public void ApplyDraft(PendingUpgradeState draft)
        {
            if (draft == null)
            {
                return;
            }

            TryResolvePlayer();
            if (_statsEntityData == null)
            {
                return;
            }

            bool changed = false;
            foreach (UpgradeTrackConfig track in _tracks)
            {
                int draftLevel = draft.GetLevel(track.Id);
                if (_statsEntityData.SetStatLevelById(track.Id, draftLevel))
                {
                    changed = true;
                }
            }

            PlayerPrefs.SetInt(UpgradePointsKey, draft.AvailablePoints);
            PlayerPrefs.Save();

            if (changed)
            {
                _playerEntity?.GetAction<ApplyStatLevelsByTableAction>()?.ApplyLevels();
                RefreshHealthBinding();
            }
        }

        public bool HasPendingChanges(PendingUpgradeState draft)
        {
            if (draft == null)
            {
                return false;
            }

            TryResolvePlayer();
            foreach (UpgradeTrackConfig track in _tracks)
            {
                if (draft.GetLevel(track.Id) != GetAppliedLevel(track.Id))
                {
                    return true;
                }
            }

            return false;
        }

        public int GetAppliedLevel(string statId)
        {
            TryResolvePlayer();
            return _statsEntityData?.GetStatLevelById(statId) ?? 0;
        }

        public bool TryIncrementDraft(PendingUpgradeState draft, string statId)
        {
            if (draft == null || draft.AvailablePoints <= 0)
            {
                return false;
            }

            TryResolvePlayer();
            UpgradeTrackConfig track = FindTrack(statId);
            if (track == null)
            {
                return false;
            }

            int draftLevel = draft.GetLevel(statId);
            if (draftLevel >= track.MaxLevel)
            {
                return false;
            }

            draft.SetLevel(statId, draftLevel + 1);
            draft.SetAvailablePoints(draft.AvailablePoints - 1);
            return true;
        }

        public void Dispose()
        {
            _healthSubscription?.Dispose();
            _healthSubscription = null;
        }

        private UpgradeTrackConfig FindTrack(string statId)
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].Id == statId)
                {
                    return _tracks[i];
                }
            }

            return null;
        }

        private void TryResolvePlayer()
        {
            if (_playerEntity != null && _statsEntityData != null)
            {
                return;
            }

            EntityMonoBehaviour[] entities = Object.FindObjectsByType<EntityMonoBehaviour>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

            for (int i = 0; i < entities.Length; i++)
            {
                StatsEntityData statsEntityData = entities[i].Data.GetElement<StatsEntityData>();
                if (statsEntityData == null || !HasUpgradeableStats(statsEntityData))
                {
                    continue;
                }

                _playerEntity = entities[i];
                _statsEntityData = statsEntityData;
                RebuildTracks();
                RefreshHealthBinding();
                return;
            }
        }

        private static bool HasUpgradeableStats(StatsEntityData statsEntityData)
        {
            foreach (RuntimeStat runtimeStat in statsEntityData.Stats.Values)
            {
                if (runtimeStat.Runtime().TryData(out RuntimeStatLevelData _))
                {
                    return true;
                }
            }

            return false;
        }

        private void RebuildTracks()
        {
            _tracks.Clear();

            if (_statsEntityData == null)
            {
                return;
            }

            foreach (KeyValuePair<string, RuntimeStat> pair in _statsEntityData.Stats)
            {
                if (!pair.Value.Runtime().TryData(out RuntimeStatLevelData levelComponent) ||
                    levelComponent.Table == null)
                {
                    continue;
                }

                _tracks.Add(new UpgradeTrackConfig(pair.Key, pair.Key, levelComponent.Table.MaxLevel));
            }
        }

        private void RefreshHealthBinding()
        {
            _healthSubscription?.Dispose();
            _healthSubscription = null;

            if (_statsEntityData == null || !_statsEntityData.Stats.TryGetValue(HealthId, out RuntimeStat healthStat))
            {
                _currentHealth.Value = 0f;
                _maxHealth.Value = 0f;
                return;
            }

            void ApplyHealth(float value)
            {
                _currentHealth.Value = value;
                _maxHealth.Value = value;
            }

            RuntimeStatValueData valueData = healthStat.Runtime().Data<RuntimeStatValueData>();
            ApplyHealth(valueData.CurrentValue);
            _healthSubscription = valueData.Value.Subscribe(ApplyHealth);
        }
    }
}
