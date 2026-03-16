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
            return UniTask.FromResult(ApplyNow());
        }

        public bool ApplyNow()
        {
            StatsEntityData stats = Get<StatsEntityData>();
            if (!stats.Stats.ContainsKey(_stat.Id))
            {
                Debug.LogWarning($"Stat `{_stat.Id}` was not found on `{EntityMonoBehaviour.name}`.", EntityMonoBehaviour);
                return false;
            }

            if (Random.value > _chance)
            {
                return false;
            }

            RuntimeStatValueData valueData = stats.Stat(_stat.Id).RuntimeData<RuntimeStatValueData>();
            return valueData.SetValue(_value);
        }
    }
}
