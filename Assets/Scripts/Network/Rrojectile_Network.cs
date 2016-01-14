using UnityEngine;
using System.Collections;

public class Rrojectile_Network : MonoBehaviour {
	public float lifeTime = 2.5f;
	public ParticleEmitter[] trailEmitters;
	public AudioSource audioSource;
	public AudioClip explosionSound;
	public GameObject explosionPrefab;
	public GTA_Controller_Network pr;
	Vector3 TEMP_smoothed_offset;


	void Awake () {
		Invoke("Destroy", lifeTime);
	}
	
	void FixedUpdate () {
		/*float rangeX = GetRandomValue(0.3f);
		float rangeY = GetRandomValue(0.3f);
		float rangeZ = GetRandomValue(0.3f);
		Vector3 TEMP_random_vector = new Vector3(rangeX, rangeY, rangeZ);*/
		TEMP_smoothed_offset = Vector3.MoveTowards(TEMP_smoothed_offset, Utils.RandomVector(0.3f), Time.fixedDeltaTime * 5.0f); 
		
		transform.Translate((Vector3.forward + TEMP_smoothed_offset) * Time.fixedDeltaTime * 15.0f);

		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2.0f)){
			LayerMask layerHit;
			layerHit = hit.collider.gameObject.layer;
			string tagHitObject = hit.collider.gameObject.tag;
			transform.position = hit.point;
			Destroy();

			foreach(WeaponSystemSetupNetwork.HIT_DAMAGE_BODY fireDamage in pr.weapons.slots[(int)pr.weapons.currentSlot].DamageBodyParts){
				if (fireDamage.Body_Part_Tag == tagHitObject)
					hit.collider.gameObject.transform.root.GetComponent<HealthSystem_Network>().Damage(fireDamage.Damage);
			}
	
			if(hit.rigidbody){
				foreach(WeaponSystemSetupNetwork.EffectsPhysics lHit in pr.weapons.LayersEffectsPhysics){
					if(layerHit.value == LayerMask.NameToLayer(lHit.layerName)){
						Vector3 explosionPos = hit.point + Vector3.up;
						Collider[] colliders = Physics.OverlapSphere(explosionPos, 5);
						foreach (Collider thing in colliders) {
							if (thing && thing.GetComponent<Rigidbody>()) 
								thing.GetComponent<Rigidbody>().AddExplosionForce(thing.GetComponent<Rigidbody>().mass * 500, explosionPos, 5, 3.0F);
						}
					}
				}
			}
		}
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.green);
	}
	
	/*float GetRandomValue(float threshold){
		return Random.Range(-threshold, threshold);
	}*/
	
	
	void Destroy(){
		foreach(ParticleEmitter emitter in trailEmitters){
			emitter.transform.parent = null;
			emitter.emit = false;
		}
		
		GameObject explosionGo = Instantiate(explosionPrefab, transform.position, Quaternion.identity) as GameObject;
		
		AudioLowPassFilter low_pass_filter_acces = explosionGo.GetComponent<AudioLowPassFilter>();
		AudioListener audioListener = FindObjectOfType(typeof(AudioListener)) as AudioListener;
		low_pass_filter_acces.cutoffFrequency = Mathf.Clamp(low_pass_filter_acces.cutoffFrequency, 100.0f,  10000.0f - Vector3.Distance(explosionGo.transform.position, audioListener.transform.position) * 200.0f);
		explosionGo.GetComponent<AudioSource>().Play();
		
		Destroy(explosionGo, 7.0f);
		Destroy(gameObject);
	}
}
