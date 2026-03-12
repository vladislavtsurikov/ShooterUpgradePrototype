using UniRx;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Input.Data
{
    [Name("AutoStrike.Input/Data/LookInput")]
    public sealed class LookInputData : ComponentData
    {
        public ReactiveProperty<Vector2> LookDelta { get; } = new();
        public ReactiveProperty<Vector2> LookRate { get; } = new();
    }
}
