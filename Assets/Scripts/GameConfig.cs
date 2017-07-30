using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kleberswf.lib.core;

public class GameConfig : Singleton<GameConfig>{

	public struct PlayerConfig
	{
        public string playerNumber;
		public Color crosshairColor;
        public ControllerType controllerType;
	}

    GameConfig()
	{
		bluePlayerConfig = new PlayerConfig ();
        bluePlayerConfig.playerNumber = "1";
        bluePlayerConfig.crosshairColor = Color.blue;
		bluePlayerConfig.controllerType = ControllerType.KeyboardMouse;

        redPlayerConfig = new PlayerConfig ();
        redPlayerConfig.playerNumber = "2";
        redPlayerConfig.crosshairColor = Color.red;
        redPlayerConfig.controllerType = ControllerType.xBox360Windows;
    }

	public PlayerConfig GetPlayerConfig(string team)
	{
		if (team == "Red") {
			return redPlayerConfig;
		} else {
			return bluePlayerConfig;
		}
	}

	private PlayerConfig bluePlayerConfig;
	private PlayerConfig redPlayerConfig;
}
