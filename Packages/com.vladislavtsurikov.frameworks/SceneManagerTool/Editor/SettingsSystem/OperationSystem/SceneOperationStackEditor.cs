#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem.OperationSystem
{
    public class SceneOperationStackEditor : ReorderableListStackEditor<Action, ReorderableListComponentEditor>
    {
        public SceneOperationStackEditor(ActionCollection list) :
            base(new GUIContent("Actions"), list, true)
        {
            CopySettings = true;
            ShowActiveToggle = false;
        }
    }
}
#endif
