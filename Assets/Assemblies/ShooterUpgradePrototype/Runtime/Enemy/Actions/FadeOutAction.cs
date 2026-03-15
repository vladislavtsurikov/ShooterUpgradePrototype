using System.Threading;
using Cysharp.Threading.Tasks;
using Nody.Runtime.Core;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [Name("AutoStrike/Actions/FadeOut")]
    [Group("Death")]
    public sealed class FadeOutAction : EntityMonoBehaviourAction
    {
        private static readonly int DitherFadeId = Shader.PropertyToID("_DitherFade");

        [SerializeField]
        [Min(0.05f)]
        private float _fadeDuration = 0.45f;

        private readonly MaterialPropertyBlock _propertyBlock = new();

        private Renderer[] _renderers;

        protected override void SetupComponent(object[] setupData = null)
        {
            _renderers = EntityMonoBehaviour.GetComponentsInChildren<Renderer>(true);
            SetFade(0f);
        }

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (EntityMonoBehaviour == null)
            {
                return UniTask.FromResult(false);
            }

            return FadeAsync(token);
        }

        private void SetFade(float value)
        {
            if (_renderers == null)
            {
                return;
            }

            for (int index = 0; index < _renderers.Length; index++)
            {
                Renderer renderer = _renderers[index];
                if (renderer == null)
                {
                    continue;
                }

                renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(DitherFadeId, value);
                renderer.SetPropertyBlock(_propertyBlock);
                _propertyBlock.Clear();
            }
        }

        private async UniTask<bool> FadeAsync(CancellationToken token)
        {
            float duration = Mathf.Max(0.05f, _fadeDuration);
            float elapsed = 0f;

            SetFade(0f);

            while (elapsed < duration)
            {
                token.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                SetFade(progress);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            SetFade(1f);
            return true;
        }
    }
}
