#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration
{
    [ParentUIHandler(typeof(Root))]
    [SceneFilter("Battle")]
    public sealed class HUD : Layer
    {
        public HUD(DiContainer container, HUDLoader loader)
            : base(container, loader)
        {
        }

        protected override string GetParentContainerName() => RootSlots.HudRoot;
        protected override string GetRootName() => "HUD";
    }
}
#endif
#endif
#endif
