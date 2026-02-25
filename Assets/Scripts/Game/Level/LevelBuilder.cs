using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject floorCellPrefab;
    [SerializeField] private GameObject wallCellPrefab;

    private List<GameObject> balls = new List<GameObject>();
    private List<GameObject> floorCells = new List<GameObject>();
    private List<GameObject> wallCells = new List<GameObject>();

    public async UniTask RebuildLevel(LevelData levelData)//TODO: add skin data
    {
        ClearLevel();
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            var grid = levelData.LevelGrid;
            if (grid == null ||grid.Length == 0) { return; }
            for (int c = 0; c < grid.GetLength(0); c++)
            {
                for (int r = 0; r < grid.GetLength(1); r++)
                {
                    if (grid[c, r] > 0)
                    {
                        var floorCell = (GameObject)PrefabUtility.InstantiatePrefab(floorCellPrefab, transform);
                        floorCell.transform.localPosition = new Vector3(c, 0, -r);

                        floorCells.Add(floorCell);

                        if (grid[c, r] == 2)
                        {
                            var ball = (GameObject)PrefabUtility.InstantiatePrefab(ballPrefab, transform);
                            ball.transform.localPosition = new Vector3(c, 0, -r);

                            balls.Add(ball);
                        }
                    }
                    else
                    {
                        var wallCell = (GameObject)PrefabUtility.InstantiatePrefab(wallCellPrefab, transform);
                        wallCell.transform.localPosition = new Vector3(c, 0, -r);

                        wallCells.Add(wallCell);
                    }
                    
                }
            }
        }
        else
        {

        }
#else

#endif
        await UniTask.Delay(100);
    }

    public void ClearLevel()
    {
        floorCells.ForEach(c => DestroyImmediate(c.gameObject));
        floorCells.Clear();

        balls.ForEach(c => DestroyImmediate(c.gameObject));
        balls.Clear();

        wallCells.ForEach(c => DestroyImmediate(c.gameObject));
        wallCells.Clear();
    }
}