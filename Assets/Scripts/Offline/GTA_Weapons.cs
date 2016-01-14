using UnityEngine;
using System.Collections;

public class GTA_Weapons{
	public struct Weapon_Info{
		public string NAME;
		public Weapon_Attack_Type ATTACK_TYPE;
		public float ATTACK_RATE;
		public float RELOAD_RATE;
		public float INACCURACY;
		public int? CLIP_SIZE;
		public int? MAX_AMMO;
		public GameObject OBJECT;
		public Texture2D ICON;
		public Texture2D crosshair;
		public Texture2D crosshairEnemy;
		public Texture2D crosshairDeath;
		public Texture2D crosshairRagdoll;
		public Texture2D crosshairGTAV;
		public Texture2D crosshairGTAVEnemy;

		public Weapon_Info(
			string NAME, 
			Weapon_Attack_Type ATTACK_TYPE,
			float ATTACK_RATE, 
			float RELOAD_RATE, 
			float INACCURACY,
			int? CLIP_SIZE, 
			int? MAX_AMMO, 
			GameObject OBJECT,
			Texture2D ICON,
			Texture2D crosshair,
			Texture2D crosshairEnemy,
			Texture2D crosshairDeath,
			Texture2D crosshairRagdoll,
			Texture2D crosshairGTAV,
			Texture2D crosshairGTAVEnemy){

			this.NAME = NAME;
			this.ATTACK_TYPE = ATTACK_TYPE;
			this.ATTACK_RATE = ATTACK_RATE;
			this.INACCURACY = INACCURACY;
			this.RELOAD_RATE = RELOAD_RATE;
			this.CLIP_SIZE = CLIP_SIZE;
			this.MAX_AMMO = MAX_AMMO;
			this.OBJECT = OBJECT;
			this.ICON = ICON;
			this.crosshair = crosshair;
			this.crosshairEnemy = crosshairEnemy;
			this.crosshairDeath = crosshairDeath;
			this.crosshairRagdoll = crosshairRagdoll;
			this.crosshairGTAV = crosshairGTAV;
			this.crosshairGTAVEnemy = crosshairGTAVEnemy;
		}
	}
	
	public enum Weapon_Attack_Type{
		MELEE,
		RAYCAST,
		PROJECTILE,
	}
	
	public struct Weapon_Flags{
		public bool CAN_AIM;
		public bool USE_IK_LH_AIM_MOVE;
		public bool USE_IK_LH_NORMAL_MOVE;
		
	
		public Weapon_Flags(
			bool CAN_AIM, 
			bool USE_IK_LH_AIM_MOVE, 
			bool USE_IK_LH_NORMAL_MOVE){
			
			this.CAN_AIM = CAN_AIM;
			this.USE_IK_LH_AIM_MOVE = USE_IK_LH_AIM_MOVE;
			this.USE_IK_LH_NORMAL_MOVE = USE_IK_LH_NORMAL_MOVE;
		}
	}
	
	public struct Weapon_Motion_Data{
		public int MOTION_TYPE_MOVEMENT_BASE;
		public int MOTION_TYPE_MOVEMENT_DIRECTIONAL;
		public int MOTION_TYPE_ATTACK;
		public int MOTION_TYPE_RELOAD;
		
	
		public Weapon_Motion_Data(
			int MOTION_TYPE_MOVEMENT_BASE, 
			int MOTION_TYPE_MOVEMENT_DIRECTIONAL, 
			int MOTION_TYPE_ATTACK, 
			int MOTION_TYPE_RELOAD){
			
			this.MOTION_TYPE_MOVEMENT_BASE = MOTION_TYPE_MOVEMENT_BASE;
			this.MOTION_TYPE_MOVEMENT_DIRECTIONAL = MOTION_TYPE_MOVEMENT_DIRECTIONAL;
			this.MOTION_TYPE_ATTACK = MOTION_TYPE_ATTACK;
			this.MOTION_TYPE_RELOAD = MOTION_TYPE_RELOAD;
		}
	}
	
	public static Weapon_Flags GET_WEAPON_SLOT_FLAGS(WeaponSystemSetup.Weapon_Slots slot, int id){
		Weapon_Flags TEMP_weapon_flags = new Weapon_Flags();
		switch(slot){
			case(WeaponSystemSetup.Weapon_Slots.UNARMED):
				TEMP_weapon_flags = new Weapon_Flags(false, false, false);
			break;
		
			case(WeaponSystemSetup.Weapon_Slots.HANDGUN):
				switch(id){
					case(0):
						TEMP_weapon_flags = new Weapon_Flags(true, true, false);
					break;
					default:
        				Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;

			case(WeaponSystemSetup.Weapon_Slots.SUBMACHINEGUN):
				switch(id){
					case(0):
						TEMP_weapon_flags = new Weapon_Flags(true, true, false);
					break;
				default:
					Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
				break;
			}
			break;
			
			case(WeaponSystemSetup.Weapon_Slots.SHOTGUN):
				switch(id){
					case(0):
						TEMP_weapon_flags = new Weapon_Flags(true, true, true);
					break;
					case(1):
						TEMP_weapon_flags = new Weapon_Flags(true, true, true);
					break;
					default:
        				Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;
			
			case(WeaponSystemSetup.Weapon_Slots.RIFLE):
				switch(id){
					case(0):
						TEMP_weapon_flags = new Weapon_Flags(true, true, true);
					break;
					case(1):
						TEMP_weapon_flags = new Weapon_Flags(true, true, true);
					break;
					default:
        				Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;
			
			case(WeaponSystemSetup.Weapon_Slots.HEAVY):
				switch(id){
					case(0):
						TEMP_weapon_flags = new Weapon_Flags(true, true, true);
					break;
					case(1):
						TEMP_weapon_flags = new Weapon_Flags(true, true, true);
					break;
					default:
					Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
				break;

				}
			break;
			
			default:
        		Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong slot type.", "red");
        	break;
		}
		return TEMP_weapon_flags;
	}
	
	public static Weapon_Motion_Data GET_WEAPON_MOTION_DATA(WeaponSystemSetup.Weapon_Slots slot, int id){
		Weapon_Motion_Data TEMP_weapon_motion_data = new Weapon_Motion_Data();
		switch(slot){
			case(WeaponSystemSetup.Weapon_Slots.UNARMED):
				TEMP_weapon_motion_data = new Weapon_Motion_Data(0, 0, 0, 0);
			break;
		
			case(WeaponSystemSetup.Weapon_Slots.HANDGUN):
				switch(id){
					case(0):
						TEMP_weapon_motion_data = new Weapon_Motion_Data(1, 1, 1, 1);
					break;
					default:
        				Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;

			case(WeaponSystemSetup.Weapon_Slots.SUBMACHINEGUN):
				switch(id){
					case(0):
						TEMP_weapon_motion_data = new Weapon_Motion_Data(1, 1, 1, 1);
					break;
					default:
						Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;
			
			case(WeaponSystemSetup.Weapon_Slots.SHOTGUN):
				switch(id){
					case(0):
						TEMP_weapon_motion_data = new Weapon_Motion_Data(2, 2, 4, 4);
					break;
					case(1):
						TEMP_weapon_motion_data = new Weapon_Motion_Data(2, 2, 4, 4);
					break;
					default:
        				Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;
			
			case(WeaponSystemSetup.Weapon_Slots.RIFLE):
				switch(id){
					case(0):
						TEMP_weapon_motion_data = new Weapon_Motion_Data(2, 2, 2, 2);
					break;
					case(1):
						TEMP_weapon_motion_data = new Weapon_Motion_Data(2, 2, 2, 2);
					break;
					default:
        				Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;
			
			case(WeaponSystemSetup.Weapon_Slots.HEAVY):
				switch(id){
					case(0):
						TEMP_weapon_motion_data = new Weapon_Motion_Data(2, 3, 3, 3);
					break;
					case(1):
						TEMP_weapon_motion_data = new Weapon_Motion_Data(2, 3, 3, 3);
					break;
					default:
						Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
				break;
				}
			break;
			
			default:
        		Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong slot type.", "red");
        	break;
		}
		return TEMP_weapon_motion_data;
	}
	
	public static Weapon_Info GET_WEAPON_INFO(WeaponSystemSetup.Weapon_Slots slot, int id){
		Weapon_Info TEMP_weapon_info = new Weapon_Info();
		switch(slot){
			case(WeaponSystemSetup.Weapon_Slots.UNARMED):
			TEMP_weapon_info = new Weapon_Info("Unarmed",Weapon_Attack_Type.MELEE, 0, 0, 0, 0, 0, null, Load_Icon("Unarmed"), Load_Reticule("normal"), Load_Reticule("normal"), Load_Reticule("normal"), Load_Reticule("normal"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"));
			break;
		
			case(WeaponSystemSetup.Weapon_Slots.HANDGUN):
				switch(id){
					case(0):
						TEMP_weapon_info = new Weapon_Info("Glock 18",Weapon_Attack_Type.RAYCAST, 0.29f, 1.7f, 0.016f, 12, 480, Load_WeaponGO("Glock18"), Load_Icon("Glock 18"), Load_Reticule("Handgun Aim"), Load_Reticule("Handgun Aim Enemy"), Load_Reticule("Enemy Death"), Load_Reticule("Aim Ragdoll"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"));
					break;
					default:
        				Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;

			case(WeaponSystemSetup.Weapon_Slots.SUBMACHINEGUN):
				switch(id){
					case(0):
						TEMP_weapon_info = new Weapon_Info("Uzi",Weapon_Attack_Type.RAYCAST, 0.11f, 1.6f, 0.025f, 30, 1200, Load_WeaponGO("Uzi"), Load_Icon("Micro-Uzi"), Load_Reticule("Submachinegun Aim"), Load_Reticule("Submachinegun Aim Enemy"), Load_Reticule("Enemy Death"), Load_Reticule("Aim Ragdoll"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"));
					break;
					default:
						Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;
			
			case(WeaponSystemSetup.Weapon_Slots.SHOTGUN):
				switch(id){
					case(0):
						TEMP_weapon_info = new Weapon_Info("Remington",Weapon_Attack_Type.RAYCAST, 0.26f, 3.0f, 0.03f, 8, 640, Load_WeaponGO("PumpSG"), Load_Icon("PumpShotgun"), Load_Reticule("Shotgun Aim"), Load_Reticule("Shotgun Aim Enemy"), Load_Reticule("Enemy Death"), Load_Reticule("Aim Ragdoll"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"));
					break;
					case(1):
						TEMP_weapon_info = new Weapon_Info("Dao-12",Weapon_Attack_Type.RAYCAST, 0.3f, 3.0f, 0.027f, 6, 600, Load_WeaponGO("Dao12"), Load_Icon("Dao-12"), Load_Reticule("Shotgun Aim"), Load_Reticule("Shotgun Aim Enemy"), Load_Reticule("Enemy Death"), Load_Reticule("Aim Ragdoll"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"));
					break;
					default:
        				Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;
			
			case(WeaponSystemSetup.Weapon_Slots.RIFLE):
				switch(id){
					case(0):
						TEMP_weapon_info = new Weapon_Info("M4A1",Weapon_Attack_Type.RAYCAST, 0.125f, 2.5f, 0.012f, 30, 1200, Load_WeaponGO("M4A1"), Load_Icon("M4A1"), Load_Reticule("Rifle Aim"), Load_Reticule("Rifle Aim Enemy"), Load_Reticule("Enemy Death"), Load_Reticule("Aim Ragdoll"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"));
					break;
					case(1):
						TEMP_weapon_info = new Weapon_Info("AK-47",Weapon_Attack_Type.RAYCAST, 0.13f, 2.5f, 0.015f, 30, 1200, Load_WeaponGO("AK47"), Load_Icon("AK-47"), Load_Reticule("Rifle Aim"), Load_Reticule("Rifle Aim Enemy"), Load_Reticule("Enemy Death"), Load_Reticule("Aim Ragdoll"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"));
					break;
					default:
        				Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;
			
			case(WeaponSystemSetup.Weapon_Slots.HEAVY):
				switch(id){
					case(0):
						TEMP_weapon_info = new Weapon_Info("RPG-7",Weapon_Attack_Type.PROJECTILE, 0.5f, 3.5f, 0, 1, 100, Load_WeaponGO("RPG"), Load_Icon("RPG-7"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"), Load_Reticule("Enemy Death"), Load_Reticule("Aim Ragdoll"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"));
					break;
					case(1):
						TEMP_weapon_info = new Weapon_Info("Lawm-72",Weapon_Attack_Type.PROJECTILE, 0.5f, 3.5f, 0, 1, 100, Load_WeaponGO("Law"), Load_Icon("Lawm-72"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"), Load_Reticule("Enemy Death"), Load_Reticule("Aim Ragdoll"), Load_Reticule("Aim"), Load_Reticule("Aim Enemy"));
					break;
					default:
						Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong weapon id.", "red");
					break;
				}
			break;
			
			default:
        		Utils.CLog("[GET_WEAPON_INFO]", "ERROR! Wrong slot type.", "red");
        	break;
		}
		return TEMP_weapon_info;
	}
	
	public static Texture2D Load_Icon(string name){
		return (Texture2D)Resources.Load("GUITextures/WeaponIcons/" + name);
	}

	public static Texture2D Load_Reticule(string name){
		return (Texture2D)Resources.Load("GUITextures/Crosshair/" + name);
	}

	public static GameObject Load_WeaponGO(string name){
		return (GameObject)Resources.Load("Prefabs/Weapons/" + name);
	}
}
