using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation
{
    public class ProgressBar : SceneOperation
    {
        public CanvasGroup Group;
        public Image Image;
        public Slider Slider;
        public Color BackgroundColor;
        public float DurationFade = 0.5f;
        private SettingsSystem.ProgressBar _progressBar;

        private void Start()
        {
            _progressBar =
                (SettingsSystem.ProgressBar)SceneCollection.Current.SettingsStack.GetElement(
                    typeof(SettingsSystem.ProgressBar));

            if (!_progressBar.DisableFade)
            {
                Group.alpha = 0;
            }
        }

        private void Update()
        {
            if (Slider)
            {
                Slider.value = SceneCollection.Current.LoadingProgress;
            }
        }

        public override async UniTask OnLoad(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Image.color = BackgroundColor;

            await UniTask.Yield(PlayerLoopTiming.Update, token);

            if (_progressBar.DisableFade)
            {
                return;
            }

            await Group.Fade(1, DurationFade);
        }

        public override async UniTask OnUnload(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (_progressBar.DisableFade)
            {
                return;
            }

            if (Slider)
            {
                Slider.gameObject.SetActive(false);
            }

            await Group.Fade(0, DurationFade);
        }
    }
}