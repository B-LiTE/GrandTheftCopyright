using UnityEngine;
using System.Collections;

public class TrailCreator : MonoBehaviour {
	public LineRenderer lineRenderer;
	public float raycastRate = 0.1f;
	
	float nextRaycastEvent;
	
	void Update(){
		if(Input.GetMouseButton(0) && Time.time > nextRaycastEvent){
			nextRaycastEvent = Time.time + raycastRate;
			Raycast();
		}
	}
	
	void Raycast(){
		RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000.0F)){
			GameObject trailGO = Instantiate(lineRenderer.gameObject, transform.position, Quaternion.identity) as GameObject;
			trailGO.GetComponent<TrailInstance>().Calculate(hit.distance, transform.position, hit.point);
		}
		//trailGO.GetComponent<TrailInstance>().Calculate(50.0f, transform.position, hit.point);
	}
}
