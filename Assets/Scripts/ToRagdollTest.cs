using UnityEngine;
using System.Collections;

[System.Serializable]
public class RagdollSetup{
	public Transform originalBone;
	public Transform ragdollBone;
}

public class ToRagdollTest : MonoBehaviour {
	public GameObject ragdollObject;
	public Transform ragdollRoot;
	public RagdollSetup[] ragdollSetup;
	public bool useRagdoll;
	
	public bool toggle;
	
	void Awake() {
		ragdollObject.SetActive(false);
	}
	
	void FixedUpdate(){
	
	}
	
	void LateUpdate () {
		/*if(!useRagdoll){
			foreach(RagdollSetup setup in ragdollSetup){
				setup.ragdollBone.position = setup.originalBone.position;
				setup.ragdollBone.rotation = setup.originalBone.rotation;
			}
		}*/
		if(useRagdoll){
			foreach(RagdollSetup setup in ragdollSetup){
				setup.originalBone.position = setup.ragdollBone.position;
				setup.originalBone.rotation = setup.ragdollBone.rotation;
			}
		}
	}
	
	void SetRagdoll(bool dir){
		if(dir){
			foreach(RagdollSetup setup in ragdollSetup){
				setup.ragdollBone.position = setup.originalBone.position;
				setup.ragdollBone.rotation = setup.originalBone.rotation;
			}
		}
		if(!dir){
			transform.position = ragdollRoot.position;
			//transform.rotation = ragdollRoot.localRotation;
		}
		ragdollObject.SetActive(dir);
		useRagdoll = dir;
	}
	
	
	/*void OnGUI(){
		if(GUI.Button(new Rect(10, 10, 70, 30), "Ragdoll")){
			toggle = !toggle;
			SetRagdoll(toggle);
		}
	}*/

	void Dead(){
		toggle = !toggle;
		SetRagdoll(toggle);
	}

}
