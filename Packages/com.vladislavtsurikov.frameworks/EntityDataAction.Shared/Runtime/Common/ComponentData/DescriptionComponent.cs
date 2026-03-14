using Nody.Runtime.Core;
using UnityEngine;
using UnityEngine.Localization;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [Name("Common/Description")]
    [Group("CommonUI")]
    public sealed class DescriptionComponent : ComponentData
    {
        [field: SerializeField] public LocalizedString ItemDescription { get; private set; }
    }
}
