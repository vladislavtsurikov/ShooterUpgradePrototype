using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine.UI;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    [RunOnDirtyData(typeof(StyleStateData))]
    [RequiresData(typeof(StyleStateData))]
    [Name("UI/Common/Style/SetStyleButtonInteractableAction")]
    [Group("Style")]
    public sealed class SetStyleButtonInteractableAction : EntityMonoBehaviourAction
    {
        [OdinSerialize]
        private Button _target;

        [OdinSerialize]
        private bool _interactableState = true;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            _target.interactable = _interactableState;

            return UniTask.FromResult(true);
        }
    }
}
