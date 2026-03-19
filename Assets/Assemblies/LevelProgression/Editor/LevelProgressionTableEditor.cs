#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LevelProgression.Runtime;
using LevelProgression.Runtime.ProgressionTables;
using OdinSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LevelProgression.Editor
{
    [CustomEditor(typeof(LevelProgressionTable))]
    public sealed class LevelProgressionTableEditor : UnityEditor.Editor
    {
        private const float GraphHeight = 360f;
        private const float MinBarWidth = 18f;
        private const float BarSpacing = 4f;

        private readonly List<VisualElement> _bars = new();

        private Label _descriptionLabel;
        private VisualElement _graphBarsRoot;
        private VisualElement _graphContentRoot;
        private VisualElement _manualSection;
        private ScrollView _manualValuesScroll;
        private PopupField<string> _progressionField;
        private VisualElement _progressionFieldsRoot;
        private Label _selectionCard;
        private VisualElement _selectionMarker;
        private int _selectedLevel;
        private Slider _zoomSlider;

        private LevelProgressionTable LevelProgressionTable => (LevelProgressionTable)target;
        private IReadOnlyList<Type> ProgressionTypes => _progressionTypes ??= LoadProgressionTypes();
        private ProgressionTable CurrentProgression => LevelProgressionTable.Progression;

        private IReadOnlyList<Type> _progressionTypes;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement
            {
                style =
                {
                    paddingLeft = 10,
                    paddingRight = 10,
                    paddingTop = 8,
                    paddingBottom = 12
                }
            };

            root.Add(BuildProgressionSelection());
            root.Add(BuildDescription());
            root.Add(BuildProgressionFieldsRoot());
            root.Add(BuildManualSection());
            root.Add(CreateZoomField());
            root.Add(CreateGraphFrame());

            RefreshUI();
            return root;
        }

        private VisualElement BuildProgressionSelection()
        {
            List<string> names = ProgressionTypes
                .Select(type => CreateProgression(type).DisplayName)
                .ToList();

            _progressionField = new PopupField<string>("Table", names, Mathf.Max(0, GetProgressionIndex(CurrentProgression?.GetType())));
            _progressionField.RegisterValueChangedCallback(OnProgressionChanged);
            return _progressionField;
        }

        private VisualElement BuildDescription()
        {
            _descriptionLabel = new Label
            {
                style =
                {
                    marginTop = 6,
                    marginBottom = 10,
                    paddingLeft = 12,
                    paddingRight = 12,
                    paddingTop = 10,
                    paddingBottom = 10,
                    backgroundColor = new Color(0.21f, 0.21f, 0.21f, 1f),
                    color = new Color(0.88f, 0.88f, 0.88f, 1f),
                    whiteSpace = WhiteSpace.Normal,
                    borderTopLeftRadius = 8,
                    borderTopRightRadius = 8,
                    borderBottomLeftRadius = 8,
                    borderBottomRightRadius = 8
                }
            };

            return _descriptionLabel;
        }

        private VisualElement BuildProgressionFieldsRoot()
        {
            _progressionFieldsRoot = new VisualElement
            {
                style =
                {
                    marginBottom = 8
                }
            };

            return _progressionFieldsRoot;
        }

        private VisualElement BuildManualSection()
        {
            _manualSection = new VisualElement
            {
                style =
                {
                    marginTop = 4,
                    marginBottom = 8
                }
            };

            IntegerField levelsCountField = new IntegerField("Levels Count");
            levelsCountField.RegisterValueChangedCallback(evt =>
            {
                RecordChange("Change Levels Count");
                LevelProgressionTable.SetValuesCount(evt.newValue);
                ClampSelectedLevel();
                RefreshUI();
            });

            _manualValuesScroll = new ScrollView
            {
                style =
                {
                    maxHeight = 260,
                    marginTop = 6
                }
            };

            _manualSection.Add(levelsCountField);
            _manualSection.Add(_manualValuesScroll);
            return _manualSection;
        }

        private VisualElement CreateZoomField()
        {
            _zoomSlider = new Slider("Zoom", 0.6f, 2.25f)
            {
                value = 1f
            };

            _zoomSlider.style.marginBottom = 10;
            _zoomSlider.RegisterValueChangedCallback(_ => RefreshGraph());
            return _zoomSlider;
        }

        private VisualElement CreateGraphFrame()
        {
            var graphFrame = new VisualElement
            {
                style =
                {
                    position = Position.Relative,
                    height = GraphHeight + 28f,
                    marginBottom = 14,
                    backgroundColor = new Color(0.19f, 0.19f, 0.19f, 1f),
                    borderTopLeftRadius = 10,
                    borderTopRightRadius = 10,
                    borderBottomLeftRadius = 10,
                    borderBottomRightRadius = 10,
                    overflow = Overflow.Hidden
                }
            };

            _selectionCard = new Label
            {
                style =
                {
                    position = Position.Absolute,
                    left = 14,
                    top = 12,
                    minWidth = 126,
                    paddingLeft = 12,
                    paddingRight = 12,
                    paddingTop = 10,
                    paddingBottom = 10,
                    backgroundColor = new Color(0.04f, 0.04f, 0.04f, 0.95f),
                    color = new Color(0.92f, 0.92f, 0.92f, 1f),
                    whiteSpace = WhiteSpace.Normal,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    borderTopLeftRadius = 8,
                    borderTopRightRadius = 8,
                    borderBottomLeftRadius = 8,
                    borderBottomRightRadius = 8
                }
            };

            _selectionMarker = new VisualElement
            {
                style =
                {
                    position = Position.Absolute,
                    top = 0,
                    width = 18,
                    height = GraphHeight + 28f,
                    backgroundColor = new Color(0.12f, 0.12f, 0.12f, 0.82f),
                    display = DisplayStyle.None
                }
            };

            var scroll = new ScrollView(ScrollViewMode.Horizontal)
            {
                style =
                {
                    height = GraphHeight + 28f
                }
            };

            _graphContentRoot = new VisualElement
            {
                style =
                {
                    position = Position.Relative,
                    minHeight = GraphHeight + 24f
                }
            };

            _graphBarsRoot = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.FlexEnd,
                    minHeight = GraphHeight,
                    height = GraphHeight,
                    paddingLeft = 12,
                    paddingRight = 12,
                    paddingTop = 12,
                    paddingBottom = 12
                }
            };

            _graphContentRoot.Add(_selectionMarker);
            _graphContentRoot.Add(_graphBarsRoot);
            scroll.Add(_graphContentRoot);
            graphFrame.Add(scroll);
            graphFrame.Add(_selectionCard);
            return graphFrame;
        }

        private void OnProgressionChanged(ChangeEvent<string> evt)
        {
            int index = _progressionField.choices.IndexOf(evt.newValue);
            if (index < 0 || index >= ProgressionTypes.Count)
            {
                return;
            }

            Type selectedType = ProgressionTypes[index];
            if (CurrentProgression?.GetType() == selectedType)
            {
                return;
            }

            RecordChange("Change Table Progression");
            LevelProgressionTable.SetProgression(CreateProgression(selectedType));
            ClampSelectedLevel();
            RefreshUI();
        }

        private void RefreshUI()
        {
            int progressionIndex = Mathf.Max(0, GetProgressionIndex(CurrentProgression?.GetType()));
            _progressionField.SetValueWithoutNotify(_progressionField.choices[progressionIndex]);
            _descriptionLabel.text = CurrentProgression?.Description ?? "Progression table is not selected.";

            RebuildProgressionFields();
            RebuildManualValues();

            _manualSection.style.display = CurrentProgression?.CanEditValuesDirectly == true
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            ClampSelectedLevel();
            RefreshGraph();
            UpdateSelectionCard();
        }

        private void RebuildProgressionFields()
        {
            _progressionFieldsRoot.Clear();

            if (CurrentProgression == null)
            {
                return;
            }

            foreach (FieldInfo field in GetEditableFields(CurrentProgression.GetType()))
            {
                VisualElement fieldElement = CreateProgressionField(field);
                if (fieldElement != null)
                {
                    _progressionFieldsRoot.Add(fieldElement);
                }
            }
        }

        private VisualElement CreateProgressionField(FieldInfo field)
        {
            object currentValue = field.GetValue(CurrentProgression);
            string label = ObjectNames.NicifyVariableName(field.Name.TrimStart('_'));

            if (field.FieldType == typeof(int))
            {
                var intField = new IntegerField(label)
                {
                    value = (int)currentValue
                };

                intField.RegisterValueChangedCallback(evt =>
                {
                    RecordChange($"Change {label}");
                    field.SetValue(CurrentProgression, evt.newValue);
                    LevelProgressionTable.RebuildValues();
                    ClampSelectedLevel();
                    RefreshUI();
                });

                return intField;
            }

            if (field.FieldType == typeof(float))
            {
                var floatField = new FloatField(label)
                {
                    value = (float)currentValue
                };

                floatField.RegisterValueChangedCallback(evt =>
                {
                    RecordChange($"Change {label}");
                    field.SetValue(CurrentProgression, evt.newValue);
                    LevelProgressionTable.RebuildValues();
                    ClampSelectedLevel();
                    RefreshUI();
                });

                return floatField;
            }

            if (field.FieldType == typeof(bool))
            {
                var toggle = new Toggle(label)
                {
                    value = (bool)currentValue
                };

                toggle.RegisterValueChangedCallback(evt =>
                {
                    RecordChange($"Change {label}");
                    field.SetValue(CurrentProgression, evt.newValue);
                    LevelProgressionTable.RebuildValues();
                    ClampSelectedLevel();
                    RefreshUI();
                });

                return toggle;
            }

            return null;
        }

        private void RebuildManualValues()
        {
            _manualValuesScroll.Clear();

            for (int level = 0; level < LevelProgressionTable.Values.Count; level++)
            {
                int capturedLevel = level;
                var field = new FloatField($"Level {capturedLevel}")
                {
                    value = LevelProgressionTable.Values[capturedLevel]
                };

                field.RegisterValueChangedCallback(evt =>
                {
                    RecordChange("Change Manual Table Value");
                    LevelProgressionTable.SetValue(capturedLevel, evt.newValue);
                    RefreshUI();
                });

                _manualValuesScroll.Add(field);
            }
        }

        private void RefreshGraph()
        {
            _graphBarsRoot.Clear();
            _bars.Clear();

            int maxLevel = LevelProgressionTable.MaxLevel;
            if (maxLevel < 0)
            {
                _selectionMarker.style.display = DisplayStyle.None;
                return;
            }

            float minValue = LevelProgressionTable.GetValue(0);
            float maxValue = minValue;

            for (int level = 1; level <= maxLevel; level++)
            {
                float value = LevelProgressionTable.GetValue(level);
                minValue = Mathf.Min(minValue, value);
                maxValue = Mathf.Max(maxValue, value);
            }

            float range = Mathf.Max(0.0001f, maxValue - minValue);
            float zeroOffset = minValue < 0f ? (-minValue / range) : 0f;
            float zoom = _zoomSlider?.value ?? 1f;
            float barWidth = MinBarWidth * zoom;

            for (int level = 0; level <= maxLevel; level++)
            {
                float value = LevelProgressionTable.GetValue(level);
                float normalizedHeight = range <= 0.0001f
                    ? 1f
                    : Mathf.Clamp01((value - minValue) / range);

                var bar = new VisualElement
                {
                    style =
                    {
                        width = barWidth,
                        height = Mathf.Max(8f, GraphHeight * Mathf.Max(0.04f, normalizedHeight)),
                        marginRight = BarSpacing,
                        backgroundColor = GetBarColor(level == _selectedLevel),
                        borderTopLeftRadius = 2,
                        borderTopRightRadius = 2
                    },
                    tooltip = GetSelectionText(level)
                };

                if (minValue < 0f)
                {
                    bar.style.marginBottom = GraphHeight * zeroOffset;
                }

                int capturedLevel = level;
                bar.RegisterCallback<MouseEnterEvent>(_ => SetSelectedLevel(capturedLevel));
                bar.RegisterCallback<MouseDownEvent>(_ => SetSelectedLevel(capturedLevel));

                _bars.Add(bar);
                _graphBarsRoot.Add(bar);
            }

            float totalWidth = (maxLevel + 1) * (barWidth + BarSpacing) + 24f;
            _graphBarsRoot.style.minWidth = totalWidth;
            _graphContentRoot.style.minWidth = totalWidth;

            UpdateSelectionVisuals(barWidth);
        }

        private void ClampSelectedLevel()
        {
            _selectedLevel = LevelProgressionTable.ClampLevel(_selectedLevel);
        }

        private void SetSelectedLevel(int level)
        {
            int clampedLevel = LevelProgressionTable.ClampLevel(level);
            if (clampedLevel == _selectedLevel && _bars.Count > 0)
            {
                return;
            }

            _selectedLevel = clampedLevel;
            UpdateSelectionCard();
            UpdateSelectionVisuals(MinBarWidth * (_zoomSlider?.value ?? 1f));
        }

        private void UpdateSelectionCard()
        {
            _selectionCard.text = GetSelectionText(_selectedLevel);
        }

        private string GetSelectionText(int level)
        {
            float previousValue = level > 0 ? LevelProgressionTable.GetValue(level - 1) : 0f;
            float value = LevelProgressionTable.GetValue(level);
            float delta = value - previousValue;
            return $"Level:    {level}\nStep:     {delta:0.##}\nRange:   {previousValue:0.##} - {value:0.##}";
        }

        private void UpdateSelectionVisuals(float barWidth)
        {
            for (int i = 0; i < _bars.Count; i++)
            {
                _bars[i].style.backgroundColor = GetBarColor(i == _selectedLevel);
            }

            if (_selectedLevel < 0 || _selectedLevel >= _bars.Count)
            {
                _selectionMarker.style.display = DisplayStyle.None;
                return;
            }

            _selectionMarker.style.display = DisplayStyle.Flex;
            _selectionMarker.style.width = barWidth + 2f;
            _selectionMarker.style.left = 12f + _selectedLevel * (barWidth + BarSpacing) - 1f;
        }

        private static IReadOnlyList<Type> LoadProgressionTypes()
        {
            return TypeCache.GetTypesDerivedFrom<ProgressionTable>()
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .OrderBy(type => CreateProgression(type).DisplayName)
                .ToArray();
        }

        private static ProgressionTable CreateProgression(Type type)
        {
            return Activator.CreateInstance(type) as ProgressionTable;
        }

        private int GetProgressionIndex(Type type)
        {
            for (int i = 0; i < ProgressionTypes.Count; i++)
            {
                if (ProgressionTypes[i] == type)
                {
                    return i;
                }
            }

            return 0;
        }

        private static IEnumerable<FieldInfo> GetEditableFields(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return type.GetFields(flags)
                .Where(field =>
                    !field.IsStatic &&
                    (field.IsPublic ||
                     field.GetCustomAttribute<SerializeField>() != null ||
                     field.GetCustomAttribute<OdinSerializeAttribute>() != null));
        }

        private static Color GetBarColor(bool selected)
        {
            return selected
                ? new Color(0.73f, 0.89f, 0.36f, 1f)
                : new Color(0.57f, 0.68f, 0.35f, 1f);
        }

        private void RecordChange(string actionName)
        {
            Undo.RecordObject(LevelProgressionTable, actionName);
            EditorUtility.SetDirty(LevelProgressionTable);
        }
    }
}
#endif
