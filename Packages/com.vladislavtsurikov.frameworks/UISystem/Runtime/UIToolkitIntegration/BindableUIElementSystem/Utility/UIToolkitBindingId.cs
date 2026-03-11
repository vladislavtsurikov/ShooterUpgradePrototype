using System;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public static class UIToolkitBindingId
    {
        public static string FromTypeAndIndex(Type handlerType, string bindingId, int index = 0)
        {
            string handlerName = handlerType.Name;
            return $"{handlerName}:{bindingId}#{index}";
        }
    }
}
