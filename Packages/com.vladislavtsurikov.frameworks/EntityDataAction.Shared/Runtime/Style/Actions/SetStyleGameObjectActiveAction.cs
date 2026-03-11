using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    [RunOnDirtyData(typeof(StyleStateData))]
    [RequiresData(typeof(StyleStateData))]
    [Name("UI/Common/Style/SetStyleGameObjectActiveAction")]
    [Group("Style")]
    public sealed class SetStyleGameObjectActiveAction : EntityMonoBehaviourAction
    {
        [OdinSerialize]
        private GameObject _target;

        [OdinSerialize]
        private bool _activeState = true;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            _target.SetActive(_activeState);

            return UniTask.FromResult(true);
        }
    }
}
