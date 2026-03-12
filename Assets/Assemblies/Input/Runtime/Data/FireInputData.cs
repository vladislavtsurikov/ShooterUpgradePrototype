using OdinSerializer;
using UniRx;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Input.Data
{
    [Name("AutoStrike.Input/Data/FireInput")]
    public sealed class FireInputData : ComponentData
    {
        [OdinSerialize]
        private ReactiveProperty<bool> _isFirePressed = new(false);

        public ReactiveProperty<bool> IsFirePressed
        {
            get
            {
                _isFirePressed ??= new ReactiveProperty<bool>(false);
                return _isFirePressed;
            }
        }
    }
}
