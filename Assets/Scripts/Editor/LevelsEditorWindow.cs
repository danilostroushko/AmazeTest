using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsEditorWindow : OdinEditorWindow
{
    [MenuItem("Game/Levels editor")]
    private static void OpenWindow()
    {
        LevelsEditorWindow editor = GetWindow<LevelsEditorWindow>();
        editor.OnClose += () =>
        {
            EditorApplication.playModeStateChanged -= editor.PlayModeStateChanged;
        };

        editor.Show();
    }

    protected override async void Initialize()
    {
        this.minSize = new Vector2(520, 500);

        base.Initialize();

        EditorApplication.playModeStateChanged -= PlayModeStateChanged;

        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        EditorUtility.DisplayProgressBar("Loading...", "", 0);

        ReloadLevels();

        await UniTask.Delay(1000);
        EditorUtility.ClearProgressBar();

        EditorApplication.playModeStateChanged += PlayModeStateChanged;
    }

    private void PlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                isBusy = true;
                break;
            case PlayModeStateChange.EnteredEditMode:
                isBusy = false;
                break;
        }
    }

    private bool isBusy = false;
    private OdinEditorWindow editedLevelPopup;
    private LevelData currentLevelData;
    private LevelHolder levelHolder;

    [BoxGroup("Edit", CenterLabel = true)]
    [HorizontalGroup("Edit/Top")]
    [Button("Reload levels", Icon = SdfIconType.ArrowRepeat, ButtonHeight = 30), DisableIf("@isBusy==true")]
    private void ReloadLevelsButton()
    {
        isBusy = true;
        ReloadLevels();
        isBusy = false;
    }

    [HorizontalGroup("Edit/Top")]
    [Button("Add new level", Icon = SdfIconType.Plus, ButtonHeight = 30), DisableIf("@isBusy==true")]
    private void AddNewLevelButton()
    {
        isBusy = true;

        var asset = ScriptableObject.CreateInstance<LevelData>();

        int levelId = Levels == null || Levels.Count == 0 ? 1 : Levels.Count + 1;

        asset.Level = levelId;

        string guid = Guid.NewGuid().ToString().Split('-')[0];
        string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/Resources/Game/Levels/LevelData_{guid}.asset");

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        ReloadLevels();

        isBusy = false;
    }

    public void DeleteLevel(LevelData levelData)
    {
        isBusy = true;

        var path = AssetDatabase.GetAssetPath(levelData);

        Levels.Remove(levelData);

        if (AssetDatabase.DeleteAsset(path))
        {
            Levels.ForEach((l, i) =>
            {
                l.Level = i + 1;

                EditorUtility.SetDirty(l);
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        isBusy = false;
    }

    public void EditLevel(LevelData levelData)
    {
        isBusy = true;
        currentLevelData = levelData;

        if (EditorSceneManager.GetActiveScene().name != "LevelEditorScene")
        {
            EditorSceneManager.sceneOpened += OnSceneOpen;
            string path = "Assets/Scenes/LevelEditorScene.unity";
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }
        else
        {
            EditLevelOnScene();
        }
    }

    private void OnSceneOpen(Scene scene, OpenSceneMode mode)
    {
        EditorSceneManager.sceneOpened -= OnSceneOpen;
        EditLevelOnScene();
    }

    private void EditLevelOnScene()
    {
        if (levelHolder == null)
        {
            var sceneObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var sceneObject in sceneObjects)
            {
                if (sceneObject.TryGetComponent<LevelHolder>(out levelHolder))
                {
                    break;
                }
            }
        }

        editedLevelPopup = OdinEditorWindow.CreateOdinEditorWindowInstanceForObject(currentLevelData);
        editedLevelPopup.minSize = new Vector2(500, 750);
        editedLevelPopup.OnClose += OnStopEditLevel;
        editedLevelPopup.Show();

        Selection.activeObject = levelHolder;

        currentLevelData.EditMode = true;
        levelHolder.SetLevelData(currentLevelData);
    }

    private void OnStopEditLevel()
    {
        editedLevelPopup.OnClose -= OnStopEditLevel;

        levelHolder.ClearAll();

        currentLevelData.EditMode = false;
        currentLevelData = null; 
        editedLevelPopup = null;
        isBusy = false;

        this.Repaint();
    }

    [PropertyOrder(999)]
    [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowPaging = false)] 
    [OnCollectionChanged(After = "OnLevelsChanged")]
    [DisableIf("@isBusy==true")]
    public List<LevelData> Levels;

    private void ReloadLevels()
    {
        Levels = Resources.LoadAll<LevelData>("Game/Levels").OrderBy(l => l.Level).ToList();
    }

    private void OnLevelsChanged(CollectionChangeInfo changeInfo, object levelData)
    {
        if (isBusy) { return; }
        isBusy = true;

        Levels.ForEach((l, i) => 
        {
            l.Level = i + 1;

            EditorUtility.SetDirty(l);
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        isBusy = false;
    }

    protected override void OnImGUI()
    {
        base.OnImGUI();
    }
}

public class LevelDataDrawer : OdinValueDrawer<LevelData>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 30);
        if (label != null) { rect = EditorGUI.PrefixLabel(rect, label); }
        
        LevelData value = this.ValueEntry.SmartValue;
        GUILayout.BeginHorizontal();

        var labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fontSize = 16;

        EditorGUI.LabelField(rect.AlignLeft(rect.width - 100), value.GetNameForList(), labelStyle);

        if (SirenixEditorGUI.SDFIconButton(rect.AlignRight(50), "Delete", SdfIconType.Trash))
        {
            if (EditorUtility.DisplayDialog("Delete Level", "Are you sure you want to delete level?", "Delete", "Cancel"))
            {
                EditorWindow.GetWindow<LevelsEditorWindow>().DeleteLevel(value);
            }
        }

        if (SirenixEditorGUI.SDFIconButton(rect.HorizontalPadding(53).AlignRight(80), "Edit", SdfIconType.PencilSquare))
        {
            EditorWindow.GetWindow<LevelsEditorWindow>().EditLevel(value);
        }

        GUILayout.EndHorizontal();
        this.ValueEntry.SmartValue = value;
    }
}