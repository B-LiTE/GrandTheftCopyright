using UnityEngine;
using System.Collections;

public class UI_Menu : MonoBehaviour {
	public MenuState menuState;
	public _GameManager _nc;
	public GUIStyle T_style;

	public enum MenuState{
		MainMenu,
		SinglePlayer,
		MultiplayerMenu,
		Multiplayer,
		Settings,
		CreateCharacter,
	}
	
	void Start(){
		_nc = FindObjectOfType(typeof(_GameManager)) as _GameManager;
		menuState = MenuState.MainMenu;
		T_style = GameObject.Find("_GameManager").GetComponent<Console>().textStyle;
	}
	
	void OnGUI () {
		GUI.depth = 1;
		switch (menuState){
			case MenuState.MainMenu : GUI_MainMenu();
				break;
			case MenuState.MultiplayerMenu : GUI_MultiplayerMenu();
				break;
			case MenuState.Multiplayer : GUI_OnlineGame();
				break;
			case MenuState.CreateCharacter: GUI_CreateCharacter();
				break;
		}
	}
	
	void GUI_MainMenu(){
		GUI.BeginGroup(new Rect(50, Screen.height - 250, 230, 210));
		GUI.Box(new Rect(70, 0, 140, 150), "");

		if(GUI.Button(new Rect(80, 30, 120, 27.5f), "Start Game", T_style))
			_nc.StartGame();

		if(GUI.Button(new Rect(80, 65, 120, 27.5f), "Multiplayer", T_style))
			menuState = MenuState.MultiplayerMenu;
		
		if(GUI.Button(new Rect(80, 100, 120, 27.5f), "Quit", T_style))
				Application.Quit();
		
		GUI.EndGroup();
	}

	void GUI_MultiplayerMenu(){
		GUI.BeginGroup(new Rect(50, Screen.height - 250, 230, 210));
		GUI.Box(new Rect(70, 0, 140, 150), "");
		
		if(GUI.Button(new Rect(80, 30, 120, 27.5f), "Start", T_style))
			menuState = MenuState.Multiplayer;
		
		if(GUI.Button(new Rect(80, 65, 120, 27.5f), "Select Character", T_style))
			menuState = MenuState.CreateCharacter;
		
		if(GUI.Button(new Rect(80, 100, 120, 27.5f), "Back", T_style))
			menuState = MenuState.MainMenu;

		GUI.EndGroup();
	}

	void GUI_OnlineGame(){
		GUI.BeginGroup(new Rect(Screen.width / 2 - 250, Screen.height/2 - 115, 400, 220), T_style);
		GUI.Box(new Rect(0, 0, 400, 220), "");
		GUI.Label(new Rect(50, 25, 150, 20), "Connection Settings", T_style);
		GUI.Label(new Rect(50, 50, 50, 20), "IP:", T_style);
		GUI.Label(new Rect(50, 75, 50, 20), "PORT:", T_style);
		
		_nc.connectIP = GUI.TextField(new Rect(105, 50, 150, 20), _nc.connectIP, T_style);
		_nc.connectPORT = int.Parse(GUI.TextField(new Rect(105, 75, 70, 20), _nc.connectPORT.ToString(), T_style));
		
		if(GUI.Button(new Rect(230, 50, 150, 30), "Connect", T_style))
			_nc.Connect();
		
		GUI.Label(new Rect(50, 105, 150, 400), "Server Settings", T_style);
		
		GUI.Label(new Rect(50, 125, 50, 20), "PORT:", T_style);
		GUI.Label(new Rect(50, 150, 50, 20), "Max Pl:", T_style);
		
		_nc.serverPORT = int.Parse(GUI.TextField(new Rect(105, 125, 70, 20), _nc.serverPORT.ToString(), T_style));
		_nc.maxPlayers = int.Parse(GUI.TextField(new Rect(105, 150, 70, 20), _nc.maxPlayers.ToString(), T_style));
		
		if(GUI.Button(new Rect(230, 125, 150, 30), "Start Server", T_style))
			_nc.StartServer();
		
		if(GUI.Button(new Rect(50, 185, 150, 30), "Back", T_style))
			menuState = MenuState.MultiplayerMenu;
		
		GUI.EndGroup();
	}

	void GUI_CreateCharacter(){
		GUI.BeginGroup(new Rect(Screen.width / 2 - 250, Screen.height/2 - 115, 500, 230), T_style);
		GUI.Box(new Rect(0, 0, 500, 230), "");
		GUI.Label(new Rect(10, 35, 50, 20), "Name ", T_style);
		GUI.Label(new Rect(10, 70, 50, 20), "Level " + Mathf.Floor(_nc.playerStats.level), T_style);
		GUI.Label(new Rect(10, 95, 50, 20), "Score " + Mathf.Floor(_nc.playerStats.score), T_style);
		GUI.Label(new Rect(10, 120, 50, 20), "Exp " + Mathf.Floor(_nc.playerStats.exp), T_style);
		_nc.username = GUI.TextField(new Rect(70, 35, 180, 20), _nc.username, T_style);
		
		_nc.playerStats.level = GUI.HorizontalSlider(new Rect(75, 70, 100, 20), _nc.playerStats.level, 0, 10);
		_nc.playerStats.score = GUI.HorizontalSlider(new Rect(75, 95, 100, 20), _nc.playerStats.score, 0, 10);
		_nc.playerStats.exp = GUI.HorizontalSlider(new Rect(75, 120, 100, 20), _nc.playerStats.exp, 0, 10);

		GUI.BeginGroup(new Rect(210, 25, 300, 200), T_style);
			GUI.Box(new Rect(10, 0, 100, 100), "");
			GUI.Label(new Rect(10, 5, 105, 10), "GTA 1", T_style);
			if(GUI.Button(new Rect(0, 15, 135, 135), (Texture2D)Resources.Load("GUITextures/Characters/GTA_Player_M_2"), T_style))
				_nc.MultiplayerPrefab = "GTA_Player_M_2_Network";

			GUI.Box(new Rect(120, 0, 100, 100), "");
			GUI.Label(new Rect(120, 5, 105, 10), "GTA 2", T_style);
			if(GUI.Button(new Rect(105, 15, 135, 135), (Texture2D)Resources.Load("GUITextures/Characters/GTA_Player"), T_style))
				_nc.MultiplayerPrefab = "GTA_Player_Network";
		GUI.EndGroup();

		if(!Utils.IsStringEmpty(_nc.username)){
			if(GUI.Button(new Rect(200, 190, 50, 30), "Ok", T_style)){
				menuState = MenuState.MultiplayerMenu;
			}
		}
		else{
			GUI.Label(new Rect(200, 200, 180, 30), "Enter correct name!", T_style);
		}
		
		if(GUI.Button(new Rect(30, 190, 150, 30), "Back", T_style))
			menuState = MenuState.MultiplayerMenu;

		GUI.EndGroup();
	}
}
