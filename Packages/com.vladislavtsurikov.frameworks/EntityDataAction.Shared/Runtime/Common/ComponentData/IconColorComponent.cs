using Nody.Runtime.Core;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [Name("Common/IconColor")]
    [Group("CommonUI")]
    public sealed class IconColorComponent : ComponentData
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Color Color { get; private set; } = Color.white;
    }
}
