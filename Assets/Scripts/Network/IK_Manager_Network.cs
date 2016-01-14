using UnityEngine;
using System.Collections;

public class IK_Manager_Network : MonoBehaviour {
	public bool useIK;
	//public float IKWeight_LH_current;
	public float IKWeight_LH_target;
	
	public GTA_Controller_Network owner;
	public Animator a;
	
	public bool current_is_aiming;
	public bool current_is_reloading;
	
	//public Vector3 lookOffset;
	
	void OnAnimatorIK (){
		if(!useIK)
			return;
		current_is_aiming = a.GetBool("aiming");
		current_is_reloading = a.GetBool("reload");
		
		//IKWeight_LH_current = Calculate_IKWeight_LH_target();
		
		if(owner.weapons.slots[(int)owner.weapons.currentSlot].HAS_WEAPON_OBJECT){
			a.SetIKPosition(AvatarIKGoal.LeftHand,  GET_IKGoal_LH().position); 
			a.SetIKRotation(AvatarIKGoal.LeftHand,  GET_IKGoal_LH().rotation);
		}
		
		
		a.SetIKPositionWeight(AvatarIKGoal.LeftHand, Calculate_IKWeight_LH_target());
		a.SetIKRotationWeight(AvatarIKGoal.LeftHand, Calculate_IKWeight_LH_target());
		
		/*if(owner.networkView.isMine){
			a.SetLookAtPosition(owner.c.cPosition + owner.c.transform.TransformDirection(lookOffset)); 
			a.SetLookAtWeight((1 - owner.aimingWeightControl) * 0.3f);
		}	*/
	}
	
	Transform GET_IKGoal_LH(){
		return owner.weapons.slots[(int)owner.weapons.currentSlot].WEAPON_OBJECT.IK_Goal_LeftHand;
	}
	
	float Calculate_IKWeight_LH_target(){
		if(!owner.weapons.slots[(int)owner.weapons.currentSlot].HAS_WEAPON_OBJECT || current_is_reloading)
			return 0.0f;
		if(current_is_aiming && !owner.weapons.slots[(int)(owner.weapons.currentSlot)].FLAGS.USE_IK_LH_NORMAL_MOVE)
			return owner.weapons.slots[(int)(owner.weapons.currentSlot)].FLAGS.USE_IK_LH_AIM_MOVE ? (!a.IsInTransition(0) ? 1.0f : 0.0f) : 0.0f;
		
		return owner.weapons.slots[(int)(owner.weapons.currentSlot)].FLAGS.USE_IK_LH_NORMAL_MOVE ? 1.0f : 0.0f;
		
	}
}
