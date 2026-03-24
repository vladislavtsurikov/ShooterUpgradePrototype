using System;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public interface IUIToolkitBindingContextResolver
    {
        Type ResolveHandlerType(UIToolkitUIHandler handler);
        string ResolveInstanceKey(UIToolkitUIHandler handler);
    }
}
