#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace AutoStrike.UI.UISystemIntegration
{
    [SceneFilter("AutoStrikeLegacyBattle")]
    public sealed class AutoStrikeBattleHUDLayoutLoader : UIToolkitLayoutLoader
    {
        public override string LayoutAddress => "AutoStrikeBattle";
    }
}
#endif
