using System;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class DefaultUIToolkitParentElementResolver : IUIToolkitParentElementResolver
    {
        public static readonly DefaultUIToolkitParentElementResolver Instance = new();

        private DefaultUIToolkitParentElementResolver()
        {
        }

        public VisualElement Resolve(UIToolkitUIHandler handler)
        {
            if (handler.Parent == null)
            {
                return handler.ResolveTopLevelRoot();
            }

            if (handler.Parent is not UIToolkitUIHandler parentHandler)
            {
                throw new InvalidOperationException(
                    $"Invalid parent type: {handler.Parent.GetType().Name}. Expected {nameof(UIToolkitUIHandler)}.");
            }

            string parentContainerName = handler.ResolveParentContainerName();
            if (string.IsNullOrEmpty(parentContainerName))
            {
                return parentHandler.SpawnedRoot;
            }

            if (parentHandler.TryGetUIComponent(parentContainerName, out VisualElement container))
            {
                return container;
            }

            throw new InvalidOperationException(
                $"[UIToolkitUIHandler] Parent container `{parentContainerName}` was not found in handler `{parentHandler.GetType().Name}`.");
        }
    }
}
