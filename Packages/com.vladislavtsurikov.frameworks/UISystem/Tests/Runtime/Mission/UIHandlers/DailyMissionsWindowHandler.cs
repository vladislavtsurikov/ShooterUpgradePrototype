#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_1")]
    [UIParent(typeof(UIMissionsMainWindowHandler))]
    public class DailyMissionsWindowHandler : UnityUIPresenter
    {
        private readonly MissionViewLoader _missionViewLoader;
        private bool _spawnedOnce;
        private MissionWindowView _view;

        public DailyMissionsWindowHandler(
            GeneralMissionLoader generalMissionLoader,
            MissionViewLoader missionViewLoader)
            : base(generalMissionLoader)
        {
            _missionViewLoader = missionViewLoader;
        }

        protected override Transform GetSpawnParentTransform()
        {
            UIMissionsMainWindowHandler mainWindowHandler = (UIMissionsMainWindowHandler)Parent;
            return mainWindowHandler.View.MissionSpawnRect;
        }

        protected override async UniTask AfterShowUIPresenter(CancellationToken ct, CompositeDisposable disposables)
        {
            _view ??= GetView<MissionWindowView>("MissionWindowView");

            if (_spawnedOnce)
            {
                return;
            }

            _spawnedOnce = true;

            for (var i = 0; i < 6; i++)
            {
                await _missionViewLoader.LoadAndSpawnPrefab(_view.MissionSpawnRect, ct);
            }
        }
    }
}

#endif

#endif

#endif
