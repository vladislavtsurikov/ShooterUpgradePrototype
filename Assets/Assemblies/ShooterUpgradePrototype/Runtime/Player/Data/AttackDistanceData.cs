using OdinSerializer;
using UniRx;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Data
{
    [Name("AutoStrike/Data/AttackDistance")]
    public sealed class AttackDistanceData : ComponentData
    {
        [OdinSerialize]
        private ReactiveProperty<float> _attackRange = new(1.2f);

        public ReactiveProperty<float> AttackRange
        {
            get
            {
                _attackRange ??= new ReactiveProperty<float>(1.2f);
                return _attackRange;
            }
        }
    }
}
