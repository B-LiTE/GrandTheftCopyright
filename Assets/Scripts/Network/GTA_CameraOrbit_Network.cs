using UnityEngine;
using System.Collections;

public class GTA_CameraOrbit_Network : MonoBehaviour {

	public Vector3 cameraRotationPoint_normal;
	public Vector3 cameraRotationPoint_aiming;
	public Vector3 cameraRotationPoint_crouching_aiming;
	public Vector3 camera_PositionOffset_normal;
	public Vector3 camera_PositionOffset_aiming;
	public Vector3 camera_PositionOffset_crouching_aiming;
	public float speed_offsetChange = 15.0f;
	public LayerMask collisionLayers = -1;
	[Range(5,9)]	public float zoom_Dampening = 5.0f;
	
	[HideInInspector]	public bool on;
	[HideInInspector][Range(0, 4)]	public float curDist;
	[HideInInspector]	public float finalDist;
	[HideInInspector]	public float xInicial;
	[HideInInspector]	public float xInicialAiming;
	[HideInInspector]	public float zInicial;
	[HideInInspector]	public float x = 0.0f, y = 0.0f;
	[HideInInspector]	public Quaternion cRotation;
	[HideInInspector]	public Vector3 cPosition;
	[HideInInspector]	public Vector3 cameraRotationPoint_current;
	[HideInInspector]	public Vector3 cameraPositionOffset_current;
	[HideInInspector]	public Transform target;

	public float maxSizeCrossfire;
	public float reticuleSize;
	public float minSizeCrossfire;
	public float crosshairSmoothTime = 0.06f;
	[HideInInspector] public Texture2D reticule;
	[HideInInspector] public float crosshairAnimSpeed;
	[HideInInspector] public Crosshair crosshair;
	[HideInInspector] public bool crosshairGTAV;
	[HideInInspector] public float g; 
	[HideInInspector] public float i;

	GTA_Controller_Network owner;

	[System.Serializable]
	public class Crosshair{
		public Texture2D aim;
		public Texture2D aim_Enemy;
	}

	public Vector3 CalculateCameraPosition(Vector3 _rotPoint, Vector3 _posOffset, Quaternion _rotation){
		return  target.position + _rotation * new Vector3(_posOffset.x, _posOffset.y, -_posOffset.z) + new Vector3(_rotPoint.x, _rotPoint.y, _rotPoint.z);
	}

	void LateUpdate(){
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
			case "Player":
				reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshairGTAVEnemy;
				break;
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
		if(owner.weapons.IS_ATTACKING && owner.weapons.currentSlot != WeaponSystemSetupNetwork.Weapon_Slots.HEAVY)
			reticuleSize = Mathf.SmoothDamp (reticuleSize, maxSizeCrossfire, ref crosshairAnimSpeed, crosshairSmoothTime);
		else
			reticuleSize = Mathf.SmoothDamp (reticuleSize, minSizeCrossfire, ref crosshairAnimSpeed, crosshairSmoothTime);
		
		switch(owner.weapons.aimTag){
			case "Player":
				reticule = owner.weapons.slots[(int) owner.weapons.currentSlot].WEAPON_INFO.crosshairEnemy;
				break;
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
			if (owner.weapons.currentSlot == WeaponSystemSetupNetwork.Weapon_Slots.HEAVY){
				g = ((Screen.width - reticuleSize) * 0.5f) - (Screen.width * 0.0375f);
				i = ((Screen.height - reticuleSize) * 0.5f) + (Screen.width * 0.0375f);
			}
			else{
				g = (Screen.width - reticuleSize) * 0.5f;
				i = (Screen.height - reticuleSize) * 0.5f;
			}
			GUI.DrawTexture(position, reticule);
		}
	}
	
	void OnPlayerSpawn(GameObject player){
		target = player.transform;
		owner = player.GetComponent<GTA_Controller_Network>();
		owner.c = gameObject.GetComponent<GTA_CameraOrbit_Network>();
		cameraRotationPoint_current = cameraRotationPoint_normal;
		cameraPositionOffset_current = camera_PositionOffset_normal;
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}
}