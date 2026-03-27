using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class UIDependencyResolver
    {
        public abstract object Instantiate(Type type);
        public abstract void BindInstance(Type type, string id, object instance);
        public abstract bool TryResolve(Type type, out object instance);
        public abstract bool TryResolveId(Type type, string id, out object instance);
        public abstract void UnbindId(Type type, string id);
    }
}
