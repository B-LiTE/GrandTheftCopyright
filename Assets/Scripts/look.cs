using UnityEngine;
using System.Collections;

[System.Serializable]
public class BodySegment_test{
	public Transform bone_target;
	public Transform bone_original;
	public float weight = 0.1f;
}
 
public class look : MonoBehaviour {
	public BodySegment_test[] bs;
	
	public Transform target;
	
	public Animator a;
	public bool use;
	
	void Start () {
		a.SetBool("aiming", true);
		
	}
	
	void LateUpdate () {
		if(!use)
			return;
		foreach(BodySegment_test s in bs){
			Quaternion TEMP_lookRotation = Quaternion.LookRotation (target.position - s.bone_original.position);
		
			s.bone_target.rotation = Quaternion.Slerp(s.bone_target.rotation, TEMP_lookRotation * s.bone_original.rotation, s.weight);
			//s.bone_target.rotation = target.rotation * s.bone_original.rotation;
		
		}
	}
}
