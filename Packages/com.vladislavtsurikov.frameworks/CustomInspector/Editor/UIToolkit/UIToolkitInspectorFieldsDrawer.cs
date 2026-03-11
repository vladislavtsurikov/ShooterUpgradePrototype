#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public class UIToolkitInspectorFieldsDrawer : InspectorFieldsDrawer<UIToolkitFieldDrawer, UIToolkitDecoratorDrawer>
    {
        private readonly UIToolkitRecursiveFieldsDrawer _recursiveFieldsDrawer = new();

        public UIToolkitInspectorFieldsDrawer(
            List<Type> excludedDeclaringTypes = null,
            bool excludeInternal = true,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            : base(excludedDeclaringTypes, excludeInternal, bindingFlags)
        {
        }

        public VisualElement CreateFieldsContainer(object target, int? elementIndex = null)
        {
            var container = new VisualElement();

            if (target == null)
            {
                container.Add(new Label("Target is null"));
                return container;
            }

            DrawFieldsRecursive(target, container, elementIndex);

            return container;
        }

        private void DrawFieldsRecursive(object target, VisualElement container, int? elementIndex)
        {
            if (target == null)
            {
                return;
            }

            foreach (var processedField in GetProcessedFields(target))
            {
                var context = new UIToolkitInspectorNodeContext(
                    target,
                    processedField.Field,
                    processedField.FieldName,
                    processedField.Tooltip,
                    processedField.Value,
                    processedField.DecoratorsBase,
                    processedField.VisibilityProcessors,
                    processedField.StateProcessors,
                    processedField.StyleProcessors,
                    processedField.ValueProcessors,
                    container,
                    _recursiveFieldsDrawer,
                    (nestedTarget, nestedContainer) => DrawFieldsRecursive(nestedTarget, nestedContainer, elementIndex));

                processedField.Graph.Execute(context);
            }
        }

        protected override FieldPresentationScope CreateFieldPresentationScope(
            FieldState state,
            FieldStyle style,
            object fieldElement)
        {
            return new UIToolkitFieldPresentationScope(state, style, fieldElement as VisualElement);
        }

        protected override FieldGraph CreateFieldGraph(ProcessedField processedField)
        {
            var graph = new FieldGraph();
            InspectorNode previousNode = null;

            foreach (UIToolkitDecoratorDrawer decorator in processedField.Decorators)
            {
                var node = new UIToolkitDecoratorNode(decorator);
                graph.AddNode(node);
                if (previousNode != null)
                {
                    graph.Connect(previousNode, node);
                }

                previousNode = node;
            }

            InspectorNode finalNode = processedField.Drawer != null
                ? new UIToolkitDrawerNode(this, processedField.Drawer)
                : new UIToolkitRecursiveDrawerNode(_recursiveFieldsDrawer);

            graph.AddNode(finalNode);
            if (previousNode != null)
            {
                graph.Connect(previousNode, finalNode);
            }

            return graph;
        }
    }
}
#endif
