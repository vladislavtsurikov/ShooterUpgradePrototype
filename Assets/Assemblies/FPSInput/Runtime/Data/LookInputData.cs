using UniRx;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Input.FPSInput.Runtime
{
    [Name("AutoStrike.Input/Data/LookInput")]
    public sealed class LookInputData : ComponentData
    {
        private ReactiveProperty<Vector2> _lookDelta;
        private ReactiveProperty<Vector2> _lookRate;

        public ReactiveProperty<Vector2> LookDelta
        {
            get
            {
                _lookDelta ??= new ReactiveProperty<Vector2>();
                return _lookDelta;
            }
        }

        public ReactiveProperty<Vector2> LookRate
        {
            get
            {
                _lookRate ??= new ReactiveProperty<Vector2>();
                return _lookRate;
            }
        }
    }
}
