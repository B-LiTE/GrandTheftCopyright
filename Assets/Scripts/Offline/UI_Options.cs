using UnityEngine;
using System.Collections;

public class UI_Options : MonoBehaviour {
	public bool showControls;
	public float x;
	public float y;
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) 
			showControls = !showControls;
	}

	void OnGUI () {
		GUI.depth = 1;
		if(showControls){
			GUI.BeginGroup(new Rect(30, 50, 300, 200));
			GUI.Box(new Rect(0, 0, 300, 200), "Menu");
			if(GUI.Button(new Rect(x, y, 80, 20), "Exit")){
				Destroy(GameObject.Find("_GameManager"));
				Application.LoadLevel(0);
			}
			GUI.EndGroup();
		}
	}
}
