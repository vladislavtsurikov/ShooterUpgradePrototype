#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.StateMachine.Runtime.Data;
using VladislavTsurikov.StateMachine.Runtime.Definitions;

namespace VladislavTsurikov.StateMachine.Editor
{
    public sealed class StateReorderableListStackEditor
        : ReorderableListStackEditor<State, ReorderableListComponentEditor>
    {
        private readonly StateMachineData _data;

        public StateReorderableListStackEditor(StateMachineData data)
            : base(data.StateStack)
        {
            _data = data;
        }

        protected override void DrawHeaderElement(Rect totalRect, int index, ReorderableListComponentEditor componentEditor)
        {
            base.DrawHeaderElement(totalRect, index, componentEditor);

            if (_data == null || index < 0 || index >= _data.StateStack.ElementList.Count)
            {
                return;
            }

            State state = _data.StateStack.ElementList[index];
            bool isActiveState = ReferenceEquals(_data.CurrentState.Value, state);
            bool isEligible = state?.IsEligibleForTransition?.Value ?? false;

            string status = $"Active: {(isActiveState ? "Yes" : "No")}  Eligible: {(isEligible ? "Yes" : "No")}";

            Rect statusRect = totalRect;
            statusRect.xMin += 260f;
            statusRect.xMax -= 34f;
            statusRect.y += 2f;
            statusRect.height = EditorGUIUtility.singleLineHeight;

            Color previous = GUI.color;
            if (isActiveState)
            {
                GUI.color = new Color(0.45f, 0.95f, 0.45f, 1f);
            }
            else if (isEligible)
            {
                GUI.color = new Color(1f, 0.85f, 0.35f, 1f);
            }

            GUI.Label(statusRect, status, EditorStyles.miniBoldLabel);
            GUI.color = previous;
        }
    }
}
#endif
