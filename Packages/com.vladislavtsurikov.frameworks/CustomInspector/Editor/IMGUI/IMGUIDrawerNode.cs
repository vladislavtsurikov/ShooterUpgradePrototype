#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class IMGUIDrawerNode : InspectorNode
    {
        private readonly IMGUIFieldDrawer _drawer;
        private readonly IMGUIInspectorFieldsDrawer _owner;

        public IMGUIDrawerNode(IMGUIInspectorFieldsDrawer owner, IMGUIFieldDrawer drawer)
        {
            _owner = owner;
            _drawer = drawer;
        }

        public override void Execute(InspectorNodeContext context)
        {
            if (context is not IMGUIInspectorNodeContext imguiContext)
            {
                return;
            }

            var field = context.Field;
            var target = context.Target;
            object value = context.Value;

            var fieldLabel = new GUIContent(context.FieldName, context.Tooltip);
            float fieldHeight = _drawer.GetFieldsHeight(target, field, value);
            Rect fieldRect = EditorGUI.IndentedRect(new Rect(
                imguiContext.Rect.x,
                imguiContext.Rect.y,
                imguiContext.Rect.width,
                fieldHeight));

            using (_owner.CreateFieldPresentationScope(
                       field,
                       target,
                       context.StateProcessors,
                       context.StyleProcessors,
                       null))
            {
                EditorGUI.BeginChangeCheck();
                var newValue = _drawer.Draw(fieldRect, fieldLabel, field, target, value);
                bool uiChanged = EditorGUI.EndChangeCheck();
                newValue = _owner.ApplyProcessorsAndAssignIfNeeded(
                    field,
                    target,
                    newValue,
                    value,
                    context.ValueProcessors,
                    uiChanged);
                context.Value = newValue;
            }

            var rect = imguiContext.Rect;
            rect.y += fieldHeight;
            imguiContext.Rect = rect;
            imguiContext.TotalHeight += fieldHeight;
        }
    }
}
#endif
