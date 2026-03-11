#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Editor.Core
{
    [CustomEditor(typeof(EntityMonoBehaviour), true)]
    public sealed class EntityEditor : UnityEditor.Editor
    {
        private readonly EntityMonoBehaviourInspectorDrawer _inspectorDrawer = new();
        private EntityMonoBehaviour _entity;

        private void OnEnable()
        {
            _entity = target as EntityMonoBehaviour;
            _inspectorDrawer.Setup(_entity);
        }

        public override void OnInspectorGUI() => _inspectorDrawer.DrawInspector();
    }
}
#endif
