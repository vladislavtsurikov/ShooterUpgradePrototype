#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration
{
    [SceneFilter("TestScene_1", "TestScene_2", "Battle")]
    public sealed class Root : UIToolkitUIHandler
    {
        public Root(DiContainer container, RootLoader loader)
            : base(container, loader)
        {
        }

        protected override async UniTask InitializeUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);

        protected override string GetRootName() => "UIRoot";
    }
}
#endif
#endif
#endif
