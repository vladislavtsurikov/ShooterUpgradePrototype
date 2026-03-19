using OdinSerializer;
using UniRx;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Input.FPSInput.Runtime
{
    [Name("AutoStrike.Input/Data/MoveInput")]
    public sealed class MoveInputData : ComponentData
    {
        [OdinSerialize]
        private ReactiveProperty<Vector2> _moveDirection = new(Vector2.zero);

        public ReactiveProperty<Vector2> MoveDirection
        {
            get
            {
                _moveDirection ??= new ReactiveProperty<Vector2>(Vector2.zero);
                return _moveDirection;
            }
        }
    }
}
