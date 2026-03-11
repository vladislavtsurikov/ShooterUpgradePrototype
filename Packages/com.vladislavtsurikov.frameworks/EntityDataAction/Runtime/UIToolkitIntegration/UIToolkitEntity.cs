using UnityEngine.UIElements;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime.UIToolkitIntegration
{
    public class UIToolkitEntity : Entity
    {
        public UIToolkitEntity(VisualElement root)
        {
            Root = root;

            SetSetupData(new object[] { Root });

            Root.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

            Setup();
        }

        public VisualElement Root { get; }

        public void Dispose()
        {
            Root.UnregisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            Disable();
        }
    }
}
