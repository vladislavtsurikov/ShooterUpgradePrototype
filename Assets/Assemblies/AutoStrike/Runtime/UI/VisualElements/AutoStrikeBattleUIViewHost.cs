using AutoStrike.UI.Entities;
using UnityEngine.UIElements;
using VladislavTsurikov.EntityDataAction.Runtime.UIToolkitIntegration;

namespace AutoStrike.UI.VisualElements
{
    public sealed class AutoStrikeBattleUIViewHost : UIToolkitEntityHost<AutoStrikeBattleUIToolkitEntity>
    {
        protected override AutoStrikeBattleUIToolkitEntity CreateEntity(VisualElement root) =>
            new(root);
    }
}

