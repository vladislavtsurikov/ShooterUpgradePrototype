#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration
{
    [ParentUIHandler(typeof(UIToolkitUIRoot))]
    [SceneFilter("TestScene_1", "TestScene_2", "Battle")]
    public sealed class UIToolkitPopups : UIToolkitLayer
    {
        public UIToolkitPopups(DiContainer container, UIToolkitPopupsLoader loader)
            : base(container, loader)
        {
        }

        protected override string GetParentContainerName() => UIToolkitRootSlots.PopupsRoot;
        protected override string GetRootName() => "Popups";
    }
}
#endif
#endif
#endif
