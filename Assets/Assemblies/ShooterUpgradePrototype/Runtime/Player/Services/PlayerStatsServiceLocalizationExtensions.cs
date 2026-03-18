using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Common;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;

namespace ShooterUpgradePrototype.Player.Services
{
    public static class PlayerStatsServiceLocalizationExtensions
    {
        public static string GetStatName(this PlayerStatsService playerStatsService, string statId)
        {
            RuntimeStat runtimeStat = GetRuntimeStat(playerStatsService, statId);
            return runtimeStat.Stat != null
                ? runtimeStat.Stat.name
                : statId;
        }

        public static string GetLocalizedStatName(this PlayerStatsService playerStatsService, string statId)
        {
            RuntimeStat runtimeStat = GetRuntimeStat(playerStatsService, statId);
            NameComponent nameComponent = runtimeStat.Stat?.ComponentStack?.GetElement<NameComponent>();

            return LocalizationSettings.StringDatabase.GetLocalizedString(
                nameComponent.TableName,
                nameComponent.Key
            );
        }

        private static RuntimeStat GetRuntimeStat(PlayerStatsService playerStatsService, string statId)
        {
            if (string.IsNullOrEmpty(statId))
            {
                throw new System.ArgumentException("Stat id is null or empty.", nameof(statId));
            }

            if (playerStatsService.Stats.Stats == null ||
                !playerStatsService.Stats.Stats.TryGetValue(statId, out RuntimeStat runtimeStat) ||
                runtimeStat == null)
            {
                throw new KeyNotFoundException($"Player stat `{statId}` was not found.");
            }

            return runtimeStat;
        }
    }
}
