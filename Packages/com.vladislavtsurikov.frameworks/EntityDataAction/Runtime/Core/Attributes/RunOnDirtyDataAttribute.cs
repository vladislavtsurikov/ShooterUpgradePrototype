using System;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RunOnDirtyDataAttribute : Attribute
    {
        public RunOnDirtyDataAttribute(params Type[] dataTypes) => DataTypes = dataTypes ?? Array.Empty<Type>();

        public Type[] DataTypes { get; }
    }
}
