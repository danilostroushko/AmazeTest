using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private InputController inputController;
    [SerializeField] private MeshRenderer floorMesh;

    [Title("UI")]
    [SerializeField] private GameHUD gameHUD;
    [SerializeField] private WinLevelPopup winLevelPopup;

    [Title("Prefabs")]
    [SerializeField] private LevelHolder levelHolderPrefab;
    
    private GameSettings gameSettings;
    private Dictionary<int, LevelData> levels = new Dictionary<int, LevelData>();

    private LevelData currentLevelData;
    private LevelHolder currentLevel;

    private LevelHolder prewLevel;

    private bool levelReadyToStart = false;

    private void Awake()
    {
        gameSettings = Configs.Get<GameSettings>();
        levels = Resources.LoadAll<LevelData>("Game/Levels").ToDictionary(l => l.Level);

        floorMesh.material.color = gameSettings.GetCurrentGameStyle.FloorColor;

        inputController.OnSwipe += OnSwipe;

        gameHUD.RestartButton.onClick.AddListener(RestartLevel);
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        levelReadyToStart = false;
        ShowLevel(gameSettings.CurrentLevel);
    }

    private void RestartLevel()
    {
        ShowLevel(gameSettings.CurrentLevel);
    }

    private void ShowLevel(int level)
    {
        Sequence swapLevels = DOTween.Sequence();

        if (currentLevel != null)
        {
            prewLevel = currentLevel;
        }

        currentLevelData = levels[level];
        currentLevel = Instantiate(levelHolderPrefab, transform, false);
        currentLevel.transform.localPosition = new Vector3(0f, 0f, 20f);
        currentLevel.SetLevelData(currentLevelData);

        if (prewLevel != null)
        {
            swapLevels.Append(prewLevel.transform.DOLocalMoveZ(-25f, 1f));
        }

        swapLevels.Join(Camera.main.DOOrthoSize(9, 0.5f));
        swapLevels.Join(currentLevel.transform.DOLocalMoveZ(0f, 1f));
        swapLevels.AppendCallback(() => 
        {
            gameHUD.SetLevel(gameSettings.CurrentLevel);
            gameHUD.SetLevelProgress(currentLevel.FillPercent);

            currentLevel.FitCameraToBounds();

            currentLevel.OnLevelFillEvent += OnLevelFillEvent;
            levelReadyToStart = true;
        });

        swapLevels.OnComplete(() => 
        {
            if (prewLevel != null)
            {
                prewLevel.ClearAll();
                Destroy(prewLevel.gameObject);
                prewLevel = null;
            }
        });

        swapLevels.Play();
    }

    private void OnSwipe(SwipeDirection swipe)
    {
        if (currentLevel != null && levelReadyToStart)
        {
            currentLevel.ApplySwipe(swipe);
        }
    }

    private void OnLevelFillEvent(float value)
    {
        gameHUD.SetLevelProgress(value);

        if (value == 1f)
        {
            levelReadyToStart = false;
            currentLevel.OnLevelFillEvent -= OnLevelFillEvent;
            LevelCompleted().Forget();
        }
    }

    private async UniTask LevelCompleted()
    {
        gameSettings.CurrentLevel = Mathf.Max((int)Mathf.Repeat(gameSettings.CurrentLevel + 1, levels.Count + 1), 1);

        await winLevelPopup.PlayAnimation();
        await UniTask.Delay(500);
        ShowLevel(gameSettings.CurrentLevel);
    }
}