#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.EntityDataAction.Shared.Editor
{
    public sealed class ObjectPoolRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() => "ENTITY_DATA_ACTION_OBJECT_POOL";
        public override string GetTypeFullName() => "VladislavTsurikov.ObjectPool.Runtime.MonoBehaviourPool`1";
    }
}
#endif
