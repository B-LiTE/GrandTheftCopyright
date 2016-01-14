using UnityEngine;
using System.Collections;

public class CheckDistance : MonoBehaviour {
	public Transform target;
	public float distance;
	
	void Update () {
		distance = Vector3.Distance(transform.position, target.position);
	}
}
