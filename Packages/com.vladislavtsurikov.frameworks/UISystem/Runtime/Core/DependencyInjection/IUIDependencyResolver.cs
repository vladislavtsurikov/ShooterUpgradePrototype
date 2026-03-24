using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public interface IUIDependencyResolver
    {
        object Instantiate(Type type);
        void BindInstance(Type type, string id, object instance);
        bool TryResolve(Type type, out object instance);
        bool TryResolveId(Type type, string id, out object instance);
        void UnbindId(Type type, string id);
    }
}
