using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    [CreateAssetMenu(menuName = "Stats/Stats Entity Config", fileName = "StatsEntityConfig")]
    public sealed class StatsEntityConfig : SerializedScriptableObject
    {
        [OdinSerialize]
        private StatsEntityState _stats = new();

        public StatsEntityState Stats => _stats ??= new StatsEntityState();
    }
}
