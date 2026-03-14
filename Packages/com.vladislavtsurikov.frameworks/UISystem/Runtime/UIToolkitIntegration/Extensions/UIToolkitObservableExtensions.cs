#if UI_SYSTEM_UNIRX
using System;
using UniRx;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public static class UIToolkitObservableExtensions
    {
        public static IObservable<ClickEvent> OnClickEventAsObservable(this VisualElement element)
        {
            if (element == null)
            {
                return Observable.Throw<ClickEvent>(new ArgumentNullException(nameof(element)));
            }

            return Observable.Create<ClickEvent>(observer =>
            {
                void OnClicked(ClickEvent evt) => observer.OnNext(evt);

                element.RegisterCallback<ClickEvent>(OnClicked);

                return Disposable.Create(() => element.UnregisterCallback<ClickEvent>(OnClicked));
            });
        }

        public static IObservable<Unit> OnClickAsObservable(this Button button)
        {
            if (button == null)
            {
                return Observable.Throw<Unit>(new ArgumentNullException(nameof(button)));
            }

            return Observable.Create<Unit>(observer =>
            {
                void OnClicked() => observer.OnNext(Unit.Default);

                button.clicked += OnClicked;

                return Disposable.Create(() => button.clicked -= OnClicked);
            });
        }
    }
}
#endif
