using System;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class ParentUIToolkitBindingContextResolver : IUIToolkitBindingContextResolver
    {
        public static readonly ParentUIToolkitBindingContextResolver Instance = new();

        private ParentUIToolkitBindingContextResolver()
        {
        }

        public Type ResolveHandlerType(UIToolkitUIHandler handler) =>
            handler.Parent?.GetType() ?? handler.GetType();

        public string ResolveInstanceKey(UIToolkitUIHandler handler) =>
            handler.Parent?.InstanceKey;
    }
}
