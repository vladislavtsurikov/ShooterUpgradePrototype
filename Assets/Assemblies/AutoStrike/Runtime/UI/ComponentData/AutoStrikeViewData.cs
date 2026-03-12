using OdinSerializer;
using UnityEngine.UIElements;
using VladislavTsurikov.EntityDataAction.Runtime.UIToolkitIntegration;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.UI.ComponentData
{
    [Name("UI/AutoStrike/AutoStrikeBattleUIViewData")]
    public sealed class AutoStrikeViewData : UIToolkitViewData
    {
        [OdinSerialize]
        private string _enemiesKilledLabelName = "enemiesKilledLabel";

        public Label EnemiesKilledLabel { get; private set; }

        protected override void BindElements()
        {
            EnemiesKilledLabel = Query<Label>(_enemiesKilledLabelName);
        }
    }
}

