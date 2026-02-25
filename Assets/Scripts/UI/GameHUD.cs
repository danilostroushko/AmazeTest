using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Slider levelProgressSlider;
    [SerializeField] private TextMeshProUGUI levelProgressLabel;

    public Button RestartButton => restartButton;

    public void SetLevel(int level)
    {
        levelLabel.text = $"LEVEL {level}";
        levelProgressLabel.text = "";
        levelProgressSlider.value = 0f;
    }

    public void SetLevelProgress(float value)
    {
        levelProgressLabel.text = $"{value:P0}";
        levelProgressSlider.DOValue(value, 0.1f).SetEase(Ease.Linear);
    }
}