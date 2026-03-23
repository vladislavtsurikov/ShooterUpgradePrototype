#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class IMGUIRecursiveFieldsDrawer : RecursiveFieldsDrawer
    {
        public RecursiveDrawResult DrawRecursiveFields(
            object value,
            FieldInfo fieldInfo,
            Rect fieldRect,
            Func<object, Rect, float> drawField)
        {
            EditorGUI.indentLevel++;

            var foldoutState = GetFoldoutState(value);
            Rect foldoutRect = new Rect(
                fieldRect.x,
                fieldRect.y,
                fieldRect.width,
                EditorGUIUtility.singleLineHeight);

            foldoutState = EditorGUI.Foldout(
                foldoutRect,
                foldoutState,
                ObjectNames.NicifyVariableName(fieldInfo.Name),
                true);

            SetFoldoutState(value, foldoutState);

            var foldoutHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            fieldRect.y += foldoutHeight;
            fieldRect.height = Mathf.Max(0f, fieldRect.height - foldoutHeight);

            float nestedHeight = 0f;
            if (foldoutState)
            {
                nestedHeight = drawField?.Invoke(value, fieldRect) ?? 0f;
            }

            EditorGUI.indentLevel--;

            return new RecursiveDrawResult(foldoutHeight + nestedHeight, foldoutState);
        }

        public readonly struct RecursiveDrawResult
        {
            public RecursiveDrawResult(float height, bool isExpanded)
            {
                Height = height;
                IsExpanded = isExpanded;
            }

            public float Height { get; }
            public bool IsExpanded { get; }
        }
    }
}
#endif
