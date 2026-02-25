using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif

//[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
[HideMonoScript]
public class LevelData : SerializedScriptableObject
{
    public event Action OnLevelDataChanged;

    public bool EditMode { get; set; } = false;
    public bool IsValid { get; set; }
    private string invalidMessage = "";

    [ShowInInspector, HideLabel, DisplayAsString(Alignment = TextAlignment.Center, FontSize = 21, EnableRichText = true), PropertyOrder(-1)]
    private string Title => $"<b><color=#FFFFFF>Level {Level}</color></b>";

    [HideInInspector]
    public int Level;

    [Space]
    public LevelDifficulty Difficulty = LevelDifficulty.Low;

    [PropertyRange(3, 30), OnValueChanged("OnGridResize")]
    public int GridCols = 5;
    [PropertyRange(3, 30), OnValueChanged("OnGridResize")]
    public int GridRows = 5; 

    [Title("Grid")]
    [InfoBox("@invalidMessage", InfoMessageType.Error, VisibleIf = "@IsValid==false")]
    [TableMatrix(DrawElementMethod = "DrawGridCell", RowHeight = 20, ResizableColumns = false, SquareCells = true, HideColumnIndices = true, HideRowIndices = true)]
    public int[,] LevelGrid;// 0 - empty; 1 - fill; 2 - ball;

    [OnInspectorInit]
    private void Init()
    {
#if UNITY_EDITOR
        if (LevelGrid == null)
        {
            OnGridResize();
        }

        ValidateLevel();
#endif
    }

    [OnInspectorDispose]
    private void Dispose()
    {
        EditMode = false;
    }

    [Button]
    private void ValidateLevel()
    {
        /*bool isValid = true;

        for (int c = 0; c < LevelGrid.GetLength(0); c++)
        {
            for (int r = 0; r < LevelGrid.GetLength(1); r++)
            {
                if (LevelGrid[c, r] == 0) { continue; }

                bool hasTop = c == 0 ? true : LevelGrid[c - 1, r] > 0;
                bool hasBottom = c == LevelGrid.GetLength(0) - 1 ? true : LevelGrid[c + 1, r] > 0;
                
                bool hasLeft = r == 0 ? true : LevelGrid[c, r - 1] > 0;
                bool hasRight = r == LevelGrid.GetLength(1) - 1 ? true : LevelGrid[c, r + 1] > 0;

                Debug.Log($"\t: {hasTop}, {hasBottom}, {hasLeft}, {hasRight}");
            }
        }*/

        bool containsBall = ContainsBall(out var position);
        bool gridEmpty = LevelGrid.Cast<int>().All(c => c == 0);

        IsValid = containsBall && !gridEmpty;
        invalidMessage = "";
        invalidMessage += !containsBall ? "Required ball! " : "";
        invalidMessage += gridEmpty ? "Grid is empty! " : "";
    }

    [ShowIf("@EditMode==true"), Button(ButtonHeight = 50)]
    private void ApplyChanges()
    {
        OnLevelDataChanged?.Invoke();
    }

    public string GetNameForList() => $"Level {Level}, difficulty: {Difficulty}, size: {GridCols}x{GridRows}";

    #region Editor
    private static Color emptyColor = new Color(0.4f, 0.4f, 0.4f);
    private static Color fillColor = new Color(0.44f, 1f, 0.44f);

#if UNITY_EDITOR
    private void OnGridResize()
    {
        int[,] newLevelGrid = new int[GridCols, GridRows];
        if (LevelGrid != null)
        {
            int minX = Math.Min(LevelGrid.GetLength(0), GridCols);
            int minY = Math.Min(LevelGrid.GetLength(1), GridRows);

            for (int i = 0; i < minX; i++)
            {
                for (int j = 0; j < minY; j++)
                {
                    newLevelGrid[i, j] = LevelGrid[i, j];
                }
            }
        }

        LevelGrid = newLevelGrid;

        EditorUtility.SetDirty(this);
    }

    private int DrawGridCell(Rect rect, int value)
    {
        if (rect.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.MouseDown)
            {
                Undo.RecordObject(this, "GridChange");

                int nextValue = (int)Mathf.Repeat(value + 1, 3);
                if (nextValue == 2 && ContainsBall(out var result))
                {
                    LevelGrid[result.Item1, result.Item2] = 1;
                }

                value = nextValue;

                UnityEditorEventUtility.DelayAction(ValidateLevel);

                GUI.changed = true;
                Event.current.Use();
            }
        }

        if (value == 2)
        {
            SdfIcons.DrawIcon(rect.Padding(10), SdfIconType.CircleFill, Color.green, fillColor);
        }
        else
        {
            EditorGUI.DrawRect(rect.Padding(1), value == 0 ? emptyColor : fillColor);
        }

        EditorUtility.SetDirty(this);

        return value;
    }
#endif

    private bool ContainsBall(out (int, int) position)
    {
        position = (-1, -1);

        for (int c = 0; c < LevelGrid.GetLength(0); c++)
        {
            for (int r = 0; r < LevelGrid.GetLength(1); r++)
            {
                if (LevelGrid[c, r].Equals(2))
                {
                    position = (c, r);
                    return true;
                }
            }
        }

        return false;
    }
    #endregion
}