using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponSystemSetupNetwork{
	public Transform weaponBone;
	public Weapon_Slot[] slots;
	public Hit_Event[] hitEvents;
	public Decal_Prefab[] decals;
	public LayerMask hitLayers;
	public EffectsPhysics[] LayersEffectsPhysics;
	
	[HideInInspector] public Weapon_Slots currentSlot, lastSlot;
	[HideInInspector] public float NEXT_SELECTION_EVENT;
	[HideInInspector] public string aimTag;
	
	[System.Serializable]
	public class Weapon_Slot{
		public string NAME_WEAPON;
		public Weapon_Slots SLOT;
		public int ID;
		public int TYPES;
		public int HAS_AMMO;
		public int AMMO_IN_CLIP;
		public int POWER_IMPACT;
		public HIT_DAMAGE_BODY[] DamageBodyParts;
		
		[HideInInspector] public bool USED = false;
		[HideInInspector] public float NEXT_ATTACK_EVENT;
		[HideInInspector] public float NEXT_RELOAD_EVENT;
		[HideInInspector] public GTA_WeaponManager WEAPON_OBJECT;
		
		public bool HAS_WEAPON_OBJECT{
			get{
				return WEAPON_OBJECT ? true : false;
			}
		}
		
		public GTA_Weapons_Network.Weapon_Flags FLAGS{
			get{
				return GTA_Weapons_Network.GET_WEAPON_SLOT_FLAGS(SLOT, ID);
			}
		}
		
		public GTA_Weapons_Network.Weapon_Info WEAPON_INFO{
			get{
				return GTA_Weapons_Network.GET_WEAPON_INFO(SLOT, ID);
			}
		}
	}
	
	[System.Serializable]
	public class EffectsPhysics{
		public string layerName;
	}
	
	[System.Serializable]
	public class Hit_Event{
		public string hitTag;
		public Transform root;
		public AudioClip clip;
		public Particle_Type selectEmmitter;
		public ParticleEmitter[] emmitt_Legacy;
		public ParticleSystem[] emitt_Shuriken;
	}
	
	public enum Particle_Type {
		Legacy,
		Shuriken,
		Both,
	}
	
	[System.Serializable]
	public class Decal_Prefab{
		public string hitTag;
		public GameObject[] decal;
	}
	
	public enum Weapon_Slots{
		UNARMED,
		MELEE,
		HANDGUN,
		SUBMACHINEGUN,
		SHOTGUN,
		RIFLE,
		SNIPER,
		HEAVY,
	}
	
	[System.Serializable]
	public class HIT_DAMAGE_BODY{
		public string Body_Part_Tag;
		public int Damage;
	}

	public bool CLIP_EMPTY{
		get{
			return !(slots[(int)currentSlot].AMMO_IN_CLIP > 0);
		}
	}
	
	public bool CLIP_FULL{
		get{
			return slots[(int)currentSlot].AMMO_IN_CLIP == (int)slots[(int)currentSlot].WEAPON_INFO.CLIP_SIZE;
		}
	}
	
	public bool HAS_AMMO{
		get{
			return slots[(int)currentSlot].HAS_AMMO > 0;
		}
	}
	
	public bool IS_SELECTION{
		get{
			return !(Time.fixedTime >= NEXT_SELECTION_EVENT);
		}
	}
	
	public bool IS_ATTACKING{
		get{
			return !(Time.fixedTime >= slots[(int)currentSlot].NEXT_ATTACK_EVENT);
		}
	}
	
	public bool IS_RELOADING{
		get{
			return !(Time.fixedTime >= slots[(int)currentSlot].NEXT_RELOAD_EVENT);
		}
	}
	
	public bool CAN_AIM{
		get{
			return slots[(int)currentSlot].FLAGS.CAN_AIM && !IS_RELOADING && !IS_SELECTION;
		}
	}
	
	public GTA_Weapons_Network.Weapon_Motion_Data CURRENT_WEAPON_MOTION_DATA{
		get{
			return GTA_Weapons_Network.GET_WEAPON_MOTION_DATA(currentSlot, slots[(int)currentSlot].ID);
		}
	}
}

[System.Serializable]
public class FootStepsNetwork{
	public AudioSource A_Source;
	public LayerMask floorLayer;
	public BoxCollider RToe;
	public BoxCollider LToe;
	public Audio[] sound;
	
	[System.Serializable]
	public class Audio {
		public string onTag;
		public AudioClip[] soundFootSteep; 
	}
}

public class GTA_Controller_Network : MonoBehaviour {
	public Animator a;
	public float mouseXSens = 5.0f, mouseYSens = 5.0f;
	public float V_LookAngleThreshold = 50.0f;
	public Transform[] upperBodyChain;
	public WeaponSystemSetupNetwork weapons;
	public FootStepsNetwork footSteps;
	public GUIStyle UIStyle;
	public bool SHOW_DEBUG_INFO = true;
	public static CustomInput cInput;
	
	Vector3 rotateDirection_current;
	Vector3 rotateDirection_target;
	Vector3 moveDirection_current;
	Vector3 cameraForward;
	
	[HideInInspector]	public bool isGrounded;
	[HideInInspector]	public bool targetIsDeath;
	[HideInInspector]	public bool targetIsDeathNow;
	[HideInInspector]	public float V_LookAngle;
	[HideInInspector]	public float aimingWeightControl;
	[HideInInspector]	public float h, v, mouseX, mouseY;
	[HideInInspector]	public bool SYNC_is_aiming;
	[HideInInspector]	public bool SYNC_is_reloading;
	[HideInInspector]	public float SYNC_animator_move_speed;
	[HideInInspector]	public GTA_CameraOrbit_Network c;

	public bool IS_CROUCHING{
		get{
			return Input.GetKey(GTA_Controller_Network.cInput.KEY_TOGGLE_CROUCH);
		}
	}
	
	public bool IS_AIMING{
		get{
			return weapons.CAN_AIM ? Input.GetKey(GTA_Controller_Network.cInput.KEY_TOGGLE_VIEW) : false;
		}
	}
	
	public float ANIMATOR_MOVE_SPEED{
		get{
			return IS_MOVING ? (Input.GetKey(GTA_Controller_Network.cInput.KEY_TOGGLE_RUN_WALK) && !weapons.IS_RELOADING ? 1.0f : 0.5f) : 0.0f;
		}
	}
	
	public bool IS_MOVING{
		get{
			return !(h == 0 && v == 0);
		}
	}
	
	public float VELOCITY_MULTIPLER{
		get{
			if(IS_MOVING){
				if(!IS_CROUCHING)
					return IS_AIMING ? 1.3f : (Input.GetKey(cInput.KEY_TOGGLE_RUN_WALK) && !weapons.IS_RELOADING ? 3.4f : 1.5f);
				return IS_AIMING ? 0.9f : 1.1f;
			}
			return 0.0f;
			//return IS_MOVING ? (IS_AIMING ? 1.3f : (Input.GetKey(cInput.KEY_TOGGLE_RUN_WALK) && !weapons.IS_RELOADING ? 3.4f : 1.5f)) : 0.0f;
		}
	}
	
	public float ROTATION_SPEED_MULTIPLER{
		get{
			return Input.GetKey(cInput.KEY_TOGGLE_RUN_WALK) ? 190.0f : 130.0f;
		}
	}
	
	public bool RELOAD_CONDITION{
		get{
			return Input.GetKey(cInput.KEY_RELOAD) && !weapons.CLIP_FULL || weapons.CLIP_EMPTY && !weapons.IS_ATTACKING;
		}
	}
	
	public bool ATTACK_CONDITION{
		get{
			return Input.GetKey(cInput.KEY_TRY_ATTACK);
		}
	}
	
	void Awake () { 
		gameObject.name = (GetComponent<NetworkView>().isMine ? "Local" : "Remote") + "_Player";
		a.SetLayerWeight(1, 1);
		a.SetLayerWeight(2, 1);

		if(GetComponent<NetworkView>().isMine){
			cInput = Utils.LoadInput();
			foreach (GameObject go in FindObjectsOfType(typeof (GameObject)))
				go.SendMessage("OnPlayerSpawn", gameObject, SendMessageOptions.DontRequireReceiver);
			moveDirection_current = transform.TransformDirection(Vector3.forward);
			
			GetComponent<NetworkView>().RPC("AddOrReplaceWeapon", RPCMode.AllBuffered, (int)WeaponSystemSetupNetwork.Weapon_Slots.UNARMED, weapons.slots[(int)weapons.currentSlot].ID);
			GetComponent<NetworkView>().RPC("AddOrReplaceWeapon", RPCMode.AllBuffered, (int)WeaponSystemSetupNetwork.Weapon_Slots.HANDGUN, weapons.slots[(int)weapons.currentSlot].ID);
			GetComponent<NetworkView>().RPC("AddOrReplaceWeapon", RPCMode.AllBuffered, (int)WeaponSystemSetupNetwork.Weapon_Slots.SUBMACHINEGUN, weapons.slots[(int)weapons.currentSlot].ID);
			GetComponent<NetworkView>().RPC("AddOrReplaceWeapon", RPCMode.AllBuffered, (int)WeaponSystemSetupNetwork.Weapon_Slots.SHOTGUN, weapons.slots[(int)weapons.currentSlot].ID);
			GetComponent<NetworkView>().RPC("AddOrReplaceWeapon", RPCMode.AllBuffered, (int)WeaponSystemSetupNetwork.Weapon_Slots.SHOTGUN, weapons.slots[(int)weapons.currentSlot].ID);
			GetComponent<NetworkView>().RPC("AddOrReplaceWeapon", RPCMode.AllBuffered, (int)WeaponSystemSetupNetwork.Weapon_Slots.RIFLE, weapons.slots[(int)weapons.currentSlot].ID);
			GetComponent<NetworkView>().RPC("AddOrReplaceWeapon", RPCMode.AllBuffered, (int)WeaponSystemSetupNetwork.Weapon_Slots.RIFLE, weapons.slots[(int)weapons.currentSlot].ID);
			GetComponent<NetworkView>().RPC("AddOrReplaceWeapon", RPCMode.AllBuffered, (int)WeaponSystemSetupNetwork.Weapon_Slots.HEAVY, weapons.slots[(int)weapons.currentSlot].ID);
			weapons.currentSlot = TryToSelectSlot(WeaponSystemSetupNetwork.Weapon_Slots.HANDGUN);
			
			c.xInicial = c.camera_PositionOffset_normal.x;
			c.xInicialAiming = c.camera_PositionOffset_aiming.x;
			c.zInicial = c.camera_PositionOffset_normal.z;
		}
	}
	
	void Update () {
		if(GetComponent<NetworkView>().isMine){
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
			mouseX = Input.GetAxis("Mouse X");
			mouseY = Input.GetAxis("Mouse Y");
			
			V_LookAngle -= mouseY * mouseYSens;
			V_LookAngle = Utils.ClampAngle(V_LookAngle, -V_LookAngleThreshold, V_LookAngleThreshold);
			
			if(!IS_AIMING && !weapons.IS_SELECTION){
				Update_WeaponSelection();
			}
			
			if(weapons.lastSlot != weapons.currentSlot){
				GetComponent<NetworkView>().RPC("CreateWeaponObject", RPCMode.AllBuffered, (int)weapons.currentSlot, (int)weapons.lastSlot);
				weapons.slots[(int)weapons.currentSlot].NEXT_RELOAD_EVENT = Time.fixedTime;
				weapons.lastSlot = weapons.currentSlot;
			}
			
			if(ATTACK_CONDITION){
				TryAttack();
			}
			
			if(RELOAD_CONDITION){
				TryReload();
			}
		}
		Update_Animator();
	}
	
	void LateUpdate(){
		LUpdate_CameraMovement();
		LUpdate_CalculatePlayerMovement();
		LUpdate_VLookAngle();
	}
	
	void FixedUpdate(){
		if(IS_AIMING) CalculateDistance(100);
		FUpdate_ApplyPlayerMovement();
		ZoomInWalls();
	}
	
	void Update_WeaponSelection(){
		if(Input.GetAxis("Mouse ScrollWheel") > 0){
			Stop_Attack();
			Stop_Reload();
			weapons.currentSlot = SelectUp();
		}
		
		if(Input.GetAxis("Mouse ScrollWheel") < 0){
			Stop_Attack();
			Stop_Reload();
			weapons.currentSlot = SelectDown();
		}
	}
	
	WeaponSystemSetupNetwork.Weapon_Slots SelectUp(){
		int TEMP_currentSlot = (int)weapons.currentSlot;
		weapons.NEXT_SELECTION_EVENT = Time.fixedTime + 0.5f;
		do{
			if (weapons.slots[(int)weapons.currentSlot].TYPES > 0 && weapons.slots[(int)weapons.currentSlot].ID < weapons.slots[(int)weapons.currentSlot].TYPES){
				weapons.slots[(int)weapons.currentSlot].ID++;
				GetComponent<NetworkView>().RPC("CreateWeaponObject", RPCMode.AllBuffered, (int)weapons.currentSlot, (int)weapons.lastSlot);
			}
			else{
				TEMP_currentSlot++;
				weapons.slots[(int)weapons.currentSlot].ID = 0;
				GetComponent<NetworkView>().RPC("CreateWeaponObject", RPCMode.AllBuffered, (int)weapons.currentSlot, (int)weapons.lastSlot);
			}
			
			if(TEMP_currentSlot == weapons.slots.Length){
				TEMP_currentSlot = 0;
				return (WeaponSystemSetupNetwork.Weapon_Slots)TEMP_currentSlot;
			}
		}
		while(!weapons.slots[TEMP_currentSlot].USED);
		return (WeaponSystemSetupNetwork.Weapon_Slots)TEMP_currentSlot;
	}
	
	WeaponSystemSetupNetwork.Weapon_Slots SelectDown(){
		int TEMP_currentSlot = (int)weapons.currentSlot;
		weapons.NEXT_SELECTION_EVENT = Time.fixedTime + 0.5f;
		do{
			if(TEMP_currentSlot == 0)
				TEMP_currentSlot = weapons.slots.Length;
			if (weapons.slots[(int)weapons.currentSlot].TYPES > 0 && weapons.slots[(int)weapons.currentSlot].ID > 0){
				weapons.slots[(int)weapons.currentSlot].ID--;
				GetComponent<NetworkView>().RPC("CreateWeaponObject", RPCMode.AllBuffered, (int)weapons.currentSlot, (int)weapons.lastSlot);
			}
			else{
				TEMP_currentSlot--;
				weapons.slots[(int)weapons.currentSlot].ID = 0;
				GetComponent<NetworkView>().RPC("CreateWeaponObject", RPCMode.AllBuffered, (int)weapons.currentSlot, (int)weapons.lastSlot);
			}
		}
		while(!weapons.slots[TEMP_currentSlot].USED);
		return (WeaponSystemSetupNetwork.Weapon_Slots)TEMP_currentSlot;
	}
	
	void Update_Animator(){
		a.SetFloat("h", h, 0.15f, Time.deltaTime);
		a.SetFloat("v", v, 0.15f, Time.deltaTime);
		a.SetBool("aiming", GetComponent<NetworkView>().isMine ? IS_AIMING : SYNC_is_aiming);
		a.SetBool("crouching", IS_CROUCHING);
		a.SetFloat("MType_Move", (GetComponent<NetworkView>().isMine ? IS_AIMING : SYNC_is_aiming) ? weapons.CURRENT_WEAPON_MOTION_DATA.MOTION_TYPE_MOVEMENT_DIRECTIONAL : weapons.CURRENT_WEAPON_MOTION_DATA.MOTION_TYPE_MOVEMENT_BASE);
		a.SetFloat("MType_Attack", weapons.CURRENT_WEAPON_MOTION_DATA.MOTION_TYPE_ATTACK);
		a.SetFloat("MType_Reload", weapons.CURRENT_WEAPON_MOTION_DATA.MOTION_TYPE_RELOAD);
		a.SetFloat("bSpeed", GetComponent<NetworkView>().isMine ? ANIMATOR_MOVE_SPEED : SYNC_animator_move_speed, 0.35f, Time.deltaTime);
	}
	
	void FUpdate_ApplyPlayerMovement(){
		if(GetComponent<NetworkView>().isMine){
			if(isGrounded){
				Vector3 velocity = GetComponent<Rigidbody>().velocity;
				Vector3 velocityChange = (moveDirection_current - velocity);
					
				velocityChange.x = Mathf.Clamp(velocityChange.x, -10.0f, 10.0f);
		        velocityChange.z = Mathf.Clamp(velocityChange.z, -10.0f, 10.0f);
		        velocityChange.y = 0;
		
				GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
				
				if (Input.GetKey(cInput.KEY_JUMP)) {
		            GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, Mathf.Sqrt(2 * 0.5f * (-Physics.gravity.y)), velocity.z);
		        }
			}
			GetComponent<Rigidbody>().AddForce(new Vector3 (0, Physics.gravity.y * GetComponent<Rigidbody>().mass, 0));
			isGrounded = false;
		}
	}
	
	void LUpdate_CalculatePlayerMovement(){
		aimingWeightControl = Mathf.MoveTowards(aimingWeightControl, (GetComponent<NetworkView>().isMine ? IS_AIMING : SYNC_is_aiming) ? 1.0f : 0.0f, Time.deltaTime * 5.0f);
		
		if(GetComponent<NetworkView>().isMine){
			cameraForward = c.transform.TransformDirection(Vector3.forward);
			cameraForward.y = 0;
			
			rotateDirection_target = h * new Vector3(cameraForward.z, 0, -cameraForward.x) + v * cameraForward;
			
			Vector3 TEMP_rotateDirection_normal = Vector3.RotateTowards(rotateDirection_current, rotateDirection_target + transform.forward, ROTATION_SPEED_MULTIPLER * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
			Vector3 TEMP_rotateDirection_facing = cameraForward;
			rotateDirection_current = Vector3.Slerp(TEMP_rotateDirection_normal, TEMP_rotateDirection_facing, aimingWeightControl);
			rotateDirection_current.Normalize();
			
			if(rotateDirection_current != Vector3.zero)
				transform.rotation = Quaternion.LookRotation(rotateDirection_current);
			
			Vector3 TEMP_moveDirection_facing = transform.TransformDirection(new Vector3(h, 0, v));
			TEMP_moveDirection_facing.Normalize();
			
			moveDirection_current = IS_AIMING ? TEMP_moveDirection_facing : rotateDirection_current;
			
			moveDirection_current *= VELOCITY_MULTIPLER;	
			moveDirection_current.y = 0.0f;
		}
	}

	public AudioClip SelectFootStepSound(){
		RaycastHit hit;
		Transform t = gameObject.transform;
		AudioClip soundFootStep = null;
		string floorTag = null;
		
		if (Physics.Raycast(t.position + new Vector3(0, 0.5f, 0), -Vector3.up, out hit, Mathf.Infinity, footSteps.floorLayer))
			floorTag = hit.collider.tag;
		
		foreach(FootStepsNetwork.Audio audio in footSteps.sound){
			if(audio.onTag == floorTag)
				soundFootStep = audio.soundFootSteep[Random.Range( 0, audio.soundFootSteep.Length)];
		}
		
		return soundFootStep;
	}
	
	void FootSteps(){
		if(Input.GetKey(cInput.KEY_TOGGLE_RUN_WALK)) footSteps.A_Source.pitch = 1;
		else footSteps.A_Source.pitch = 0.25f;
		footSteps.A_Source.PlayOneShot(SelectFootStepSound());
		footSteps.A_Source.velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
	}
	
	void LUpdate_CameraMovement(){
		if(GetComponent<NetworkView>().isMine){
			c.x += mouseX * mouseXSens;
			c.y -= mouseY * mouseYSens;
			
			c.y = V_LookAngle;
			
			c.cameraRotationPoint_current = Vector3.MoveTowards(c.cameraRotationPoint_current, IS_AIMING ? (IS_CROUCHING ? c.cameraRotationPoint_crouching_aiming : c.cameraRotationPoint_aiming) : c.cameraRotationPoint_normal, Time.deltaTime * c.speed_offsetChange);
			c.cameraPositionOffset_current = Vector3.MoveTowards(c.cameraPositionOffset_current, IS_AIMING ? (IS_CROUCHING ? c.camera_PositionOffset_crouching_aiming : c.camera_PositionOffset_aiming) : c.camera_PositionOffset_normal, Time.deltaTime * c.speed_offsetChange);
			
			c.cRotation = Quaternion.Euler(c.y, c.x, 0);
			c.cPosition = c.CalculateCameraPosition(c.cameraRotationPoint_current, c.cameraPositionOffset_current, c.cRotation);
			c.transform.position = c.cPosition;
			c.transform.rotation = c.cRotation;
		}
	}
	
	void ZoomInWalls (){
		Vector3 truePlayerPos = new Vector3 (transform.position.x, transform.position.y + 1.75f, transform.position.z);
		RaycastHit camera;
		if(Physics.Linecast(truePlayerPos, c.cPosition, out camera, c.collisionLayers.value)){
			c.finalDist = Vector3.Distance(truePlayerPos, camera.point);
			c.on = true;
			Debug.DrawLine(truePlayerPos, c.cPosition, Color.blue);
		}
		else{
			c.on = false;
			Debug.DrawLine(truePlayerPos, c.cPosition, Color.red);
		}
		
		if(!c.on && aimingWeightControl <= 0.95f){
			c.curDist = Mathf.Lerp(c.curDist, c.zInicial, Time.smoothDeltaTime * c.zoom_Dampening);
			c.camera_PositionOffset_normal.z =  Mathf.Lerp(c.camera_PositionOffset_normal.z, c.curDist, Time.smoothDeltaTime * c.zoom_Dampening);
			c.camera_PositionOffset_normal.x =  Mathf.Lerp(c.camera_PositionOffset_normal.x, c.xInicial, Time.smoothDeltaTime * c.zoom_Dampening);
		}
		else{
			c.curDist = Mathf.Lerp(c.curDist, c.finalDist, Time.smoothDeltaTime * c.zoom_Dampening);
			c.camera_PositionOffset_normal.z =  Mathf.Lerp(c.camera_PositionOffset_normal.z, c.curDist, Time.smoothDeltaTime * c.zoom_Dampening);
			c.camera_PositionOffset_normal.x =  Mathf.Lerp(c.camera_PositionOffset_normal.x, (c.xInicial + c.xInicialAiming)/2 - (c.xInicial + c.xInicialAiming)/10, Time.smoothDeltaTime * c.zoom_Dampening);
		}
	}
	
	void LUpdate_VLookAngle(){
		foreach(Transform segment in upperBodyChain){
			segment.RotateAround(segment.position, transform.right, (V_LookAngle * aimingWeightControl) / upperBodyChain.Length);
		}
	}
	
	void TryAttack(){
		GTA_Weapons_Network.Weapon_Attack_Type TEMP_attack_type = weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.ATTACK_TYPE;
		switch(TEMP_attack_type){
			case(GTA_Weapons_Network.Weapon_Attack_Type.MELEE):
				break;
			case(GTA_Weapons_Network.Weapon_Attack_Type.RAYCAST):
				if (aimingWeightControl > 0.95f && !weapons.CLIP_EMPTY && !weapons.IS_ATTACKING){
					GetComponent<NetworkView>().RPC("NetworkSetAnimatorBool", RPCMode.All, "attack", true);
					CastRay();
					if (weapons.currentSlot == WeaponSystemSetupNetwork.Weapon_Slots.SHOTGUN) CastRay();
					weapons.slots[(int)weapons.currentSlot].AMMO_IN_CLIP--;
					weapons.slots[(int)weapons.currentSlot].NEXT_ATTACK_EVENT = Time.fixedTime + weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.ATTACK_RATE;
					StartCoroutine("Delayed_AttackFinish", weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.ATTACK_RATE - 0.05f);
				}
				break;
			case(GTA_Weapons_Network.Weapon_Attack_Type.PROJECTILE):
				if (aimingWeightControl > 0.95f && !weapons.CLIP_EMPTY && !weapons.IS_ATTACKING){
					GetComponent<NetworkView>().RPC("NetworkSetAnimatorBool", RPCMode.All, "attack", true);
					weapons.slots[(int)weapons.currentSlot].WEAPON_OBJECT.EjectTestNetwork(this.GetComponent<GTA_Controller_Network>());
					weapons.slots[(int)weapons.currentSlot].AMMO_IN_CLIP--;
					weapons.slots[(int)weapons.currentSlot].NEXT_ATTACK_EVENT = Time.fixedTime + weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.ATTACK_RATE;
					StartCoroutine("Delayed_AttackFinish", weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.ATTACK_RATE - 0.05f);
				}
				break;
		}
	}
	
	void TryReload(){
		GTA_Weapons_Network.Weapon_Attack_Type TEMP_attack_type = weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.ATTACK_TYPE;
		if(TEMP_attack_type != GTA_Weapons_Network.Weapon_Attack_Type.MELEE){
			if(!weapons.IS_SELECTION && weapons.HAS_AMMO && !weapons.IS_RELOADING){
				GetComponent<NetworkView>().RPC("NetworkEvent_ReloadStart", RPCMode.All, "OnReloadStart");
				GetComponent<NetworkView>().RPC("NetworkSetAnimatorBool", RPCMode.All, "reload", true);
				weapons.slots[(int)weapons.currentSlot].NEXT_RELOAD_EVENT = Time.fixedTime + weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.RELOAD_RATE;
				StartCoroutine("Delayed_ReloadFinish", weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.RELOAD_RATE - 0.1f);
			}
		}
	}

	void CalculateDistance(int range){
		float m;
		float n;
		
		if(weapons.currentSlot == WeaponSystemSetupNetwork.Weapon_Slots.HEAVY){
			m = -0.05f;
			n = -0.05f;
		}
		else{
			m = 0;
			n = 0;
		}

		RaycastHit hit;
		Ray ray = Camera.main.ViewportPointToRay (new Vector3( 0.5f + m, 0.5f + n, 0.0f));
		if(Physics.Raycast(ray, out hit, range)){
			weapons.aimTag = hit.collider.tag;
			if((weapons.aimTag == "Enemy" || weapons.aimTag == "Player") && hit.collider.transform.root.GetComponent<HealthSystem_Network>().dead) targetIsDeath = true;
			else targetIsDeath = false;
			if(weapons.aimTag == "Ragdoll" && hit.collider.transform.root.GetComponent<HealthSystem_Network>().fS) targetIsDeathNow = true;
			else targetIsDeathNow = false;
		}
		else weapons.aimTag = "No Object Hitted";
	}
	
	void CastRay(){
		float TEMP_inaccuracy = weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.INACCURACY;
		Vector3 dir;
		RaycastHit hit;
		Ray ray = Camera.main.ViewportPointToRay (new Vector3(0.5f + Utils.RandomValue(TEMP_inaccuracy), 0.5f + Utils.RandomValue(TEMP_inaccuracy), 0.0f));
		if(Physics.Raycast(ray, out hit, 1000.0f, weapons.hitLayers)){
			LayerMask layerHit;
			layerHit = hit.collider.gameObject.layer;
			dir = 50 * (-(hit.point - transform.position)).normalized;
			string tagHitObject= hit.collider.gameObject.tag;
			
			GetComponent<NetworkView>().RPC("Sync_Hit_Effect", RPCMode.All, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag);
			GetComponent<NetworkView>().RPC("NetworkEvent_AttackStart", RPCMode.All, hit.point, hit.distance);
			if(hit.rigidbody) Sync_Force_Impact(layerHit, hit, dir);
			
			foreach(WeaponSystemSetupNetwork.HIT_DAMAGE_BODY fireDamage in weapons.slots[(int) weapons.currentSlot].DamageBodyParts){
				if (fireDamage.Body_Part_Tag == tagHitObject)
					hit.collider.gameObject.transform.root.GetComponent<HealthSystem_Network>().Recived(fireDamage.Damage);
			}

			foreach(WeaponSystemSetupNetwork.Decal_Prefab mark in weapons.decals){
				if(mark.hitTag == tagHitObject){
					DecalGen (hit.point, hit.normal, hit.collider, mark.decal[Random.Range( 0, mark.decal.Length)], hit.collider.gameObject);
				}
			}
		}
		
		else{
			Vector3 imaginary_hit_point = ray.GetPoint(50.0f);
			NetworkEvent_AttackStart(imaginary_hit_point, 50.0f);
		}
	}

	[RPC]
	GameObject DecalGen(Vector3 p, Vector3 n, Collider c, GameObject o, GameObject e){
		GameObject decalInst;
		
		decalInst = (GameObject)Instantiate( o, p, Quaternion.FromToRotation(Vector3.up, n));
		decalInst.transform.parent = e.transform;
		
		MeshFilter mf = decalInst.GetComponent(typeof(MeshFilter)) as MeshFilter;
		Mesh m = mf.mesh;
		
		Vector3[] verts = m.vertices;
		
		for (int i = 0; i < verts.Length; i++){
			verts[i] = decalInst.transform.TransformPoint(verts[i]);
			
			if (verts[i].x > c.bounds.max.x)
				verts[i].x = c.bounds.max.x;
			
			if (verts[i].x < c.bounds.min.x)
				verts[i].x = c.bounds.min.x;
			
			if (verts[i].y > c.bounds.max.y)
				verts[i].y = c.bounds.max.y;
			
			if (verts[i].y < c.bounds.min.y)
				verts[i].y = c.bounds.min.y;
			
			if (verts[i].z > c.bounds.max.z)
				verts[i].z = c.bounds.max.z;
			
			if (verts[i].z < c.bounds.min.z)
				verts[i].z = c.bounds.min.z;
			verts[i] = decalInst.transform.InverseTransformPoint(verts[i]);
			m.vertices = verts;
		}
		Destroy(decalInst, 3.0f);
		return decalInst;
	} 
	
	WeaponSystemSetupNetwork.Weapon_Slots TryToSelectSlot(WeaponSystemSetupNetwork.Weapon_Slots slot){
		Stop_Attack();
		Stop_Reload();
		
		WeaponSystemSetupNetwork.Weapon_Slots TEMP_weapon_slot = weapons.slots[(int)slot].USED ? slot : WeaponSystemSetupNetwork.Weapon_Slots.UNARMED;
		weapons.slots[(int)TEMP_weapon_slot].NEXT_RELOAD_EVENT = Time.fixedTime;
		return TEMP_weapon_slot;
	}
	
	[RPC]
	void NetworkEvent_ReloadStart(string method_name){
		if(weapons.slots[(int)weapons.currentSlot].HAS_WEAPON_OBJECT)
			weapons.slots[(int)weapons.currentSlot].WEAPON_OBJECT.Invoke(method_name, 0.0f);
	}
	
	[RPC]
	void NetworkEvent_AttackStart(Vector3 hit_point, float hit_distance){
		if(weapons.slots[(int)weapons.currentSlot].HAS_WEAPON_OBJECT)
			weapons.slots[(int)weapons.currentSlot].WEAPON_OBJECT.OnAttackStart(hit_point, hit_distance);
	}
	
	[RPC]
	void NetworkEvent_ReloadEnd(string method_name){
		if(weapons.slots[(int)weapons.currentSlot].HAS_WEAPON_OBJECT)
			weapons.slots[(int)weapons.currentSlot].WEAPON_OBJECT.Invoke(method_name, 0.0f);
	}
	
	[RPC]
	void NetworkSetAnimatorBool(string propertyName, bool val){
		a.SetBool(propertyName, val);
	}
	
	[RPC]
	void Sync_Hit_Effect(Vector3 pos, Quaternion rot, string hit_tag){
		foreach(WeaponSystemSetupNetwork.Hit_Event hit_event in weapons.hitEvents){
			if(hit_event.hitTag == hit_tag){
				hit_event.root.position = pos;
				hit_event.root.rotation = rot;
				switch(hit_event.selectEmmitter){
				case WeaponSystemSetupNetwork.Particle_Type.Legacy :
					foreach(ParticleEmitter emitter in hit_event.emmitt_Legacy)
						emitter.Emit();
				break;
				case WeaponSystemSetupNetwork.Particle_Type.Shuriken :
					foreach(ParticleSystem emitter in hit_event.emitt_Shuriken)
						emitter.Emit(5);
				break;
				case WeaponSystemSetupNetwork.Particle_Type.Both :
					foreach(ParticleEmitter emitter in hit_event.emmitt_Legacy)
						emitter.Emit();
					foreach(ParticleSystem emitter in hit_event.emitt_Shuriken)
						emitter.Emit(5);
				break;
				}
			}
		}
	}
	
	void Sync_Force_Impact(LayerMask layerHit, RaycastHit hit, Vector3 dir){
		foreach(WeaponSystemSetupNetwork.EffectsPhysics lHit in weapons.LayersEffectsPhysics){
			if(layerHit.value == LayerMask.NameToLayer(lHit.layerName))
				hit.rigidbody.AddForceAtPosition( -weapons.slots[(int) weapons.currentSlot].POWER_IMPACT * dir, hit.point);
		}
	}
	
	[RPC]
	void AddOrReplaceWeapon(int slot, int ID){
		weapons.slots[slot].ID = ID;
		weapons.slots[slot].USED = true;
		weapons.slots[slot].AMMO_IN_CLIP = (int)weapons.slots[slot].AMMO_IN_CLIP;
		weapons.slots[slot].HAS_AMMO = (int)weapons.slots[slot].HAS_AMMO;
	}
	
	[RPC]
	void CreateWeaponObject(int slot_current, int slot_last){
		if(weapons.slots[slot_last].HAS_WEAPON_OBJECT)
			Destroy(weapons.slots[slot_last].WEAPON_OBJECT.gameObject);
		if(weapons.slots[slot_current].WEAPON_INFO.OBJECT){
			Vector3 TEMP_pos = weapons.weaponBone.position;
			Quaternion TEMP_rot = weapons.weaponBone.rotation;
			GameObject TEMP_weapon_object = Instantiate(weapons.slots[slot_current].WEAPON_INFO.OBJECT, TEMP_pos, TEMP_rot) as GameObject;
			TEMP_weapon_object.transform.parent = weapons.weaponBone;
			TEMP_weapon_object.transform.localPosition = Vector3.zero;
			TEMP_weapon_object.transform.localRotation = Quaternion.identity;
			weapons.slots[slot_current].WEAPON_OBJECT = TEMP_weapon_object.GetComponent<GTA_WeaponManager>();
			weapons.slots[slot_current].WEAPON_OBJECT.owner_Network = gameObject.GetComponent<GTA_Controller_Network>();
		}
	}
	
	void Stop_Attack(){
		StopCoroutine("Delayed_AttackFinish");
		GetComponent<NetworkView>().RPC("NetworkSetAnimatorBool", RPCMode.All, "attack", false);
	}
	
	void Stop_Reload(){
		StopCoroutine("Delayed_ReloadFinish");
		GetComponent<NetworkView>().RPC("NetworkSetAnimatorBool", RPCMode.All, "reload", false);
	}
	
	IEnumerator Delayed_AttackFinish(float time) {
		yield return new WaitForSeconds(time);
		GetComponent<NetworkView>().RPC("NetworkSetAnimatorBool", RPCMode.All, "attack", false);
	}
	
	IEnumerator Delayed_ReloadFinish(float time) {
		yield return new WaitForSeconds(time);
		
		int TEMP_need_ammo = (int)weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.CLIP_SIZE - weapons.slots[(int)weapons.currentSlot].AMMO_IN_CLIP;
		if(weapons.slots[(int)weapons.currentSlot].HAS_AMMO >= TEMP_need_ammo){
			weapons.slots[(int)weapons.currentSlot].HAS_AMMO -= TEMP_need_ammo;
			weapons.slots[(int)weapons.currentSlot].AMMO_IN_CLIP = (int)weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.CLIP_SIZE;
		}
		else{
			weapons.slots[(int)weapons.currentSlot].AMMO_IN_CLIP = weapons.slots[(int)weapons.currentSlot].HAS_AMMO;
			weapons.slots[(int)weapons.currentSlot].HAS_AMMO = 0;
		}
		GetComponent<NetworkView>().RPC("NetworkEvent_ReloadEnd", RPCMode.All, "OnReloadFinish");
		yield return new WaitForSeconds(0.1f);
		GetComponent<NetworkView>().RPC("NetworkSetAnimatorBool", RPCMode.All, "reload", false);
    }
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 TEMP_pos = Vector3.zero;
		Quaternion TEMP_rot = Quaternion.identity;
		float TEMP_V_LookAngle = 0.0f;
		//float TEMP_aimingWeightControl = 0.0f;
		float TEMP_h = 0.0f;
	 	float TEMP_v = 0.0f; 
		int TEMP_current_weapon_slot = 0;
		int TEMP_last_weapon_slot = 0;
		
		bool TEMP_SYNC_is_aiming = false;
		//bool TEMP_SYNC_is_attacking = false;
		//bool TEMP_SYNC_is_reloading = false;
		float TEMP_SYNC_animator_move_speed = 0.0f;
		
		
		if (stream.isWriting) {
            TEMP_pos = transform.position;
			TEMP_rot = transform.rotation;
			TEMP_V_LookAngle = V_LookAngle;
			//TEMP_aimingWeightControl = aimingWeightControl;
			TEMP_h = h;
	 		TEMP_v = v; 
			TEMP_current_weapon_slot = (int)weapons.currentSlot;
			TEMP_last_weapon_slot = (int)weapons.lastSlot;
			
			TEMP_SYNC_is_aiming = IS_AIMING;
			//TEMP_SYNC_is_attacking = weapons.IS_ATTACKING && !weapons.IS_SELECTION;
			//TEMP_SYNC_is_reloading = weapons.IS_RELOADING && !weapons.IS_SELECTION;
			TEMP_SYNC_animator_move_speed = ANIMATOR_MOVE_SPEED;
			
			stream.Serialize(ref TEMP_pos);
			stream.Serialize(ref TEMP_rot);
			stream.Serialize(ref TEMP_V_LookAngle);
			//stream.Serialize(ref TEMP_aimingWeightControl);
			stream.Serialize(ref TEMP_h);
			stream.Serialize(ref TEMP_v);
			stream.Serialize(ref TEMP_current_weapon_slot);
			stream.Serialize(ref TEMP_last_weapon_slot);
			
			stream.Serialize(ref TEMP_SYNC_is_aiming);
			//stream.Serialize(ref TEMP_SYNC_is_attacking);
			//stream.Serialize(ref TEMP_SYNC_is_reloading);
			stream.Serialize(ref TEMP_SYNC_animator_move_speed);
        } else {
           	stream.Serialize(ref TEMP_pos);
			stream.Serialize(ref TEMP_rot);
			stream.Serialize(ref TEMP_V_LookAngle);
			//stream.Serialize(ref TEMP_aimingWeightControl);
			stream.Serialize(ref TEMP_h);
			stream.Serialize(ref TEMP_v);
			stream.Serialize(ref TEMP_current_weapon_slot);
			stream.Serialize(ref TEMP_last_weapon_slot);
			
			stream.Serialize(ref TEMP_SYNC_is_aiming);
			//stream.Serialize(ref TEMP_SYNC_is_attacking);
			//stream.Serialize(ref TEMP_SYNC_is_reloading);
			stream.Serialize(ref TEMP_SYNC_animator_move_speed);
			
			transform.position = TEMP_pos;
			transform.rotation = TEMP_rot;
			V_LookAngle = TEMP_V_LookAngle;
			//aimingWeightControl = TEMP_aimingWeightControl;
			h = TEMP_h;
	 		v = TEMP_v; 
			weapons.currentSlot = (WeaponSystemSetupNetwork.Weapon_Slots)TEMP_current_weapon_slot;
			weapons.currentSlot = (WeaponSystemSetupNetwork.Weapon_Slots)TEMP_last_weapon_slot;
       		
			SYNC_is_aiming = TEMP_SYNC_is_aiming;
			//SYNC_is_attacking = TEMP_SYNC_is_attacking;
			//SYNC_is_reloading = TEMP_SYNC_is_reloading;
			SYNC_animator_move_speed = TEMP_SYNC_animator_move_speed;
		}
    }
	
	void OnGUI(){
		if(GetComponent<NetworkView>().isMine){
			if(SHOW_DEBUG_INFO){
				GUI.Label(new Rect(10, 10, 200, 20), "is selection: " + weapons.IS_SELECTION.ToString());
				GUI.Label(new Rect(10, 30, 200, 20), "is reloading: " + weapons.IS_RELOADING.ToString());
				GUI.Label(new Rect(10, 50, 200, 20), "is attacking: " + weapons.IS_ATTACKING.ToString());
				GUI.Label(new Rect(10, 70, 200, 20), "is crouching: " + IS_CROUCHING.ToString());
				GUI.Label(new Rect(10, 90, 200, 20), "is aiming: " + IS_AIMING.ToString());
				GUI.Label(new Rect(10, 110, 200, 20), "is moving: " + IS_MOVING.ToString());
				GUI.Label(new Rect(10, 130, 200, 20), "can aim: " + weapons.CAN_AIM.ToString());
				GUI.Label(new Rect(10, 150, 200, 20), "look angle: " + V_LookAngle.ToString("f2"));
				GUI.Label(new Rect(10, 170, 200, 20), "mouseX: " + mouseX.ToString());
				GUI.Label(new Rect(10, 190, 200, 20), "mouseY: " + mouseX.ToString());
			}
			
			if(weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.ATTACK_TYPE != GTA_Weapons_Network.Weapon_Attack_Type.MELEE){
				string TEMP_text_ammo_in_clip = weapons.slots[(int)weapons.currentSlot].AMMO_IN_CLIP.ToString();
				string TEMP_text_has_ammo = weapons.slots[(int)weapons.currentSlot].HAS_AMMO.ToString();
				
				
				Utils.DrawOutlineText(new Rect(Screen.width - Screen.width/5.6f + 80, Screen.height/12, 40, 30), TEMP_text_ammo_in_clip, UIStyle, Color.black, Color.grey, 4.0f);
				Utils.DrawOutlineText(new Rect(Screen.width - Screen.width/5.6f, Screen.height/12, 85, 30), TEMP_text_has_ammo, UIStyle, Color.black, Color.white, 4.0f);
			}
			GUI.DrawTexture(new Rect(Screen.width - Screen.width/5.6f, Screen.height/12, 130, 130), weapons.slots[(int)weapons.currentSlot].WEAPON_INFO.ICON, ScaleMode.ScaleToFit, true, 0f);
		}
	}

	void OnTriggerEnter(Collider col){
		col = footSteps.RToe;
		col = footSteps.LToe;
		FootSteps();
	}
	
	void OnCollisionStay () {
	    isGrounded = true;    
	}
}