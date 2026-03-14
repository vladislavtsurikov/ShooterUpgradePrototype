#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration
{
    [SceneFilter("TestScene_1", "TestScene_2", "Battle")]
    public sealed class RootLoader : UIToolkitLayoutLoader
    {
        public override string LayoutAddress => "UIRootSystem_UIToolkitRoot";
    }
}
#endif
