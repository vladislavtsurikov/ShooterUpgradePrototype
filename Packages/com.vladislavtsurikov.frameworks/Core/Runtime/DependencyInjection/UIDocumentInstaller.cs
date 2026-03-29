#if UI_SYSTEM_ZENJECT
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace VladislavTsurikov.Core.Runtime.DependencyInjection
{
    public sealed class UIDocumentInstaller : MonoInstaller
    {
        [SerializeField]
        private UIDocument _document;

        public override void InstallBindings()
        {
            if (_document == null)
            {
                throw new System.InvalidOperationException("UIDocument is not assigned.");
            }

            ProjectContext.Instance.Container.Bind<UIDocument>().FromInstance(_document).AsSingle();
        }

        private void OnDestroy()
        {
            ProjectContext.Instance.Container.Unbind<UIDocument>();
        }
    }
}
#endif
