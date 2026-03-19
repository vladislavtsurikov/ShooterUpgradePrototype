#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace UIRootSystem.Runtime
{
    [SceneFilter("Battle")]
    public sealed class RootLoader : UIToolkitLayoutLoader
    {
        public override string LayoutAddress => "UIRootSystem_UIToolkitRoot";
    }
}
#endif
