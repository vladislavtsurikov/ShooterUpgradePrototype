using System;
using AutoStrike.UI.Actions;
using AutoStrike.UI.ComponentData;
using UnityEngine.UIElements;
using VladislavTsurikov.EntityDataAction.Runtime.UIToolkitIntegration;

namespace AutoStrike.UI.Entities
{
    public sealed class AutoStrikeBattleUIToolkitEntity : UIToolkitEntity, IDisposable
    {
        public AutoStrikeBattleUIToolkitEntity(VisualElement root) : base(root)
        {
        }

        protected override Type[] ComponentDataTypesToCreate() =>
            new[] { typeof(AutoStrikeViewData) };

        protected override Type[] ActionTypesToCreate() =>
            new[] { typeof(SyncKillsLabelAction) };
    }
}

