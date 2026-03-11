using OdinSerializer;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [Name("UI/Common/Selected/SelectedData")]
    public sealed class SelectedData : ComponentData
    {
        [OdinSerialize]
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                MarkDirty();
            }
        }
    }
}
