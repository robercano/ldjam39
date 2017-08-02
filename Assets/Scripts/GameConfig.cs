using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kleberswf.lib.core;

public class GameConfig : Singleton<GameConfig>
{

    public struct PlayerConfig
    {
        public string playerNumber;
        public Color crosshairColor;
        public ControllerType unityControllerType;
        public ControlType controlType;
    }

    public enum Team
    {
        Red,
        Blue
    }

    public enum ControlType
    {
        Keyboard,
        Controller,
        AI
    }

    public void SetPlayerController(Team team, ControlType type)
    {
        PlayerConfig newConfig = playerConfig[team];

        newConfig.controlType = type;

        switch (type)
        {
            case ControlType.Keyboard:
                newConfig.unityControllerType = ControllerType.KeyboardMouse;
                break;
            case ControlType.Controller:
                if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer)
                {
                    newConfig.unityControllerType = ControllerType.xBox360Mac;
                }
                else
                {
                    newConfig.unityControllerType = ControllerType.xBox360Windows;
                }
                break;
            case ControlType.AI:
                newConfig.unityControllerType = ControllerType.None;
                break;
        }

        playerConfig[team] = newConfig;
    }

    GameConfig()
    {
        playerConfig = new Dictionary<Team, PlayerConfig>();

        PlayerConfig bluePlayerConfig;

        bluePlayerConfig = new PlayerConfig();
        bluePlayerConfig.playerNumber = "1";
        bluePlayerConfig.crosshairColor = Color.blue;
        bluePlayerConfig.unityControllerType = ControllerType.KeyboardMouse;
        bluePlayerConfig.controlType = ControlType.Controller;

        playerConfig[Team.Blue] = bluePlayerConfig;


        PlayerConfig redPlayerConfig;

        redPlayerConfig = new PlayerConfig();
        redPlayerConfig.playerNumber = "2";
        redPlayerConfig.crosshairColor = Color.red;
        redPlayerConfig.unityControllerType = ControllerType.None;
        redPlayerConfig.controlType = ControlType.AI;

        playerConfig[Team.Red] = redPlayerConfig;
    }

    public PlayerConfig GetPlayerConfig(Team team)
    {
        return playerConfig[team];
    }
    public PlayerConfig GetPlayerConfig(string team)
    {
        if (team == "Red")
        {
            return playerConfig[Team.Red];
        }
        else
        {
            return playerConfig[Team.Blue];
        }
    }

    private PlayerConfig bluePlayerConfig;
    private PlayerConfig redPlayerConfig;

    private Dictionary<Team, PlayerConfig> playerConfig;
}
