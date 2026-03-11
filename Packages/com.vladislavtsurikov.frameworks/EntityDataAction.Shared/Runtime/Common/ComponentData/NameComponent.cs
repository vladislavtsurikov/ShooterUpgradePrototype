using Nody.Runtime.Core;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [Name("Common/Name")]
    [Group("CommonUI")]
    public sealed class NameComponent : ComponentData
    {
        [field: SerializeField] public string NameTitle { get; private set; }
    }
}
