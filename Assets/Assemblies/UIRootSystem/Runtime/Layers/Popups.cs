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
    public sealed class Popups : Layer
    {
        public Popups(DiContainer container, PopupsLoader loader)
            : base(container, loader)
        {
        }

        protected override string GetParentContainerName() => RootSlots.PopupsRoot;
        protected override string GetRootName() => "Popups";
    }
}
#endif
#endif
#endif
