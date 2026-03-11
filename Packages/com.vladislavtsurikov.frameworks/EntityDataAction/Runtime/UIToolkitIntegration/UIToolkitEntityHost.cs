using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.EntityDataAction.Runtime.UIToolkitIntegration
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIToolkitEntityHost<TEntity> : MonoBehaviour where TEntity : class, IDisposable
    {
        private TEntity _entity;
        private CancellationTokenSource _enableCts;
        private bool _isInitializing;

        protected abstract TEntity CreateEntity(VisualElement root);

        private void OnEnable()
        {
            _enableCts?.Cancel();
            _enableCts?.Dispose();
            _enableCts = new CancellationTokenSource();

            UIDocument document = GetComponent<UIDocument>();
            VisualElement root = document.rootVisualElement;
            if (root == null)
            {
                return;
            }

            if (_entity != null || _isInitializing)
            {
                return;
            }

            InitializeWhenReady(root, _enableCts.Token).Forget();
        }

        private void OnDisable()
        {
            _enableCts?.Cancel();
            _enableCts?.Dispose();
            _enableCts = null;
            _isInitializing = false;

            _entity?.Dispose();
            _entity = null;
        }

        private async UniTaskVoid InitializeWhenReady(VisualElement root, CancellationToken token)
        {
            _isInitializing = true;

            try
            {
                await UniTask.WaitUntil(
                    VladislavTsurikov.ZenjectUtility.Runtime.ZenjectUtility.IsSceneContextReady,
                    cancellationToken: token);

                if (!isActiveAndEnabled || _entity != null)
                {
                    return;
                }

                _entity = CreateEntity(root);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _isInitializing = false;
            }
        }
    }
}
