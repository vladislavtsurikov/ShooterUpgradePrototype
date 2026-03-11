#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SceneCollectionSystem
{
    public class SceneCollectionStackEditor : ReorderableListStackEditor<SceneCollection, SceneCollectionEditor>
    {
        public SceneCollectionStackEditor(GUIContent reorderableListName,
            NodeStackSupportSameType<SceneCollection> list) : base(reorderableListName, list, true)
        {
            RenameSupport = true;
            CopySettings = true;
        }

        protected override void AddCB(ReorderableList list)
        {
            var nodeStackSupportSameType = (NodeStackSupportSameType<SceneCollection>)Stack;
            nodeStackSupportSameType.CreateNode(typeof(SceneCollection));
        }
    }
}
#endif
