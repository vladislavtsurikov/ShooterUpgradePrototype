using System;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class SelfUIToolkitBindingContextResolver : IUIToolkitBindingContextResolver
    {
        public static readonly SelfUIToolkitBindingContextResolver Instance = new();

        private SelfUIToolkitBindingContextResolver()
        {
        }

        public Type ResolveHandlerType(UIToolkitUIHandler handler) => handler.GetType();

        public string ResolveInstanceKey(UIToolkitUIHandler handler) => handler.InstanceKey;
    }
}
