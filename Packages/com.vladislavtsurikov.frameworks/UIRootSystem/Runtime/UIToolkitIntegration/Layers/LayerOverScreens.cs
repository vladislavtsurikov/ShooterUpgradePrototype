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
    public sealed class LayerOverScreens : Layer
    {
        public LayerOverScreens(DiContainer container, LayerOverScreensLoader loader)
            : base(container, loader)
        {
        }

        protected override string GetParentContainerName() => RootSlots.LayerOverScreensRoot;
        protected override string GetRootName() => "LayerOverScreens";
    }
}
#endif
#endif
#endif
