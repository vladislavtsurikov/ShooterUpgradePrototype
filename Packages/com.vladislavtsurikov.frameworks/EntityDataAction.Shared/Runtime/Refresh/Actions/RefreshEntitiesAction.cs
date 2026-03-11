using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Refresh
{
    [Name("UI/Common/Refresh/RefreshEntitiesAction")]
    public sealed class RefreshEntitiesAction : RefreshAction
    {
        [OdinSerialize]
        private List<EntityMonoBehaviour> _entitiesToRefresh = new List<EntityMonoBehaviour>();

        protected override void OnRefresh(CancellationToken token)
        {
            foreach (EntityMonoBehaviour entity in _entitiesToRefresh)
            {
                if (entity != null && entity.IsEntityActive)
                {
                    entity.Actions.Run(token).Forget();
                }
            }
        }
    }
}
