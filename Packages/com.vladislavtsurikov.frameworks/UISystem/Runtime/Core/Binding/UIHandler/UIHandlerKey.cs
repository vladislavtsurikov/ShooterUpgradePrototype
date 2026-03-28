using System;
using System.Runtime.CompilerServices;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    internal readonly struct UIHandlerKey
    {
        public Type ParentType { get; }
        public int ParentRuntimeId { get; }
        public string InstanceKey { get; }
        public bool IsDynamic { get; }

        public string Id
        {
            get
            {
                if (IsDynamic)
                {
                    return $"{ParentRuntimeId}:{InstanceKey ?? string.Empty}";
                }

                string parentId = ParentType?.FullName ?? "0";
                return string.IsNullOrEmpty(InstanceKey)
                    ? parentId
                    : $"{parentId}:{InstanceKey}";
            }
        }

        private UIHandlerKey(Type parentType, int parentRuntimeId, string instanceKey, bool isDynamic)
        {
            ParentType = parentType;
            ParentRuntimeId = parentRuntimeId;
            InstanceKey = instanceKey;
            IsDynamic = isDynamic;
        }

        public static UIHandlerKey FromHandler(UIHandler handler) =>
            IsDynamicChild(handler)
                ? FromDynamicParent(handler.Parent, handler.InstanceKey)
                : FromParentType(handler.Parent?.GetType(), handler.InstanceKey);

        public static UIHandlerKey FromParentType(Type parentType, string instanceKey = null) =>
            new(parentType, 0, instanceKey, false);

        public static UIHandlerKey FromDynamicParent(UIHandler parent, string instanceKey) =>
            new(null, parent != null ? RuntimeHelpers.GetHashCode(parent) : 0, instanceKey, true);

        private static bool IsDynamicChild(UIHandler handler) =>
            handler != null && Attribute.IsDefined(handler.GetType(), typeof(DynamicUIChildAttribute), inherit: true);
    }
}
