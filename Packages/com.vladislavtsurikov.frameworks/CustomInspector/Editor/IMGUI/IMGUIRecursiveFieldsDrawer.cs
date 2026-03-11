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
            Action<object, Rect> drawField)
        {
            EditorGUI.indentLevel++;

            var foldoutState = GetFoldoutState(value);

            foldoutState = EditorGUI.Foldout(fieldRect, foldoutState, ObjectNames.NicifyVariableName(fieldInfo.Name));

            SetFoldoutState(value, foldoutState);

            var foldoutHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            fieldRect.y += foldoutHeight;

            if (foldoutState)
            {
                drawField(value, fieldRect);
            }

            EditorGUI.indentLevel--;

            return new RecursiveDrawResult(foldoutHeight, foldoutState);
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
