using System.Collections.Generic;
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
    [Name("Stats/Modify/Random")]
    public sealed class ModifyStatRandomAction : EntityMonoBehaviourAction
    {
        [InfoBox("Sets the stat value to a random number in the [Min, Max] range.")]
        [SerializeField] private Stat _stat;
        [SerializeField] private float _min;
        [SerializeField] private float _max = 1;
        [SerializeField] private bool _integerValue;

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

            float min = Mathf.Min(_min, _max);
            float max = Mathf.Max(_min, _max);

            if (_integerValue)
            {
                int intMin = Mathf.CeilToInt(min);
                int intMax = Mathf.FloorToInt(max);

                return valueData.SetValue(Random.Range(intMin, intMax + 1));
            }

            return valueData.SetValue(Random.Range(min, max));
        }
    }
}
