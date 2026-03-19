#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    [SceneFilter("Battle")]
    public sealed class UpgradeStatRowLayoutLoader : UIToolkitLayoutLoader
    {
        public override string LayoutAddress => "UpgradeStatRow";
    }
}
#endif
