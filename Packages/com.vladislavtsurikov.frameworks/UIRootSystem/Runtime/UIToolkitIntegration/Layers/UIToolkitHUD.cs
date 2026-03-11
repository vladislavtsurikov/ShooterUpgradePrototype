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
    public sealed class UIToolkitHUD : UIToolkitLayer
    {
        public UIToolkitHUD(DiContainer container, UIToolkitHUDLoader loader)
            : base(container, loader)
        {
        }

        protected override string GetParentContainerName() => UIToolkitRootSlots.HudRoot;
        protected override string GetRootName() => "HUD";
    }
}
#endif
#endif
#endif
