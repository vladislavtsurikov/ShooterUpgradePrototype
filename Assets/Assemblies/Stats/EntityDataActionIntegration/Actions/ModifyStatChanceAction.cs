using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [RequiresData(typeof(StatsEntityData))]
    [Name("Stats/Modify/Chance")]
    public sealed class ModifyStatChanceAction : EntityMonoBehaviourAction
    {
        [InfoBox("Applies Value to the stat only if the Chance roll succeeds.")]
        [SerializeField] private Stat _stat;
        [Range(0f, 1f)]
        [SerializeField] private float _chance = 1f;
        [SerializeField] private float _value;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (!TryGetValueData(out RuntimeStatValueData valueData))
            {
                return UniTask.FromResult(false);
            }

            if (Random.value > _chance)
            {
                return UniTask.FromResult(false);
            }

            return UniTask.FromResult(valueData.SetValue(_value));
        }

        private bool TryGetValueData(out RuntimeStatValueData valueData)
        {
            valueData = null;

            StatsEntityData statsEntityData = Get<StatsEntityData>();
            if (!statsEntityData.Stats.TryGetValue(_stat.Id, out RuntimeStat runtimeStat))
            {
                Debug.LogWarning($"Stat `{_stat.Id}` was not found on `{EntityMonoBehaviour.name}`.", EntityMonoBehaviour);
                return false;
            }

            if (!runtimeStat.Runtime().TryData(out valueData))
            {
                Debug.LogWarning($"Stat `{_stat.Id}` does not have {nameof(RuntimeStatValueData)}.", EntityMonoBehaviour);
                return false;
            }

            return true;
        }
    }
}
