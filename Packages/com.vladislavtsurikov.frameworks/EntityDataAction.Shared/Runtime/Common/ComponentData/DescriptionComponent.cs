using Nody.Runtime.Core;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [Name("Common/Description")]
    [Group("CommonUI")]
    public sealed class DescriptionComponent : ComponentData
    {
        [field: SerializeField] public string Description { get; private set; }
    }
}
