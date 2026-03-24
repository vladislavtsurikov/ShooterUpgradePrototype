using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    internal readonly struct BoundBindingRecord
    {
        public BoundBindingRecord(Type type, string id)
        {
            Type = type;
            Id = id;
        }

        public Type Type { get; }
        public string Id { get; }
    }
}
