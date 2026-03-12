#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.ActionFlow.Runtime.Stats;

namespace VladislavTsurikov.ActionFlow.Editor.Stats
{
    [CustomEditor(typeof(Table))]
    public sealed class TableEditor : UnityEditor.Editor
    {
        private const float GraphHeight = 360f;
        private const float MinBarWidth = 18f;
        private const float BarSpacing = 4f;

        private static readonly string[] ModeOptions =
        {
            "Manual Values",
            "Linear Progression"
        };

        private readonly List<VisualElement> _bars = new();

        private FloatField _baseValueField;
        private VisualElement _graphBarsRoot;
        private VisualElement _graphContentRoot;
        private VisualElement _graphFrame;
        private FloatField _incrementField;
        private IntegerField _levelsCountField;
        private VisualElement _linearSection;
        private ScrollView _manualValuesScroll;
        private IntegerField _maxLevelField;
        private PopupField<string> _modeField;
        private Label _selectionCard;
        private VisualElement _selectionMarker;
        private int _selectedLevel;
        private Slider _zoomSlider;

        private Table Table => (Table)target;

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

            root.Add(CreateZoomField());
            root.Add(CreateGraphFrame());

            _modeField = new PopupField<string>("Table", new List<string>(ModeOptions), GetModeIndex(Table.Mode));
            _modeField.RegisterValueChangedCallback(OnModeChanged);
            root.Add(_modeField);

            _linearSection = BuildLinearSection();
            root.Add(_linearSection);

            root.Add(BuildManualSection());

            RefreshUI();
            return root;
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
            _graphFrame = new VisualElement
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
                    borderBottomRightRadius = 8,
                    zIndex = 2
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
            _graphFrame.Add(scroll);
            _graphFrame.Add(_selectionCard);
            return _graphFrame;
        }

        private VisualElement BuildLinearSection()
        {
            var container = new VisualElement
            {
                style =
                {
                    marginTop = 6
                }
            };

            _maxLevelField = new IntegerField("Max Level");
            _maxLevelField.RegisterValueChangedCallback(evt =>
            {
                RecordChange("Change Linear Max Level");
                Table.LinearMaxLevel = evt.newValue;
                ClampSelectedLevel();
                RefreshUI();
            });

            _baseValueField = new FloatField("Base Value");
            _baseValueField.RegisterValueChangedCallback(evt =>
            {
                RecordChange("Change Linear Base Value");
                Table.LinearBaseValue = evt.newValue;
                RefreshUI();
            });

            _incrementField = new FloatField("Increment Per Level");
            _incrementField.RegisterValueChangedCallback(evt =>
            {
                RecordChange("Change Linear Increment");
                Table.LinearIncrementPerLevel = evt.newValue;
                RefreshUI();
            });

            container.Add(_maxLevelField);
            container.Add(_baseValueField);
            container.Add(_incrementField);
            return container;
        }

        private VisualElement BuildManualSection()
        {
            var container = new VisualElement
            {
                style =
                {
                    marginTop = 6
                }
            };

            _levelsCountField = new IntegerField("Levels Count");
            _levelsCountField.RegisterValueChangedCallback(evt =>
            {
                RecordChange("Change Levels Count");
                ResizeManualValues(evt.newValue);
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

            container.Add(_levelsCountField);
            container.Add(_manualValuesScroll);
            return container;
        }

        private void OnModeChanged(ChangeEvent<string> evt)
        {
            Table.ProgressionMode mode = evt.newValue == ModeOptions[1]
                ? Table.ProgressionMode.Linear
                : Table.ProgressionMode.Manual;

            if (Table.Mode == mode)
            {
                return;
            }

            RecordChange("Change Table Mode");
            Table.Mode = mode;
            ClampSelectedLevel();
            RefreshUI();
        }

        private void RefreshUI()
        {
            _modeField.SetValueWithoutNotify(ModeOptions[GetModeIndex(Table.Mode)]);

            bool isLinear = Table.Mode == Table.ProgressionMode.Linear;
            _linearSection.style.display = isLinear ? DisplayStyle.Flex : DisplayStyle.None;
            _levelsCountField?.parent?.style.display = isLinear ? DisplayStyle.None : DisplayStyle.Flex;

            if (isLinear)
            {
                _maxLevelField.SetValueWithoutNotify(Table.LinearMaxLevel);
                _baseValueField.SetValueWithoutNotify(Table.LinearBaseValue);
                _incrementField.SetValueWithoutNotify(Table.LinearIncrementPerLevel);
            }
            else
            {
                _levelsCountField.SetValueWithoutNotify(Mathf.Max(1, Table.Values.Count));
                RebuildManualValues();
            }

            ClampSelectedLevel();
            RefreshGraph();
            UpdateSelectionCard();
        }

        private void RefreshGraph()
        {
            _graphBarsRoot.Clear();
            _bars.Clear();

            int maxLevel = Table.MaxLevel;
            if (maxLevel < 0)
            {
                _selectionMarker.style.display = DisplayStyle.None;
                return;
            }

            float minValue = Table.GetValue(0);
            float maxValue = minValue;

            for (int level = 1; level <= maxLevel; level++)
            {
                float value = Table.GetValue(level);
                minValue = Mathf.Min(minValue, value);
                maxValue = Mathf.Max(maxValue, value);
            }

            float range = Mathf.Max(0.0001f, maxValue - minValue);
            float zeroOffset = minValue < 0f ? (-minValue / range) : 0f;
            float zoom = _zoomSlider?.value ?? 1f;
            float barWidth = MinBarWidth * zoom;

            for (int level = 0; level <= maxLevel; level++)
            {
                float value = Table.GetValue(level);
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

        private void RebuildManualValues()
        {
            _manualValuesScroll.Clear();

            for (int level = 0; level < Table.Values.Count; level++)
            {
                int capturedLevel = level;
                var field = new FloatField($"Level {capturedLevel}")
                {
                    value = Table.Values[capturedLevel]
                };

                field.RegisterValueChangedCallback(evt =>
                {
                    RecordChange("Change Manual Table Value");
                    Table.Values[capturedLevel] = evt.newValue;
                    RefreshUI();
                });

                _manualValuesScroll.Add(field);
            }
        }

        private void ResizeManualValues(int requestedCount)
        {
            int targetCount = Mathf.Max(1, requestedCount);
            while (Table.Values.Count < targetCount)
            {
                float nextValue = Table.Values.Count == 0 ? 0f : Table.Values[Table.Values.Count - 1];
                Table.Values.Add(nextValue);
            }

            while (Table.Values.Count > targetCount)
            {
                Table.Values.RemoveAt(Table.Values.Count - 1);
            }
        }

        private void ClampSelectedLevel()
        {
            _selectedLevel = Table.ClampLevel(_selectedLevel);
        }

        private void SetSelectedLevel(int level)
        {
            int clampedLevel = Table.ClampLevel(level);
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
            float previousValue = level > 0 ? Table.GetValue(level - 1) : 0f;
            float value = Table.GetValue(level);
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

        private static Color GetBarColor(bool selected)
        {
            return selected
                ? new Color(0.73f, 0.89f, 0.36f, 1f)
                : new Color(0.57f, 0.68f, 0.35f, 1f);
        }

        private static int GetModeIndex(Table.ProgressionMode mode)
        {
            return mode == Table.ProgressionMode.Linear ? 1 : 0;
        }

        private void RecordChange(string actionName)
        {
            Undo.RecordObject(Table, actionName);
            EditorUtility.SetDirty(Table);
        }
    }
}
#endif
