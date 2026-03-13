using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace AutoStrike.Actions
{
    // Legacy shim kept only so old serialized prefab actions can be loaded and removed safely by SyncToTypesIfNeeded.
    public sealed class ReadInputAction : EntityMonoBehaviourAction
    {
    }
}
