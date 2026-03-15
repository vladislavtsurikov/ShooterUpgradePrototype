#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Loaders
{
    [SceneFilter("Battle")]
    public sealed class UpgradeWindowLayoutLoader : UIToolkitLayoutLoader
    {
        public override string LayoutAddress => "UpgradeWindow";
    }
}
#endif
