using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class UIToolkitElementBinder
    {
        private readonly DiContainer _container;
        private readonly List<BoundElementRecord> _records = new();
        private readonly UIToolkitBindingRepeatTracker _repeatTracker = new();
        private readonly UIHandler _uiHandler;

        public UIToolkitElementBinder(DiContainer container, UIHandler handler)
        {
            _container = container;
            _uiHandler = handler;
        }

        public void BindElementsFrom(VisualElement root)
        {
            if (root == null)
            {
                return;
            }

            foreach (VisualElement element in EnumerateElements(root))
            {
                string rawBindingId = GetBindingId(element);
                if (string.IsNullOrEmpty(rawBindingId))
                {
                    continue;
                }

                Type type = element.GetType();
                int index = _repeatTracker.GetAndIncrement(type, rawBindingId);
                string finalId = UIToolkitBindingId.FromTypeAndIndex(
                    _uiHandler.GetType(),
                    rawBindingId,
                    index,
                    _uiHandler.InstanceKey);

                _container.Bind(type)
                    .WithId(finalId)
                    .FromInstance(element)
                    .AsCached();

                _records.Add(new BoundElementRecord(type, finalId));
            }
        }

        public void Dispose()
        {
            foreach (BoundElementRecord record in _records)
            {
                _container.UnbindId(record.Type, record.Id);
            }

            _records.Clear();
            _repeatTracker.Reset();
        }

        private static IEnumerable<VisualElement> EnumerateElements(VisualElement root)
        {
            yield return root;

            List<VisualElement> elements = root.Query<VisualElement>().ToList();
            foreach (VisualElement element in elements)
            {
                if (!ReferenceEquals(element, root))
                {
                    yield return element;
                }
            }
        }

        private static string GetBindingId(VisualElement element)
        {
            if (element is IBindableUIElement bindableElement && !string.IsNullOrEmpty(bindableElement.BindingId))
            {
                return bindableElement.BindingId;
            }

            return element.name;
        }
    }
}
