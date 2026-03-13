#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;

namespace VladislavTsurikov.EntityDataAction.Shared.Editor.Stats
{
    [ElementEditor(typeof(StatsEntityData))]
    public sealed class StatsEntityDataEditor : ReorderableListComponentEditor
    {
        private StatsEntityData _data;
        private GUIStyle _headerStyle;
        private GUIStyle _rowStyle;
        private GUIStyle _nameStyle;
        private GUIStyle _valueStyle;

        public override void OnEnable()
        {
            _data = Target as StatsEntityData;
        }

        public override void OnGUI(Rect rect, int index)
        {
            if (_data == null)
            {
                return;
            }

            BuildStyles();

            float y = rect.y;
            float lineHeight = EditorGUIUtility.singleLineHeight;

            Rect sourceRect = new Rect(rect.x, y, rect.width, lineHeight);
            EditorGUI.BeginChangeCheck();
            StatsEntitySourceType sourceType = (StatsEntitySourceType)EditorGUI.EnumPopup(
                sourceRect,
                "Source",
                _data.SourceType);
            if (EditorGUI.EndChangeCheck())
            {
                _data.SourceType = sourceType;
                MarkTargetDirty();
            }
            y += lineHeight + 4f;

            if (_data.SourceType == StatsEntitySourceType.Global)
            {
                Rect configRect = new Rect(rect.x, y, rect.width, lineHeight);
                EditorGUI.BeginChangeCheck();
                StatsEntityConfig config = (StatsEntityConfig)EditorGUI.ObjectField(
                    configRect,
                    "Global Config",
                    _data.GlobalConfig,
                    typeof(StatsEntityConfig),
                    false);
                if (EditorGUI.EndChangeCheck())
                {
                    _data.GlobalConfig = config;
                    MarkTargetDirty();
                }

                y += lineHeight + 6f;
            }

            Rect collectionRect = new Rect(rect.x, y, rect.width, lineHeight);
            using (new EditorGUI.DisabledScope(_data.SourceType == StatsEntitySourceType.Global))
            {
                EditorGUI.BeginChangeCheck();
                StatCollection collection = (StatCollection)EditorGUI.ObjectField(
                    collectionRect,
                    "Collection",
                    _data.Collection,
                    typeof(StatCollection),
                    false);
                if (EditorGUI.EndChangeCheck())
                {
                    _data.Collection = collection;
                    MarkTargetDirty();
                }
            }
            y += lineHeight + 6f;

            float headerHeight = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, headerHeight), "Stats", _headerStyle);
            y += headerHeight + 4f;

            var stats = _data.Stats;
            if (stats == null || stats.Count == 0)
            {
                EditorGUI.LabelField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    "No stats in collection", EditorStyles.helpBox);
                return;
            }

            float rowHeight = EditorGUIUtility.singleLineHeight + 8f;
            foreach (var stat in stats.Values)
            {
                if (stat == null || stat.Stat == null)
                {
                    continue;
                }

                Rect rowRect = new Rect(rect.x, y, rect.width, rowHeight);
                EditorGUI.LabelField(rowRect, GUIContent.none, _rowStyle);

                string name = stat.Stat.name;
                var valueComponent = stat.Stat.ComponentStack.GetElement<StatValueComponent>();
                RuntimeStatValueData valueData = stat.Runtime().Data<RuntimeStatValueData>();
                string valueText = valueData.CurrentValue.ToString("0.##");

                Rect nameRect = new Rect(rowRect.x + 8f, rowRect.y + 4f, rowRect.width * 0.7f, rowHeight);
                EditorGUI.LabelField(nameRect, $"{name}: {valueText}", _nameStyle);

                if (valueComponent != null && valueComponent.ClampEnabled)
                {
                    string clampText = BuildClampText(valueComponent, valueData.CurrentValue);
                    Rect clampRect = new Rect(rowRect.x + rowRect.width * 0.7f, rowRect.y + 4f,
                        rowRect.width * 0.3f - 8f, rowHeight);
                    EditorGUI.LabelField(clampRect, clampText, _valueStyle);
                }

                y += rowHeight + 2f;
            }
        }

        public override float GetElementHeight(int index)
        {
            if (_data == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float sourceHeight = EditorGUIUtility.singleLineHeight + 4f;
            float configHeight = _data.SourceType == StatsEntitySourceType.Global
                ? EditorGUIUtility.singleLineHeight + 6f
                : 0f;
            float collectionHeight = EditorGUIUtility.singleLineHeight + 6f;
            int count = _data.Stats != null ? _data.Stats.Count : 0;
            float headerHeight = EditorGUIUtility.singleLineHeight + 4f;
            float rowHeight = EditorGUIUtility.singleLineHeight + 8f;
            float rowsHeight = count > 0 ? count * (rowHeight + 2f) : EditorGUIUtility.singleLineHeight;
            return sourceHeight + configHeight + collectionHeight + headerHeight + rowsHeight;
        }

        private static string BuildClampText(StatValueComponent component, float value)
        {
            string minText = component.UseMin ? component.MinValue.ToString("0.##") : "-inf";
            string maxText = component.UseMax ? component.MaxValue.ToString("0.##") : "inf";
            return $"{minText}..{maxText} ({value:0.##})";
        }

        private void BuildStyles()
        {
            GUIStyle baseLabel = GUI.skin != null ? GUI.skin.label : null;

            _headerStyle ??= new GUIStyle(baseLabel ?? new GUIStyle())
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            _rowStyle ??= new GUIStyle(EditorStyles.helpBox);
            _nameStyle ??= new GUIStyle(baseLabel ?? new GUIStyle())
            {
                fontStyle = FontStyle.Bold
            };
            _valueStyle ??= new GUIStyle(baseLabel ?? new GUIStyle())
            {
                alignment = TextAnchor.MiddleRight
            };
        }
    }
}
#endif
