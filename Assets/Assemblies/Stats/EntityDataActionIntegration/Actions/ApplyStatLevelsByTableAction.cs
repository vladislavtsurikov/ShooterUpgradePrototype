using System.Collections.Generic;
using System.Threading;
using ArmyClash.Battle.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [RequiresData(typeof(StatsEntityData), typeof(ModifiersData))]
    [Name("Stats/Apply Stat Levels By Table")]
    public sealed class ApplyStatLevelsByTableAction : EntityMonoBehaviourAction
    {
        protected override UniTask<bool> Run(CancellationToken token)
        {
            ApplyLevels();
            return UniTask.FromResult(true);
        }

        public void ApplyLevels()
        {
            StatsEntityData statsEntityData = Get<StatsEntityData>();
            ModifiersData modifiersData = Get<ModifiersData>();
            var effects = new List<ModifierStatEffect>();

            foreach (RuntimeStat runtimeStat in statsEntityData.Stats.Values)
            {
                if (!runtimeStat.Runtime().TryData(out RuntimeStatLevelData levelComponent) ||
                    levelComponent.Table == null)
                {
                    continue;
                }

                if (!runtimeStat.Runtime().TryData(out RuntimeStatValueData valueComponent))
                {
                    continue;
                }

                float baseValue = valueComponent.BaseValue;
                float targetValue = levelComponent.Table.GetValue(levelComponent.AppliedLevel.Value);
                float delta = targetValue - baseValue;

                if (Mathf.Approximately(delta, 0f))
                {
                    continue;
                }

                effects.Add(BuildModifier(runtimeStat.Stat, delta));
            }

            modifiersData.ReplaceAll(effects);
        }

        private static ModifierStatEffect BuildModifier(Stat stat, float delta)
        {
            var effect = ScriptableObject.CreateInstance<ModifierStatEffect>();
            effect.hideFlags = HideFlags.HideAndDontSave;
            effect.SetEntries(new[]
            {
                new StatEffect.Entry
                {
                    Stat = stat,
                    Delta = delta
                }
            });

            return effect;
        }
    }
}
