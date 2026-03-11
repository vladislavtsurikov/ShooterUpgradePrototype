using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    [RunOnDirtyData(typeof(StyleStateData))]
    [RequiresData(typeof(StyleStateData))]
    [Name("UI/Common/Style/SetStyleGraphicColorAction")]
    [Group("Style")]
    public sealed class SetStyleGraphicColorAction : EntityMonoBehaviourAction
    {
        [OdinSerialize]
        private Graphic _target;

        [OdinSerialize]
        private Color _activeColor = Color.white;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            _target.color = _activeColor;

            return UniTask.FromResult(true);
        }
    }
}
