#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.LevelProgression;
using VladislavTsurikov.ActionFlow.Runtime.Stats;

namespace ShooterUpgradePrototype.Editor.Progression
{
    public sealed class TestStatsSetupWindow : EditorWindow
    {
        private const string DefaultStatsFolder = "Assets/Assemblies/AutoStrike/Content/Configs/Stats/Player";
        private const string DefaultTablesFolder = "Assets/Assemblies/AutoStrike/Content/Configs/LevelProgression/Player";
        private const string DefaultCollectionPath = "Assets/Assemblies/AutoStrike/Content/Configs/StatsCollections/PlayerEntityCollection.asset";

        [SerializeField] private string _statsFolder = DefaultStatsFolder;
        [SerializeField] private string _tablesFolder = DefaultTablesFolder;
        [SerializeField] private string _collectionPath = DefaultCollectionPath;
        [SerializeField] private StatCollection _collection;
        [SerializeField] private bool _updateCollection = true;
        [SerializeField] private bool _replaceCollectionContents;
        [SerializeField] private bool _selectCreatedAssets = true;
        [SerializeField] private Vector2 _scrollPosition;
        [SerializeField] private StatSetupPreset _healthPreset = StatSetupPreset.CreateHealth();
        [SerializeField] private StatSetupPreset _speedPreset = StatSetupPreset.CreateSpeed();
        [SerializeField] private StatSetupPreset _attackPreset = StatSetupPreset.CreateAttack();
        [SerializeField] private StatSetupPreset _experiencePreset = StatSetupPreset.CreateExperience();

        [MenuItem("Tools/ShooterUpgradePrototype/Stats/Test Stats Setup")]
        private static void Open()
        {
            TestStatsSetupWindow window = GetWindow<TestStatsSetupWindow>("Test Stats Setup");
            window.minSize = new Vector2(540f, 720f);
        }

        private void OnGUI()
        {
            EnsurePresets();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Asset Targets", EditorStyles.boldLabel);
            _statsFolder = EditorGUILayout.TextField("Stats Folder", _statsFolder);
            _tablesFolder = EditorGUILayout.TextField("Tables Folder", _tablesFolder);
            _collectionPath = EditorGUILayout.TextField("Collection Path", _collectionPath);
            _collection = (StatCollection)EditorGUILayout.ObjectField("Collection Override", _collection, typeof(StatCollection), false);
            _updateCollection = EditorGUILayout.ToggleLeft("Create or update stat collection", _updateCollection);
            using (new EditorGUI.DisabledScope(!_updateCollection))
            {
                _replaceCollectionContents = EditorGUILayout.ToggleLeft("Replace collection contents", _replaceCollectionContents);
            }

            _selectCreatedAssets = EditorGUILayout.ToggleLeft("Select created assets after generation", _selectCreatedAssets);

            EditorGUILayout.Space(8f);
            DrawPreset(_healthPreset);
            DrawPreset(_speedPreset);
            DrawPreset(_attackPreset);
            DrawPreset(_experiencePreset);

            EditorGUILayout.Space(10f);
            EditorGUILayout.HelpBox(
                "HP, SPEED and ATK are configured as upgradeable stats: Value + Level Table component. EXP creates a stat and a separate progression table by default, but does not attach the level table unless you enable it.",
                MessageType.Info);

            if (GUILayout.Button("Create Or Update Test Stats", GUILayout.Height(34f)))
            {
                CreateOrUpdateAssets();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawPreset(StatSetupPreset preset)
        {
            if (preset == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            preset.Expanded = EditorGUILayout.Foldout(preset.Expanded, preset.Label, true);
            if (!preset.Expanded)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            preset.Enabled = EditorGUILayout.ToggleLeft("Enabled", preset.Enabled);
            if (!preset.Enabled)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            preset.StatAssetName = EditorGUILayout.TextField("Stat Asset Name", preset.StatAssetName);
            preset.StatId = EditorGUILayout.TextField("Stat Id", preset.StatId);
            preset.BaseValue = EditorGUILayout.FloatField("Base Value", preset.BaseValue);
            preset.SaveValue = EditorGUILayout.ToggleLeft("Save current value", preset.SaveValue);

            preset.ClampEnabled = EditorGUILayout.ToggleLeft("Clamp value", preset.ClampEnabled);
            if (preset.ClampEnabled)
            {
                preset.UseMin = EditorGUILayout.ToggleLeft("Use min value", preset.UseMin);
                if (preset.UseMin)
                {
                    preset.MinValue = EditorGUILayout.FloatField("Min Value", preset.MinValue);
                }

                preset.UseMax = EditorGUILayout.ToggleLeft("Use max value", preset.UseMax);
                if (preset.UseMax)
                {
                    preset.MaxValue = EditorGUILayout.FloatField("Max Value", preset.MaxValue);
                }
            }

            EditorGUILayout.Space(4f);
            preset.CreateTableAsset = EditorGUILayout.ToggleLeft("Create or update level progression table", preset.CreateTableAsset);
            using (new EditorGUI.DisabledScope(!preset.CreateTableAsset))
            {
                preset.TableAssetName = EditorGUILayout.TextField("Table Asset Name", preset.TableAssetName);
                preset.ProgressionMode = (ProgressionMode)EditorGUILayout.EnumPopup("Progression Mode", preset.ProgressionMode);

                if (preset.ProgressionMode == ProgressionMode.Linear)
                {
                    preset.MaxLevel = Mathf.Max(0, EditorGUILayout.IntField("Max Level", preset.MaxLevel));
                    preset.TableBaseValue = EditorGUILayout.FloatField("Table Base Value", preset.TableBaseValue);
                    preset.IncrementPerLevel = EditorGUILayout.FloatField("Increment Per Level", preset.IncrementPerLevel);
                }
                else
                {
                    DrawManualValues(preset);
                }
            }

            preset.AttachLevelComponent = EditorGUILayout.ToggleLeft("Attach level table component to stat", preset.AttachLevelComponent);
            if (preset.AttachLevelComponent)
            {
                preset.InitialLevel = Mathf.Max(0, EditorGUILayout.IntField("Initial Level", preset.InitialLevel));
                preset.SaveAppliedLevel = EditorGUILayout.ToggleLeft("Save applied level", preset.SaveAppliedLevel);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(6f);
        }

        private static void DrawManualValues(StatSetupPreset preset)
        {
            preset.ManualValues ??= new List<float> { 0f };

            EditorGUILayout.LabelField("Manual Values", EditorStyles.miniBoldLabel);
            for (int i = 0; i < preset.ManualValues.Count; i++)
            {
                preset.ManualValues[i] = EditorGUILayout.FloatField($"Level {i}", preset.ManualValues[i]);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Level"))
            {
                float lastValue = preset.ManualValues.Count > 0 ? preset.ManualValues[preset.ManualValues.Count - 1] : 0f;
                preset.ManualValues.Add(lastValue);
            }

            using (new EditorGUI.DisabledScope(preset.ManualValues.Count <= 1))
            {
                if (GUILayout.Button("Remove Last"))
                {
                    preset.ManualValues.RemoveAt(preset.ManualValues.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void CreateOrUpdateAssets()
        {
            try
            {
                string statsFolder = NormalizeFolderPath(_statsFolder);
                string tablesFolder = NormalizeFolderPath(_tablesFolder);
                EnsureFolderExists(statsFolder);
                EnsureFolderExists(tablesFolder);

                var createdObjects = new List<UnityEngine.Object>();
                var configuredStats = new List<Stat>();

                foreach (StatSetupPreset preset in EnumerateEnabledPresets())
                {
                    LevelProgressionTable table = null;
                    if (preset.CreateTableAsset)
                    {
                        table = CreateOrUpdateTable(preset, tablesFolder, createdObjects);
                    }

                    Stat stat = CreateOrUpdateStat(preset, statsFolder, table, createdObjects);
                    if (stat != null)
                    {
                        configuredStats.Add(stat);
                    }
                }

                if (_updateCollection)
                {
                    StatCollection collection = ResolveCollectionAsset();
                    if (collection != null)
                    {
                        UpdateCollection(collection, configuredStats);
                        createdObjects.Add(collection);
                    }
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (_selectCreatedAssets && createdObjects.Count > 0)
                {
                    Selection.objects = createdObjects.ToArray();
                    EditorGUIUtility.PingObject(createdObjects[0]);
                }

                ShowNotification(new GUIContent("Test stats updated"));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Test Stats Setup Failed", exception.Message, "OK");
            }
        }

        private IEnumerable<StatSetupPreset> EnumerateEnabledPresets()
        {
            if (_healthPreset != null && _healthPreset.Enabled)
            {
                yield return _healthPreset;
            }

            if (_speedPreset != null && _speedPreset.Enabled)
            {
                yield return _speedPreset;
            }

            if (_attackPreset != null && _attackPreset.Enabled)
            {
                yield return _attackPreset;
            }

            if (_experiencePreset != null && _experiencePreset.Enabled)
            {
                yield return _experiencePreset;
            }
        }

        private LevelProgressionTable CreateOrUpdateTable(
            StatSetupPreset preset,
            string tablesFolder,
            ICollection<UnityEngine.Object> createdObjects)
        {
            string assetPath = CombineAssetPath(tablesFolder, $"{preset.TableAssetName}.asset");
            LevelProgressionTable table = LoadOrCreateAsset<LevelProgressionTable>(assetPath);

            if (preset.ProgressionMode == ProgressionMode.Linear)
            {
                table.ConfigureLinear(preset.MaxLevel, preset.TableBaseValue, preset.IncrementPerLevel);
            }
            else
            {
                table.ConfigureManual(preset.ManualValues);
            }

            table.name = preset.TableAssetName;
            EditorUtility.SetDirty(table);
            createdObjects.Add(table);
            return table;
        }

        private Stat CreateOrUpdateStat(
            StatSetupPreset preset,
            string statsFolder,
            LevelProgressionTable table,
            ICollection<UnityEngine.Object> createdObjects)
        {
            string assetPath = CombineAssetPath(statsFolder, $"{preset.StatAssetName}.asset");
            Stat stat = LoadOrCreateAsset<Stat>(assetPath);

            var valueComponent = new StatValueComponent();
            valueComponent.Configure(
                preset.BaseValue,
                preset.SaveValue,
                preset.ClampEnabled,
                preset.UseMin,
                preset.MinValue,
                preset.UseMax,
                preset.MaxValue);

            if (preset.AttachLevelComponent)
            {
                if (table == null)
                {
                    throw new InvalidOperationException($"Stat `{preset.StatId}` requires a level progression table.");
                }

                var levelComponent = new StatLevelTableComponent();
                levelComponent.Configure(table, preset.InitialLevel, preset.SaveAppliedLevel);
                stat.ReplaceComponents(valueComponent, levelComponent);
            }
            else
            {
                stat.ReplaceComponents(valueComponent);
            }

            stat.SetId(preset.StatId);
            stat.name = preset.StatAssetName;
            EditorUtility.SetDirty(stat);
            createdObjects.Add(stat);
            return stat;
        }

        private StatCollection ResolveCollectionAsset()
        {
            if (_collection != null)
            {
                return _collection;
            }

            string collectionPath = NormalizeAssetFilePath(_collectionPath, ".asset");
            EnsureFolderExists(Path.GetDirectoryName(collectionPath)?.Replace("\\", "/"));

            _collection = LoadOrCreateAsset<StatCollection>(collectionPath);
            _collection.name = Path.GetFileNameWithoutExtension(collectionPath);
            EditorUtility.SetDirty(_collection);
            return _collection;
        }

        private void UpdateCollection(StatCollection collection, IReadOnlyList<Stat> configuredStats)
        {
            if (collection == null)
            {
                return;
            }

            if (_replaceCollectionContents)
            {
                collection.Clear();
            }

            for (int i = 0; i < configuredStats.Count; i++)
            {
                collection.AddStat(configuredStats[i]);
            }

            EditorUtility.SetDirty(collection);
        }

        private static T LoadOrCreateAsset<T>(string assetPath) where T : ScriptableObject
        {
            T existingAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existingAsset != null)
            {
                return existingAsset;
            }

            UnityEngine.Object foreignAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (foreignAsset != null)
            {
                throw new InvalidOperationException($"Asset already exists at `{assetPath}` but has type `{foreignAsset.GetType().Name}`.");
            }

            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            return asset;
        }

        private static string CombineAssetPath(string folder, string fileName)
        {
            return $"{folder.TrimEnd('/')}/{fileName}";
        }

        private static string NormalizeFolderPath(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new InvalidOperationException("Folder path must not be empty.");
            }

            string normalizedPath = folderPath.Replace("\\", "/").TrimEnd('/');
            if (!normalizedPath.StartsWith("Assets", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Folder path `{normalizedPath}` must start with `Assets`.");
            }

            return normalizedPath;
        }

        private static string NormalizeAssetFilePath(string assetPath, string extension)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                throw new InvalidOperationException("Asset path must not be empty.");
            }

            string normalizedPath = assetPath.Replace("\\", "/");
            if (!normalizedPath.StartsWith("Assets", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Asset path `{normalizedPath}` must start with `Assets`.");
            }

            if (!normalizedPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                normalizedPath += extension;
            }

            return normalizedPath;
        }

        private static void EnsureFolderExists(string folderPath)
        {
            string normalizedPath = NormalizeFolderPath(folderPath);
            if (AssetDatabase.IsValidFolder(normalizedPath))
            {
                return;
            }

            string[] pathParts = normalizedPath.Split('/');
            string currentPath = pathParts[0];

            for (int i = 1; i < pathParts.Length; i++)
            {
                string nextPath = $"{currentPath}/{pathParts[i]}";
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                }

                currentPath = nextPath;
            }
        }

        private void EnsurePresets()
        {
            _healthPreset ??= StatSetupPreset.CreateHealth();
            _speedPreset ??= StatSetupPreset.CreateSpeed();
            _attackPreset ??= StatSetupPreset.CreateAttack();
            _experiencePreset ??= StatSetupPreset.CreateExperience();
        }

        [Serializable]
        private sealed class StatSetupPreset
        {
            [SerializeField] private bool _expanded = true;

            public string Label;
            public bool Enabled = true;
            public string StatAssetName;
            public string StatId;
            public float BaseValue;
            public bool SaveValue;
            public bool ClampEnabled;
            public bool UseMin;
            public float MinValue;
            public bool UseMax;
            public float MaxValue;
            public bool CreateTableAsset = true;
            public string TableAssetName;
            public ProgressionMode ProgressionMode = ProgressionMode.Linear;
            public int MaxLevel = 10;
            public float TableBaseValue;
            public float IncrementPerLevel = 1f;
            public List<float> ManualValues = new() { 0f };
            public bool AttachLevelComponent = true;
            public int InitialLevel;
            public bool SaveAppliedLevel = true;

            public bool Expanded
            {
                get => _expanded;
                set => _expanded = value;
            }

            public static StatSetupPreset CreateHealth()
            {
                return new StatSetupPreset
                {
                    Label = "HP",
                    StatAssetName = "HP",
                    StatId = "HP",
                    BaseValue = 100f,
                    SaveValue = false,
                    ClampEnabled = true,
                    UseMin = true,
                    MinValue = 1f,
                    TableAssetName = "HP_LevelProgression",
                    ProgressionMode = ProgressionMode.Linear,
                    MaxLevel = 10,
                    TableBaseValue = 100f,
                    IncrementPerLevel = 20f,
                    AttachLevelComponent = true,
                    InitialLevel = 0,
                    SaveAppliedLevel = true
                };
            }

            public static StatSetupPreset CreateSpeed()
            {
                return new StatSetupPreset
                {
                    Label = "SPEED",
                    StatAssetName = "SPEED",
                    StatId = "SPEED",
                    BaseValue = 7f,
                    SaveValue = false,
                    ClampEnabled = true,
                    UseMin = true,
                    MinValue = 0.1f,
                    TableAssetName = "SPEED_LevelProgression",
                    ProgressionMode = ProgressionMode.Linear,
                    MaxLevel = 10,
                    TableBaseValue = 7f,
                    IncrementPerLevel = 0.5f,
                    AttachLevelComponent = true,
                    InitialLevel = 0,
                    SaveAppliedLevel = true
                };
            }

            public static StatSetupPreset CreateAttack()
            {
                return new StatSetupPreset
                {
                    Label = "ATK",
                    StatAssetName = "ATK",
                    StatId = "ATK",
                    BaseValue = 50f,
                    SaveValue = false,
                    ClampEnabled = true,
                    UseMin = true,
                    MinValue = 1f,
                    TableAssetName = "ATK_LevelProgression",
                    ProgressionMode = ProgressionMode.Linear,
                    MaxLevel = 10,
                    TableBaseValue = 50f,
                    IncrementPerLevel = 10f,
                    AttachLevelComponent = true,
                    InitialLevel = 0,
                    SaveAppliedLevel = true
                };
            }

            public static StatSetupPreset CreateExperience()
            {
                return new StatSetupPreset
                {
                    Label = "EXP",
                    StatAssetName = "EXP",
                    StatId = "EXP",
                    BaseValue = 0f,
                    SaveValue = true,
                    ClampEnabled = true,
                    UseMin = true,
                    MinValue = 0f,
                    TableAssetName = "EXP_LevelProgression",
                    ProgressionMode = ProgressionMode.Manual,
                    ManualValues = new List<float> { 0f, 1f, 3f, 6f, 10f, 15f, 21f, 28f, 36f, 45f, 55f },
                    AttachLevelComponent = false,
                    InitialLevel = 0,
                    SaveAppliedLevel = false
                };
            }
        }

        private enum ProgressionMode
        {
            Linear,
            Manual
        }
    }
}
#endif
