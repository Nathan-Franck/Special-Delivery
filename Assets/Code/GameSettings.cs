using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public string GameState = "Master-P_gamestatesleeping";
    public string MenuState = "Master-P_menuloop";
    public string LevelSelectState = "Master-P_levelselect";
    public string SettingsState = "Master-P_settings";
    public string InGameState = "Master-P_gamestatesleeping";

    [System.Serializable]
    public class GameTweaks
    {
        public string ObstacleTag = "Obstacle";
        public int ObstacleTimePunish = 5;
        public int StandardTime = 30;
    }

    public GameTweaks Tweaks = new GameTweaks();
    public Level[] Levels;
    [System.Serializable]
    public struct Level
    {
        public int TimeLimit;
        public int ObstaclePunishment;
        public GameObject Prefab;
    }

    [System.Serializable]
    public class Triggers
    {
        public string ClickSettings = "ClickSettings";
        public string ClickBack = "ClickBack";
        public string ClickNewGame = "ClickNewGame";
        public string ClickLevel = "ClickLevel";
        public string GameFail = "GameFail";
        public string GameVictory = "GameVictory";
        public string GameComplete = "GameComplete";
    }

    [System.Serializable]
    public class FXTriggers
    {
        public string MusicNote = "MusicNote";
        public string ExclamationPoint = "ExclamationPoint";
        public string FrustrationVessle = "FrustrationVessle";
    }

    public Triggers Trigger = new Triggers();
    public FXTriggers FXTrigger = new FXTriggers();
}
