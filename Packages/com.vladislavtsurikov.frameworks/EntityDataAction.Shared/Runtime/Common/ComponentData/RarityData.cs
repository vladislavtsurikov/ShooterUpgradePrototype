using OdinSerializer;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [Name("UI/Common/RarityData")]
    public sealed class RarityData : ComponentData
    {
        [OdinSerialize]
        [Min(0)]
        private int _rarity;

        public int Rarity
        {
            get => _rarity;
            set
            {
                if (_rarity == value)
                {
                    return;
                }

                _rarity = value;
                MarkDirty();
            }
        }
    }
}
