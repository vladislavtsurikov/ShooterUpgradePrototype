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
    [Name("Stats/Modify/Dice")]
    public sealed class ModifyStatDiceAction : EntityMonoBehaviourAction
    {
        [InfoBox("Rolls `Rolls` dice with `Sides` sides and sets the stat value to the sum.")]
        [SerializeField] private Stat _stat;
        [UnityEngine.Min(1)]
        [SerializeField] private int _rolls = 1;
        [UnityEngine.Min(1)]
        [SerializeField] private int _sides = 6;

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

            RuntimeStatValueData valueData = stats.Stat(_stat.Id).RuntimeData<RuntimeStatValueData>();
            int rolls = Mathf.Max(1, _rolls);
            int sides = Mathf.Max(1, _sides);
            int total = 0;

            for (int index = 0; index < rolls; index++)
            {
                total += Random.Range(1, sides + 1);
            }

            return valueData.SetValue(total);
        }
    }
}
