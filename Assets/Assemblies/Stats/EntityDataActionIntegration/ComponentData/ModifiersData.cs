using OdinSerializer;
using UniRx;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace ArmyClash.Battle.Data
{
    [Name("Battle/ModifiersData")]
    public sealed class ModifiersData : ComponentData
    {
        [OdinSerialize]
        private ReactiveCollection<ModifierStatEffect> _effects = new();

        public ReactiveCollection<ModifierStatEffect> Effects
        {
            get
            {
                _effects ??= new ReactiveCollection<ModifierStatEffect>();
                return _effects;
            }
        }

        public void Clear()
        {
            Effects.Clear();
            MarkDirty();
        }

        public void Add(ModifierStatEffect effect)
        {
            Effects.Add(effect);
            MarkDirty();
        }

        public bool Remove(ModifierStatEffect effect)
        {
            bool removed = Effects.Remove(effect);
            MarkDirty();
            return removed;
        }
    }
}
