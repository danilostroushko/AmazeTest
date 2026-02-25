using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Extensions;
using DG.Tweening;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelHolder : MonoBehaviour
{
    public event Action<float> OnLevelFillEvent;

    [SerializeField] private Transform levelTransform;

    [SerializeField] private Ball ballPrefab;
    [SerializeField] private FloorCell floorCellPrefab;
    [SerializeField] private WallCell wallCellPrefab;

    [SerializeField] private int wallOffsetX = 5;
    [SerializeField] private int wallOffsetZ = 5;

    private List<Ball> balls = new List<Ball>();
    private List<FloorCell> floorCells = new List<FloorCell>();
    private List<WallCell> wallCells = new List<WallCell>();

    private LevelData currentLevel;
    private GameSettings gameSettings;

    public float FillPercent {  get; private set; }

    public void SetLevelData(LevelData levelData)
    {
        currentLevel = levelData;
        gameSettings = Configs.Get<GameSettings>();

#if UNITY_EDITOR
        currentLevel.OnLevelDataChanged += OnLevelDataChanged;
#endif

        BuildLevel();
    }

#if UNITY_EDITOR
    private void OnLevelDataChanged()
    {
        Clear();
        BuildLevel();
    }
#endif

    #region Builder
    private void BuildLevel()
    {
        var grid = currentLevel.LevelGrid;
        if (grid == null || grid.Length == 0) { return; }

        int cWall = currentLevel.GridCols + (wallOffsetX * 2);
        int rWall = currentLevel.GridRows + (wallOffsetZ * 2);
        int totalWalls = cWall * rWall;

        int offsetX = wallOffsetX;
        int offsetZ = wallOffsetZ;

        for (int c = 0; c < cWall; c++)
        {
            for (int r = 0; r < rWall; r++)
            {
                int posX = c - offsetX;
                int posZ = r - offsetZ;

                if (posX.InRange(0, currentLevel.GridCols - 1) && posZ.InRange(0, currentLevel.GridRows - 1))
                {
                    continue;
                }

                var wallCell = CreatePrefab(wallCellPrefab.gameObject).GetComponent<WallCell>();
                if (Application.isPlaying) { wallCell.Init(gameSettings.GetCurrentGameStyle.WallColor); }
                wallCell.transform.localPosition = new Vector3(posX, 0, -posZ);

                wallCells.Add(wallCell);
            }
        }

        for (int c = 0; c < grid.GetLength(0); c++)
        {
            for (int r = 0; r < grid.GetLength(1); r++)
            {
                if (grid[c, r] > 0)
                {
                    var floorCell = CreatePrefab(floorCellPrefab.gameObject).GetComponent<FloorCell>();
                    if (Application.isPlaying) { floorCell.Init(gameSettings.GetCurrentGameStyle.FillCellColor); }
                    floorCell.OnFilled += OnLevelFilled;
                    floorCell.transform.localPosition = new Vector3(c, 0, -r);

                    floorCells.Add(floorCell);

                    if (grid[c, r] == 2)
                    {
                        var ball = CreatePrefab(ballPrefab.gameObject).GetComponent<Ball>();
                        ball.GridPosition = new Vector2Int(c, r);
                        if (Application.isPlaying) { ball.Init(gameSettings.GetCurrentGameStyle.BallColor); }
                        ball.transform.localPosition = new Vector3(c, 0, -r);

                        balls.Add(ball);
                    }
                }
                else
                {
                    var wallCell = CreatePrefab(wallCellPrefab.gameObject).GetComponent<WallCell>();
                    if (Application.isPlaying) { wallCell.Init(gameSettings.GetCurrentGameStyle.WallColor); }
                    wallCell.transform.localPosition = new Vector3(c, 0, -r);

                    wallCells.Add(wallCell);
                }
            }
        }

        levelTransform.localPosition = new Vector3(-(currentLevel.GridCols * 0.5f) + 0.5f, 0, currentLevel.GridRows * 0.5f);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UniTask.Void(async () =>
            {
                await UniTask.Delay(500);
                FitCameraToBounds();
            });
        }
#endif
    }

    #region Camera
    public void FitCameraToBounds()
    {
        Bounds targetBounds = GetTotalBounds(floorCells.Select(c => c.gameObject).ToList());

        if (!Application.isPlaying)
        {
            Camera.main.transform.position = new Vector3(0, 10, targetBounds.center.z - 0.5f);
        }

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = targetBounds.size.x / targetBounds.size.y;

        float orthoSize;
        if (screenRatio >= targetRatio)
        {
            orthoSize = targetBounds.size.y / 2f;
        }
        else
        {
            float cameraWidth = targetBounds.size.x / screenRatio;
            orthoSize = cameraWidth / 2f;
        }

        float size = Mathf.Clamp(orthoSize + 2f, 5.5f, 50);

        if (!Application.isPlaying)
        {
            Camera.main.orthographicSize = size;
        }
        else
        {
            Camera.main.DOOrthoSize(size, 0.5f);
        }
    }
    #endregion

    private void Clear()
    {
        floorCells.ForEach(c => 
        {
            c.OnFilled -= OnLevelFilled;
            if (!Application.isPlaying)
            {
                DestroyImmediate(c.gameObject);
            }
            else
            {
                Destroy(c.gameObject);
            }
        });

        balls.ForEach(c => 
        {
            if (!Application.isPlaying)
            {
                DestroyImmediate(c.gameObject);
            }
            else
            {
                Destroy(c.gameObject);
            }
        });

        wallCells.ForEach(c => 
        {
            if (!Application.isPlaying)
            {
                DestroyImmediate(c.gameObject);
            }
            else
            {
                Destroy(c.gameObject);
            }
        });

        floorCells.Clear();
        balls.Clear();
        wallCells.Clear();
    }

    public void ClearAll()
    {
        Clear();

#if UNITY_EDITOR
        currentLevel.OnLevelDataChanged -= OnLevelDataChanged;
#endif

        currentLevel = null;
    }

    //TODO: Use pool!
    private GameObject CreatePrefab(GameObject prefab)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab, levelTransform);
        }
#endif

        return Instantiate(prefab, levelTransform);
    }

    #endregion

    #region Game
    public void ApplySwipe(SwipeDirection swipe)
    {
        foreach (var ball in balls)
        {
            Vector2Int pos = ball.GridPosition;
            
            if (GetTargetPos(pos, swipe, out var result))
            {
                ball.MoveTo(result, gameSettings.BallMoveSpeed);
            }
        }
    }

    private bool GetTargetPos(Vector2Int start, SwipeDirection swipe, out Vector2Int target)
    {
        target = new Vector2Int(start.x, start.y);
        bool result = false;

        int cols = currentLevel.LevelGrid.GetLength(0);
        int rows = currentLevel.LevelGrid.GetLength(1);
        switch (swipe)
        {
            case SwipeDirection.Left:
                if (start.x == 0) { return false; }
                for (int x = start.x - 1; x >= 0; x--)
                {
                    if (currentLevel.LevelGrid[x, start.y] != 0)
                    {
                        target.x = x;
                        result = true;
                    } else { break; }
                }
                break;
            case SwipeDirection.Right:
                if (start.x == cols - 1) { return false; }
                for (int x = start.x + 1; x < cols; x++)
                {
                    if (currentLevel.LevelGrid[x, start.y] != 0)
                    {
                        target.x = x;
                        result = true;
                    } else { break; }
                }
                break;
            case SwipeDirection.Up:
                if (start.y == 0) { return false; }
                for (int y = start.y - 1; y >= 0; y--)
                {
                    if (currentLevel.LevelGrid[start.x, y] != 0)
                    {
                        target.y = y;
                        result = true;
                    }
                    else { break; }
                }
                break;
            case SwipeDirection.Down:
                if (start.y == rows - 1) { return false; }
                for (int y = start.y + 1; y < rows; y++)
                {
                    if (currentLevel.LevelGrid[start.x, y] != 0)
                    {
                        target.y = y;
                        result = true;
                    }
                    else { break; }
                }
                break;
        }

        return result;
    }

    public Bounds GetTotalBounds(List<GameObject> objects)
    {
        if (objects.Count == 0)
        {
            return new Bounds();
        }

        Renderer renderer = objects[0].GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            return new Bounds(transform.position, Vector3.zero);
        }

        Bounds totalBounds = renderer.bounds;

        for (int i = 1; i < objects.Count; i++)
        {
            renderer = objects[i].GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                totalBounds.Encapsulate(renderer.bounds);
            }
        }

        return totalBounds;
    }

    private void OnLevelFilled()
    {
        float total = floorCells.Count(c => c.Filled);
        FillPercent = total.RangeToPercent(0, floorCells.Count);

        //Debug.Log($"[LevelHolder] > OnFilledCell: {FillPercent:P0}");
        OnLevelFillEvent?.Invoke(FillPercent);
    }
    #endregion


}