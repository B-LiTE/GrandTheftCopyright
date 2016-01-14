using UnityEngine;
using System.Collections;

public class UI_Game : MonoBehaviour {
	public bool showControls;
	_GameManager _nc;
	
	void OnNetworkLoadedLevel () {
		_nc = FindObjectOfType(typeof(_GameManager)) as _GameManager;
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) 
			showControls = !showControls;
	}
	
	void OnGUI () {
		GUI.depth = 1;

		if(Network.peerType == NetworkPeerType.Disconnected)
			return;
		if(showControls){
			GUI.BeginGroup(new Rect(30, 50, 300, 200));
			GUI.Box(new Rect(0, 0, 300, 200), "Menu");
			GUI.Label(new Rect(10, 20, 150, 20), "Peer Type: " + Network.peerType.ToString());
			if(GUI.Button(new Rect(30, 80, 240, 30), Network.isClient ? "Disconnect" : "Stop Server" ))
				_nc.Disconnect(150);
			GUI.EndGroup();
			
			GUI.BeginGroup(new Rect(Screen.width - 350, 50, 300, 200));
			GUI.Box(new Rect(0, 0, 300, 200), "Players");
			
			for(int i=0; i<_nc.playerList.Count; i++){
				_GameManager.PlayerInfo iPlayer = (_GameManager.PlayerInfo)(_nc.playerList[i]);
				GUI.Label(new Rect(10, 20 + i*20, 150, 20), "Name: " + iPlayer.username + 
					" Ping: " + Network.GetAveragePing(iPlayer.player));
				if(iPlayer.host)
					GUI.Label(new Rect(170,  20 + i*20, 70, 20), "Host");
				else if(Network.isServer)
					if(GUI.Button(new Rect(170,  20 + i*20, 70, 20), "Kick"))
						_nc.CloseConnection(iPlayer.player, iPlayer.username); 
			}

			GUI.EndGroup();
		}
	}
}
