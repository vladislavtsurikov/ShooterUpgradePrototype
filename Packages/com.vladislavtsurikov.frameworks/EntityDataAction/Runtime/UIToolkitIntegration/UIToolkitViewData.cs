using UnityEngine.UIElements;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime.UIToolkitIntegration
{
    public abstract class UIToolkitViewData : ComponentData
    {
        protected VisualElement Root { get; private set; }

        protected sealed override void OnFirstSetupComponent(object[] setupData = null)
        {
            Root = FindRoot(setupData);
            BindElements();
        }

        protected TElement Query<TElement>(string name) where TElement : VisualElement =>
            Root?.Q<TElement>(name);

        protected abstract void BindElements();

        private static VisualElement FindRoot(object[] setupData)
        {
            if (setupData == null)
            {
                return null;
            }

            for (int i = 0; i < setupData.Length; i++)
            {
                if (setupData[i] is VisualElement root)
                {
                    return root;
                }
            }

            return null;
        }
    }
}
