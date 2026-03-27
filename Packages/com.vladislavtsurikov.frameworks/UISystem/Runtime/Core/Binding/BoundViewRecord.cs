using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    internal readonly struct BoundViewRecord
    {
        public BoundViewRecord(Type type, string id)
        {
            Type = type;
            Id = id;
        }

        public Type Type { get; }
        public string Id { get; }
    }
}
