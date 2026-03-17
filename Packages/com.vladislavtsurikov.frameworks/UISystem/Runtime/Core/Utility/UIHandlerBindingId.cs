using System;
using System.Runtime.CompilerServices;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    internal static class UIHandlerBindingId
    {
        public static string FromHandler(UIHandler handler) =>
            IsDynamicChild(handler)
                ? FromDynamicParent(handler.Parent, handler.InstanceKey)
                : FromParentType(handler.Parent?.GetType(), handler.InstanceKey);

        public static string FromParentType(Type parentType, string instanceKey = null)
        {
            string parentId = parentType?.FullName ?? "0";
            return string.IsNullOrEmpty(instanceKey)
                ? parentId
                : $"{parentId}:{instanceKey}";
        }

        public static string FromDynamicParent(UIHandler parent, string instanceKey)
        {
            int parentId = parent != null ? RuntimeHelpers.GetHashCode(parent) : 0;
            string key = instanceKey ?? string.Empty;
            return $"{parentId}:{key}";
        }

        private static bool IsDynamicChild(UIHandler handler) =>
            handler != null && Attribute.IsDefined(handler.GetType(), typeof(DynamicUIChildAttribute), inherit: true);
    }
}
