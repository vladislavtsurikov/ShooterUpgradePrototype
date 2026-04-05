#if UI_SYSTEM_ZENJECT
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;

namespace VladislavTsurikov.UIRootSystem.Runtime
{
    public abstract class UILayer : UnityUIPresenter
    {
        protected UILayer(PrefabAssetLoader loader)
            : base(loader)
        {
        }

        public abstract int GetLayerIndex();
    }
}

#endif
