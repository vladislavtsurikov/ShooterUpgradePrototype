using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UIBindingId
    {
        public static string FromTypeAndIndex(
            Type handlerType,
            string bindingId,
            int index = 0,
            string instanceKey = null)
        {
            string handlerName = handlerType.Name;

            return string.IsNullOrEmpty(instanceKey)
                ? $"{handlerName}:{bindingId}#{index}"
                : $"{handlerName}:{instanceKey}:{bindingId}#{index}";
        }
    }
}
