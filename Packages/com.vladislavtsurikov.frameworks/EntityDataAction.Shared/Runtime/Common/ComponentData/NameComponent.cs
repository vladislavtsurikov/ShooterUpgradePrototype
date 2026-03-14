using Nody.Runtime.Core;
using UnityEngine;
using UnityEngine.Localization;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [Name("Common/Name")]
    [Group("CommonUI")]
    public sealed class NameComponent : ComponentData
    {
        [field: SerializeField] public LocalizedString ItemName { get; private set; }
    }
}
