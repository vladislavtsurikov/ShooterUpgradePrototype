using Nody.Runtime.Core;
using OdinSerializer;
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
        // TODO: There was a problem displaying LocalizedString and serializing it.
        //[OdinSerialize]
        //private LocalizedString _itemName;

        [OdinSerialize]
        private string _tableName;

        [OdinSerialize]
        private string _key;

        public string TableName => _tableName;
        public string Key => _key;
    }
}
