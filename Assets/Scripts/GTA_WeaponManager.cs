using UnityEngine;
using System.Collections;

public class GTA_WeaponManager : MonoBehaviour {
	public Transform IK_Goal_LeftHand;
	public ParticleEmitter[] emittOnAttack;
	public AudioClip clip;
	public AudioSource source;
	public Light lightOnShot;
	public Transform trailStart;
	public GameObject lineRenderer;
	
	Renderer Rrojectile_Render;
	Transform Rrojectile_EjectPoint;

	[HideInInspector] public GTA_Controller owner;
	[HideInInspector] public GTA_Controller_Network owner_Network;

	void Awake(){
		foreach(Transform child in transform){
			if(child.name == "Rrojectile_Render")
				Rrojectile_Render = child.gameObject.GetComponent<MeshRenderer>();
			if(child.name == "Rrojectile_EjectPoint")
				Rrojectile_EjectPoint = child;	
		}
	}
	
	public void AttackEffectEvent(){
		StopCoroutine("LightOnShot");
		source.Stop();
		source.PlayOneShot(clip);
		StartCoroutine("LightOnShot", 0.037f);
		foreach(ParticleEmitter pe in emittOnAttack)
			pe.Emit();
	}
	
	public void RaycastTrail(Vector3 start_point, Vector3 end_point, float hit_distance){
		GameObject trailGO = Instantiate(lineRenderer.gameObject, transform.position, Quaternion.identity) as GameObject;
		trailGO.GetComponent<TrailInstance>().Calculate(hit_distance, start_point, end_point);
	}
	
	public void OnAttackStart(Vector3 hit_point, float hit_distance){
		RaycastTrail(trailStart.position, hit_point, hit_distance);
		AttackEffectEvent();
		if(Rrojectile_Render)
			Rrojectile_Render.enabled = false;
	}
	
	public void OnReloadStart(){
		if(Rrojectile_Render)
			Rrojectile_Render.enabled = false;
	}
	
	public void OnReloadFinish(){
		if(Rrojectile_Render)
			Rrojectile_Render.enabled = true;
	}
	
	public void EjectTest(GTA_Controller RR){
		GameObject RPG_Rocket;
		RPG_Rocket = (GameObject) Instantiate(GTA_Weapons.Load_WeaponGO("RPG_Rocket"), Rrojectile_EjectPoint.position, Rrojectile_EjectPoint.rotation);
		RPG_Rocket.GetComponent<Rrojectile>().rr = RR; 
		source.PlayOneShot(clip);
	}

	public void EjectTestNetwork(GTA_Controller_Network RR){
		GameObject RPG_Rocket;
		RPG_Rocket = (GameObject) Network.Instantiate(GTA_Weapons.Load_WeaponGO("RPG_Rocket_Network"), Rrojectile_EjectPoint.position, Rrojectile_EjectPoint.rotation, 0);
		RPG_Rocket.GetComponent<Rrojectile_Network>().pr = RR; 
		source.PlayOneShot(clip);
	}

	[ContextMenu ("Start Emitters")]
    void StartEmitters() {
        foreach(ParticleEmitter pe in emittOnAttack)
			pe.emit = true;
    }
	
	[ContextMenu ("Stop Emitters")]
    void StopEmitters() {
        foreach(ParticleEmitter pe in emittOnAttack)
			pe.emit = false;
    }
	
	IEnumerator LightOnShot(float time){
		lightOnShot.enabled = true;
		yield return new WaitForSeconds(time);
		lightOnShot.enabled = false;
	}
}
