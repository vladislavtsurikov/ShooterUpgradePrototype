using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class UIToolkitElementResolver
    {
        private readonly VisualElement _root;

        public UIToolkitElementResolver(VisualElement root) => _root = root;

        public TElement Resolve<TElement>(string bindingId, int index = 0) where TElement : VisualElement
        {
            if (!TryResolve(bindingId, out TElement element, index))
            {
                throw new InvalidOperationException(
                    $"[UIToolkitElementResolver] Cannot resolve `{typeof(TElement).Name}` with bindingId `{bindingId}` at index {index}.");
            }

            return element;
        }

        public bool TryResolve<TElement>(string bindingId, out TElement element, int index = 0)
            where TElement : VisualElement
        {
            element = null;

            if (_root == null || index < 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(bindingId))
            {
                element = _root as TElement;
                return element != null;
            }

            List<TElement> results = _root.Query<TElement>(bindingId).ToList();
            if (index >= results.Count)
            {
                return false;
            }

            element = results[index];
            return element != null;
        }
    }
}
