using System;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ParentRequiredAttribute : Attribute
    {
        public ParentRequiredAttribute(params Type[] parentTypes)
        {
            ParentTypes = parentTypes ?? Array.Empty<Type>();
        }

        public Type[] ParentTypes { get; }
    }
}
