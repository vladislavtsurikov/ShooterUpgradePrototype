#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration
{
    public abstract class UIToolkitLayerLayoutLoader : UIToolkitLayoutLoader
    {
        public override string LayoutAddress => "UIRootSystem_UIToolkitLayer";
    }
}
#endif
