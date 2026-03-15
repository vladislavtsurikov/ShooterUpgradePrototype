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

        protected override UniTask<bool> Run(CancellationToken token)
        {
            return UniTask.FromResult(ApplyNow());
        }

        public void Configure(Stat stat, float min, float max)
        {
            _stat = stat;
            _min = min;
            _max = max;
        }

        public bool ApplyNow()
        {
            if (!TryGetValueData(out RuntimeStatValueData valueData))
            {
                return false;
            }

            float min = Mathf.Min(_min, _max);
            float max = Mathf.Max(_min, _max);
            float value = Random.Range(min, max);

            return valueData.SetValue(value);
        }

        private bool TryGetValueData(out RuntimeStatValueData valueData)
        {
            valueData = null;

            if (_stat == null)
            {
                Debug.LogWarning($"{nameof(ModifyStatRandomAction)} requires a Stat reference.", EntityMonoBehaviour);
                return false;
            }

            StatsEntityData statsEntityData = Get<StatsEntityData>();
            if (statsEntityData?.Stats == null || !statsEntityData.Stats.TryGetValue(_stat.Id, out RuntimeStat runtimeStat))
            {
                Debug.LogWarning($"Stat `{_stat.Id}` was not found on `{EntityMonoBehaviour?.name}`.", EntityMonoBehaviour);
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
