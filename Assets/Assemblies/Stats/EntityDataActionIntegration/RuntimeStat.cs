using System;
using System.Collections.Generic;
using OdinSerializer;
using VladislavTsurikov.ActionFlow.Runtime.Stats;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [Serializable]
    public sealed class RuntimeStat
    {
        [OdinSerialize] private Stat _stat;
        [OdinSerialize] private Dictionary<string, RuntimeStatData> _runtimeData;

        public Stat Stat => _stat;

        public RuntimeStat()
        {
            _runtimeData = new Dictionary<string, RuntimeStatData>();
        }

        public void SetStat(Stat stat)
        {
            _stat = stat;
        }

        public void ClearRuntimeData()
        {
            _runtimeData.Clear();
        }

        internal void RestoreRuntimeData()
        {
            var context = new RuntimeStatBuildContext(_stat.Id);

            foreach (RuntimeStatData data in _runtimeData.Values)
            {
                data.Restore(context);
            }
        }

        internal void PersistRuntimeData()
        {
            var context = new RuntimeStatBuildContext(_stat.Id);

            foreach (RuntimeStatData data in _runtimeData.Values)
            {
                data.Persist(context);
            }
        }

        internal void AddRuntimeData(RuntimeStatData data)
        {
            if (data == null)
            {
                return;
            }

            _runtimeData[data.GetType().AssemblyQualifiedName ?? data.GetType().FullName] = data;
        }

        internal T GetRuntimeData<T>() where T : RuntimeStatData
        {
            if (TryGetRuntimeData(out T data))
            {
                return data;
            }

            throw new KeyNotFoundException($"Runtime data `{typeof(T).Name}` was not found on stat `{_stat?.Id}`.");
        }

        internal bool TryGetRuntimeData<T>(out T data) where T : RuntimeStatData
        {
            foreach (RuntimeStatData runtimeComponent in _runtimeData.Values)
            {
                if (runtimeComponent is T typedComponent)
                {
                    data = typedComponent;
                    return true;
                }
            }

            data = null;
            return false;
        }
    }
}
