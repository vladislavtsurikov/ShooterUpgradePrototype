using AutoStrike.Services;
using AutoStrike.UI.ComponentData;
using UniRx;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Runtime.UIToolkitIntegration;
using VladislavTsurikov.ReflectionUtility;
using Zenject;

namespace AutoStrike.UI.Actions
{
    [RequiresData(typeof(AutoStrikeViewData))]
    [Name("UI/AutoStrike/SyncKillsLabelAction")]
    public sealed class SyncKillsLabelAction : UIToolkitAction
    {
        private CompositeDisposable _subscriptions = new();

        [Inject]
        private KillCounterService _killCounterService;

        protected override void SetupComponent(object[] setupData = null)
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            _killCounterService.Kills
                .DistinctUntilChanged()
                .Subscribe(ApplyKills)
                .AddTo(_subscriptions);

            ApplyKills(_killCounterService.Kills.Value);
        }

        protected override void OnDisableElement() => _subscriptions?.Clear();

        private void ApplyKills(int kills)
        {
            AutoStrikeViewData view = Get<AutoStrikeViewData>();
            if (view.EnemiesKilledLabel == null)
            {
                return;
            }

            view.EnemiesKilledLabel.text = $"Enemies Killed: {kills}";
        }
    }
}

