#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Editor.Core
{
    public sealed class EntityMonoBehaviourInspectorDrawer
    {
        private ActionReorderableListStackEditor _actionsEditor;
        private ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor> _dataEditor;
        private EntityMonoBehaviour _entity;

        public void Setup(EntityMonoBehaviour entity)
        {
            _entity = entity;
            if (_entity == null)
            {
                return;
            }

            if (!_entity.IsSetup && !Application.isPlaying)
            {
                _entity.Entity.Setup();
            }

            _dataEditor = new ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor>(
                new GUIContent("Data"), _entity.Data, true);
            _dataEditor.ShowActiveToggle = false;

            _actionsEditor = new ActionReorderableListStackEditor(_entity.Actions, _entity.Data);
        }

        public void DrawInspector()
        {
            if (_entity == null)
            {
                return;
            }

            DrawDirtyRunnerButton();

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(3f);
            bool isDerived = _entity.GetType() != typeof(EntityMonoBehaviour);
            ApplyEditingMode(isDerived);

            _dataEditor.OnGUI();

            GUILayout.Space(3f);
            _actionsEditor.OnGUI();

            if (EditorGUI.EndChangeCheck())
            {
                SetDirtyTarget();
            }
        }

        private void ApplyEditingMode(bool isDerived)
        {
            _dataEditor.DisplayPlusButton = !isDerived;
            _dataEditor.DuplicateSupport = !isDerived;
            _dataEditor.RemoveSupport = !isDerived;
            _dataEditor.ReorderSupport = !isDerived;

            _actionsEditor.DisplayPlusButton = !isDerived;
            _actionsEditor.DuplicateSupport = !isDerived;
            _actionsEditor.RemoveSupport = !isDerived;
            _actionsEditor.ReorderSupport = !isDerived;
        }

        private void DrawDirtyRunnerButton()
        {
            DirtyActionRunner runner = _entity.Entity.DirtyRunner;
            if (runner == null)
            {
                return;
            }

            bool enabled = _entity.LocalActive;

            Color prev = GUI.color;
            GUI.color = enabled ? new Color(0.2f, 0.8f, 0.2f, 1f) : new Color(0.8f, 0.2f, 0.2f, 1f);

            string label = enabled ? "Active" : "Inactive";
            if (GUILayout.Button(label))
            {
                _entity.LocalActive = !enabled;
                SetDirtyTarget();
            }

            GUI.color = prev;

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                EditorGUILayout.HelpBox("Inactive in Prefab Mode.", MessageType.Info);
            }
        }

        private void SetDirtyTarget()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(_entity);
            }
        }
    }
}
#endif
