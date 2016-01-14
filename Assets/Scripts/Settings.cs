using UnityEngine;
using System.Collections;

[System.Serializable]
public class ScreenResolution{
	public int width = 1024;
	public int height = 576;
	
	public void ShowCurrentResolution(){
		Utils.CLog("[SETTINGS]", "(Build only) Display resolution: " + width + "x" + height, "green");
	}
}

[System.Serializable]
public class CustomInput{
	public KeyCode KEY_TOGGLE_RUN_WALK;
	public KeyCode KEY_RELOAD;
	public KeyCode KEY_TRY_ATTACK;
	public KeyCode KEY_TOGGLE_VIEW;
	public KeyCode KEY_TOGGLE_CROUCH;
	public KeyCode KEY_JUMP;
	
	/*public void ShowCurrentInput(){
		string TEMP_string = "Input: " +
			"$ 	Browse Weapons:  	" + "Mouse Wheel" + 
			"$ 	Walk/Run:       	" + KEY_TOGGLE_RUN_WALK.ToString() +
			"$ 	Reload:         	" + KEY_RELOAD.ToString() +
			"$ 	Attack:         	" + KEY_TRY_ATTACK.ToString() +
			"$ 	Aiming:        		" + KEY_TOGGLE_VIEW.ToString() +
			"$ 	Crouch:        		" + KEY_TOGGLE_CROUCH.ToString() + 
			"$ 	Jump:        		" + KEY_JUMP.ToString();
		
		Utils.CLog("[SETTINGS]", TEMP_string, "green");
	}*/
}

public class Settings : MonoBehaviour {
	public CustomInput customInput;
	public ScreenResolution[] resolutions;
	public int startIndex;
	public float volume = 0.3f;
	public bool resetTimeScale = false;
	
	[HideInInspector]public ScreenResolution currentResolution;
	
	void Start () {
		SetResolution(startIndex, false);
		volume = Mathf.Clamp(volume, 0.0f, 1.0f);
		AudioListener.volume = volume;
		if(resetTimeScale)
			Time.timeScale = 1.0f;
	}
	
	public void SetResolution(int index, bool fs){
		Screen.SetResolution(resolutions[index].width, resolutions[index].height, fs);
		currentResolution = resolutions[index];
	}
	
	public string GetResolutionInString(ScreenResolution input){
		return (input.width.ToString() + "x" + input.height.ToString());
	}
	/*
	void C_ShowInput(){
		customInput.ShowCurrentInput();
	}
	
	void C_ShowDisplayRes(){
		currentResolution.ShowCurrentResolution();
	}
	
	void C_SetDisplayRes(int index){
		if(index > resolutions.Length - 1)
			Utils.CLog("[ERROR]", "Wrong resolution index " + index + ". There are only 0 - " + (resolutions.Length - 1).ToString() + 
				"$Use /alldisplayres command", "red");
		else{
			SetResolution(index, false);
			currentResolution.ShowCurrentResolution();
		}
	}
	
	void C_AllDisplayRes(){
		string TEMP_string = "Avaliable display resolutions";
		for(int i = 0; i < resolutions.Length; i++){
			TEMP_string += "$  " + resolutions[i].width + "x" + resolutions[i].height + "; Index: " + i;
		}	
		Utils.CLog("[SETTINGS]", TEMP_string, "green");
	}
	
	void C_Help(){
		string TEMP_string = "Commands: " +
			"$ 	Use [/cmd value] to apply command." +
			"$ " +
			"$ 	/sysinfo (Watch system information)" +
			"$ 	/cinput (Watch current user input)" +
			"$ 	/setdisplayres (Set display resolution by index )" +
			"$ 	/cdisplayres (Watch current display resolution)" + 
			"$ 	/alldisplayres (Watch all avaliable display resolutions)";
			Utils.CLog("[HELP]", TEMP_string, "orange");
	}
	
	void C_SysInfo(){
		string TEMP_string = "System information: " +
			"$ 	Unity version: " + Application.unityVersion +
			"$ 	Internet Reachability: " + Application.internetReachability.ToString() +
			"$ 	Operating System : " + SystemInfo.operatingSystem +
			"$ 	Processor Type: " + SystemInfo.processorType +
			"$ 	Processor Count: " + SystemInfo.processorCount +
			"$ 	System Memory Size: " + SystemInfo.systemMemorySize +
			"$ 	Graphics Memory Size: " + SystemInfo.graphicsMemorySize +
			"$ 	Graphics Device Name: " + SystemInfo.graphicsDeviceName +
			"$ 	Graphics Shader Level: " + SystemInfo.graphicsShaderLevel;
			Utils.CLog("[SYSINFO]", TEMP_string, "green");
	}*/
}
