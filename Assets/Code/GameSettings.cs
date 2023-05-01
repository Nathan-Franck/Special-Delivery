using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public string LevelSelectState = "Master-P_levelselect";
    public string SettingsState = "Master-P_settings";
    public string InGameState = "Master-P_gamestatesleeping";

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

    public Triggers Trigger = new Triggers();
}
