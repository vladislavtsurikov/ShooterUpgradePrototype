using System;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public readonly struct BoundElementRecord
    {
        public BoundElementRecord(Type type, string id)
        {
            Type = type;
            Id = id;
        }

        public Type Type { get; }
        public string Id { get; }
    }
}
