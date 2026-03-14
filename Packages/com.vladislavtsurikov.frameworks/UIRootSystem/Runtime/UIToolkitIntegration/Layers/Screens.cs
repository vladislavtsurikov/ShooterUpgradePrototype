#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration
{
    [ParentUIHandler(typeof(Root))]
    [SceneFilter("TestScene_1", "TestScene_2", "Battle")]
    public sealed class Screens : Layer
    {
        public Screens(DiContainer container, ScreensLoader loader)
            : base(container, loader)
        {
        }

        protected override string GetParentContainerName() => RootSlots.ScreensRoot;
        protected override string GetRootName() => "Screens";
    }
}
#endif
#endif
#endif
