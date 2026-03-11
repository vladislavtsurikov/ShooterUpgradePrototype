#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class LayerMaskFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => field.FieldType == typeof(LayerMask);
        public override Type DrawerType => typeof(LayerMaskFieldDrawer);
    }

    public sealed class LayerMaskFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            LayerMask layerMask = value != null ? (LayerMask)value : new LayerMask();

            var layers = new List<string>(32);
            var layerNumbers = new List<int>(32);

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != string.Empty)
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }

            if (layers.Count == 0)
            {
                return layerMask;
            }

            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) != 0)
                {
                    maskWithoutEmpty |= 1 << i;
                }
            }

            maskWithoutEmpty = EditorGUI.MaskField(rect, label, maskWithoutEmpty, layers.ToArray());

            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) != 0)
                {
                    mask |= 1 << layerNumbers[i];
                }
            }

            layerMask.value = mask;
            return layerMask;
        }
    }
}
#endif
