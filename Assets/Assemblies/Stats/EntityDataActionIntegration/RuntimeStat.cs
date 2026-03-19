using System;
using System.Collections.Generic;
using OdinSerializer;
using Stats.Runtime;

namespace Stats.EntityDataActionIntegration
{
    [Serializable]
    public sealed class RuntimeStat
    {
        [OdinSerialize] private Stat _stat;
        [OdinSerialize] private Dictionary<string, RuntimeStatData> _runtimeData;

        public Stat Stat => _stat;
        public string StatId => _stat?.Id;

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
            foreach (RuntimeStatData data in _runtimeData.Values)
            {
                data.BindRuntimeStat(this);
                data.Restore();
            }
        }

        internal void AddRuntimeData(RuntimeStatData data)
        {
            if (data == null)
            {
                return;
            }

            data.BindRuntimeStat(this);
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
