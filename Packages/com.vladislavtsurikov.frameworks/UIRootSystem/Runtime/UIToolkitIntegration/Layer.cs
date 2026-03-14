#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration
{
    public abstract class Layer : UIToolkitUIHandler
    {
        protected Layer(DiContainer container, UIToolkitLayoutLoader loader)
            : base(container, loader)
        {
        }

        protected override async UniTask InitializeUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);
    }
}
#endif
#endif
#endif
