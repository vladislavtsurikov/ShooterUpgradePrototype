using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public readonly struct ViewKey
    {
        public ViewKey(
            Type viewType,
            Type handlerType,
            string bindingId,
            int index = 0,
            string instanceKey = null)
        {
            ViewType = viewType;
            HandlerType = handlerType;
            BindingId = bindingId;
            Index = index;
            InstanceKey = instanceKey;
        }

        public Type ViewType { get; }
        public Type HandlerType { get; }
        public string BindingId { get; }
        public int Index { get; }
        public string InstanceKey { get; }

        public string Id
        {
            get
            {
                string handlerName = HandlerType.Name;

                return string.IsNullOrEmpty(InstanceKey)
                    ? $"{handlerName}:{BindingId}#{Index}"
                    : $"{handlerName}:{InstanceKey}:{BindingId}#{Index}";
            }
        }
    }
}
