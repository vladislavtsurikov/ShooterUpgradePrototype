#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.StateMachine.Runtime.Data;

namespace VladislavTsurikov.StateMachine.Editor
{
    [ElementEditor(typeof(StateMachineData))]
    public sealed class StateMachineDataEditor : ReorderableListComponentEditor
    {
        private StateReorderableListStackEditor _statesEditor;
        private StateMachineData _data;

        public override void OnEnable()
        {
            _data = Target as StateMachineData;

            _statesEditor = new StateReorderableListStackEditor(_data)
            {
                DisplayHeaderText = true
            };
        }

        public override void OnGUI(Rect rect, int index)
        {
            if (_data == null)
            {
                return;
            }

            float height = _statesEditor.GetElementStackHeight();
            Rect statesRect = new Rect(rect.x, rect.y, rect.width, height);
            _statesEditor.OnGUI(statesRect);
        }

        public override float GetElementHeight(int index)
        {
            if (_data == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return _statesEditor.GetElementStackHeight();
        }
    }
}
#endif
