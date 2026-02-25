using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : SerializedScriptableObject
{
    private const string GAME_CURRENT_LEVEL = "CurrentLevel";
    private const string GAME_STYLE_KEY = "GameStyle";

    [PropertyRange(0.01f, 2f)]
    public float BallMoveSpeed = 1f;
    [Space]
    public List<GameStyleSetting> GameStyles = new List<GameStyleSetting>();

    public int CurrentLevel
    {
        get { return PlayerPrefs.GetInt(GAME_CURRENT_LEVEL, 1); }
        set { PlayerPrefs.SetInt(GAME_CURRENT_LEVEL, value); }
    }

    public GameStyleSetting GetCurrentGameStyle => GameStyles[PlayerPrefs.GetInt(GAME_STYLE_KEY, 0)];
    public void SetCurrentGameStyle(int value) => PlayerPrefs.SetInt(GAME_STYLE_KEY, value);
}

[Serializable]
public class GameStyleSetting
{
    public Color BallColor = new Color();
    public Color FillCellColor = new Color();
    [Space]
    public Color WallColor = new Color();
    [Space]
    public Color FloorColor = new Color();
}