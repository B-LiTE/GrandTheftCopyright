using UnityEngine;
using System.Collections;

public class GTA_Controller_Enemies2 : MonoBehaviour {
	
	public Animator a;

	void Awake(){
		a.SetLayerWeight(1, 1);
		a.SetLayerWeight(2, 1);
	}
}