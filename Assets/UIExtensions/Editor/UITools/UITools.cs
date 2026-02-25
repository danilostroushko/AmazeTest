using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor.Toolbars;
using UnityEditor.Overlays;
using UnityEditor;
using TMPro;

[EditorToolbarElement(AnchorsToCorners_id, typeof(SceneView))]
class AnchorsToCorners : EditorToolbarButton
{
    public const string AnchorsToCorners_id = "UI Tools/AnchorsToCorners";

    public AnchorsToCorners()
    {
        text = "Anchors to Corners";
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UIExtensions/Editor/UITools/Icons/AnchorsToCorners.png");
        tooltip = "Anchors to Corners";
        clicked += OnClick;
    }

    void OnClick()
    {
        if (Selection.transforms == null || Selection.transforms.Length == 0)
        {
            return;
        }
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("AnchorsToCorners");
        var undoGroup = Undo.GetCurrentGroup();

        foreach (Transform transform in Selection.transforms)
        {
            RectTransform t = transform as RectTransform;
            Undo.RecordObject(t, "AnchorsToCorners");
            RectTransform pt = Selection.activeTransform.parent as RectTransform;

            if (t == null || pt == null) return;

            Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                                t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }
        Undo.CollapseUndoOperations(undoGroup);
    }
}

[EditorToolbarElement(CreateImage_id, typeof(SceneView))]
class CreateImage : EditorToolbarButton
{
    public const string CreateImage_id = "UI Tools/CreateImage";

    public CreateImage()
    {
        text = "Create image";
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UIExtensions/Editor/UITools/Icons/CreateImage.png");
        tooltip = "Create image";
        clicked += OnClick;
    }

    void OnClick()
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("CreateImage");
        var undoGroup = Undo.GetCurrentGroup();

        GameObject gameObject = new GameObject("Image", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        if (Selection.transforms != null && Selection.transforms.Length >= 1)
        {
            gameObject.transform.SetParent(Selection.transforms[0], false);
        }
        else
        {
            var canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                gameObject.transform.SetParent(canvas.transform, false);
            }

            
            //gameObject.GetComponent<RectTransform>().anchoredPosition = ?;
        }

        Selection.activeGameObject = gameObject;

        Undo.CollapseUndoOperations(undoGroup);
    }
}

[EditorToolbarElement(CreateText_id, typeof(SceneView))]
class CreateText : EditorToolbarButton
{
    public const string CreateText_id = "UI Tools/CreateText";

    public CreateText()
    {
        text = "Create image";
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UIExtensions/Editor/UITools/Icons/CreateText.png");
        tooltip = "Create image";
        clicked += OnClick;
    }

    void OnClick()
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("CreateText");
        var undoGroup = Undo.GetCurrentGroup();

        GameObject gameObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        if (Selection.transforms != null && Selection.transforms.Length >= 1)
        {
            gameObject.transform.SetParent(Selection.transforms[0], false);
        }
        else
        {
            var canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                gameObject.transform.SetParent(canvas.transform, false);
            }


            //gameObject.GetComponent<RectTransform>().anchoredPosition = ?;
        }

        gameObject.GetComponent<TextMeshProUGUI>().text = "Text";

        Selection.activeGameObject = gameObject;

        Undo.CollapseUndoOperations(undoGroup);
    }
}

[Overlay(typeof(SceneView), "UI Tools")]
public class EditorToolbarUITools : ToolbarOverlay
{
    EditorToolbarUITools() : base(
        AnchorsToCorners.AnchorsToCorners_id,
        CreateImage.CreateImage_id,
        CreateText.CreateText_id
        )
    { }
}