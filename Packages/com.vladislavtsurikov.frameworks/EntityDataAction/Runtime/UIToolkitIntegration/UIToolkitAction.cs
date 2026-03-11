using UnityEngine.UIElements;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime.UIToolkitIntegration
{
    public abstract class UIToolkitAction : EntityAction
    {
        protected VisualElement Root { get; private set; }

        protected sealed override void OnFirstSetupComponent(object[] setupData = null)
        {
            Root = Entity is UIToolkitEntity uiEntity ? uiEntity.Root : null;
            OnFirstSetupComponentUI(setupData);
        }

        protected virtual void OnFirstSetupComponentUI(object[] setupData = null)
        {
        }

        protected TElement Query<TElement>(string name) where TElement : VisualElement =>
            Root == null ? null : Root.Q<TElement>(name);
    }
}
