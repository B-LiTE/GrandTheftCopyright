using UnityEngine;
using System.Collections;

public class GTA_CameraOrbit : MonoBehaviour {
	
	public float maxSizeCrossfire;
	public float reticuleSize;
	public float minSizeCrossfire;
	public float crosshairSmoothTime = 0.06f;
	public float speed_offsetChange = 15;
	public float camera_PositionOffset_normal;
	public float camera_PositionOffset_aiming;
	public float cameraPositionOffset_current;
		
	[HideInInspector] public Texture2D reticule;
	[HideInInspector] public float crosshairAnimSpeed;
	[HideInInspector] public Crosshair crosshair;
	[HideInInspector] public bool crosshairGTAV;
	[HideInInspector] public float g; 
	[HideInInspector] public float i;

	GTA_Controller owner;
	HealthSystem howner;
	ProtectCameraFromWallClip powner;
	FreeLookCam ipowner;

	[System.Serializable]
	public class Crosshair{
		public Texture2D aim;
		public Texture2D aim_Enemy;
	}
	
	void Update(){
		switch(crosshairGTAV){
			case true:
				A ();
				break;
			case false:
				B();
				break;
		}
	}

	void A (){
		switch(owner.weapons.aimTag){
			case "Enemy":
				reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshairGTAVEnemy;
				break;
			case "Ragdoll":
				if(owner.targetIsDeathNow) reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshairDeath;
				else reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshairRagdoll;
				break;
			default:
				reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshairGTAV;
				break;
		}
	}

	void B(){
		if(owner.weapons.IS_ATTACKING && owner.weapons.currentSlot != WeaponSystemSetup.Weapon_Slots.HEAVY)
			reticuleSize = Mathf.SmoothDamp (reticuleSize, maxSizeCrossfire, ref crosshairAnimSpeed, crosshairSmoothTime);
		else
			reticuleSize = Mathf.SmoothDamp (reticuleSize, minSizeCrossfire, ref crosshairAnimSpeed, crosshairSmoothTime);
			
		switch(owner.weapons.aimTag){
			case "Enemy":
				reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshairEnemy;
				break;
			case "Ragdoll":
				if(owner.targetIsDeathNow) reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshairDeath;
				else reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshairRagdoll;
				break;
			default:
				reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshair;
				break;
		}
	}

	void OnGUI(){
		Rect position = new Rect(g, i, reticuleSize, reticuleSize);
		
		if(owner.aimingWeightControl > 0.95f){
			if (owner.weapons.currentSlot == WeaponSystemSetup.Weapon_Slots.HEAVY){
				g = ((Screen.width - reticuleSize) * 0.5f) - (Screen.width * 0.0375f);
				i = ((Screen.height - reticuleSize) * 0.5f) + (Screen.height * 0.0625f);
			}
			else{
				g = (Screen.width - reticuleSize) * 0.5f;
				i = (Screen.height - reticuleSize) * 0.5f;
			}
			GUI.DrawTexture(position, reticule);
		}
	}
	
	void OnPlayerSpawn(GameObject player){
		cameraPositionOffset_current = camera_PositionOffset_normal;
		Vector3 angles = transform.eulerAngles;
		owner = player.GetComponent<GTA_Controller>();
		owner.c = gameObject.GetComponent<GTA_CameraOrbit>();
		powner = gameObject.GetComponent<ProtectCameraFromWallClip>();
		powner.p = gameObject.GetComponent<GTA_CameraOrbit>();
		ipowner = gameObject.GetComponent<FreeLookCam>();
		ipowner.z = owner;
	}	
}
