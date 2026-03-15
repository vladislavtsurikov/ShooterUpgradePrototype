using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    internal static class UIHandlerBindingId
    {
        public static string FromHandler(UIHandler handler) =>
            FromParentType(handler.Parent?.GetType(), handler.InstanceKey);

        public static string FromParentType(Type parentType, string instanceKey = null)
        {
            string parentId = parentType?.FullName ?? "0";
            return string.IsNullOrEmpty(instanceKey)
                ? parentId
                : $"{parentId}:{instanceKey}";
        }
    }
}
