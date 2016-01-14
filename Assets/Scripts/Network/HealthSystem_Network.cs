using UnityEngine;
using System.Collections;

public class HealthSystem_Network : MonoBehaviour {

	public float curHealth;
	public float maxHealth;
	public AudioClip deathClip;
	public GameObject iName;
	public GameObject cams;
	public GameObject iBarHealth;
	public string myname;
	
	[HideInInspector] public float lostHealth;
	[HideInInspector] public float minHealth;
	[HideInInspector] public bool recovery;
	[HideInInspector] public float recoveySpeed;
	[HideInInspector] public float counter;
	[HideInInspector] public float waitTime;
	[HideInInspector] public float timer;
	[HideInInspector] public float resetAfterDeathTime;
	[HideInInspector] public bool dead;
	[HideInInspector] public float timerfS;
	[HideInInspector] public float timerS;
	[HideInInspector] public bool fS;
	
	void Awake () {
		lostHealth = 0;
		minHealth = 1;
		recovery = false;
		recoveySpeed = 15;
		waitTime = 7.0f;
		counter = waitTime;
		resetAfterDeathTime = 3.0f;
		cams = GameObject.Find("Main Camera");
		timerfS = 0.25f;
		timerS = timerfS;
		
		if(curHealth < 1)
			curHealth = 100;
		if(maxHealth < 1)
			maxHealth = 100;
	}
	
	public void uName (string me){
		GetComponent<NetworkView>().RPC("userName", RPCMode.AllBuffered, me);
	}
	
	[RPC]
	public void userName (string nameOfUser){
		myname = nameOfUser;
	}
	
	public void Update () {
		if(curHealth < maxHealth && curHealth >= minHealth && !recovery) counter -= Time.deltaTime;
		if(counter <= 0 && !dead)recovery = true;
		if(counter > 0) recovery = false;
		if(recovery) curHealth += recoveySpeed * Time.deltaTime;
		if(curHealth >= maxHealth){
			counter = waitTime;
			curHealth = maxHealth;
		}
		if(curHealth < minHealth) Die();
		if(dead)timer += Time.deltaTime;
		else timer = 0;
		if(timer >= resetAfterDeathTime) LevelReset();
		lostHealth = maxHealth - curHealth;
		if(timerfS > 0) fS = true;
		else fS = false;
		if(dead) timerfS -= Time.deltaTime;
		else timerfS = timerS;
		if(iName && iBarHealth && cams) Info_Enemy();
	}
	
	public void Recived (int fireDamage){
		GetComponent<NetworkView>().RPC("Damage", RPCMode.All, fireDamage);
		counter = waitTime;
	}
	
	[RPC]
	public void Damage(int damage){
		if(curHealth >= minHealth && !dead) curHealth -= damage;
	}
	
	void Die(){
		if(curHealth < minHealth && curHealth != -300){
			dead = true;
			curHealth = -300;
			GetComponent<GTA_Controller_Network>().enabled = false;
			GetComponent<ToRagdollTest>().SendMessage("Dead");
			if(deathClip) AudioSource.PlayClipAtPoint(deathClip, transform.position);
			this.GetComponent<CapsuleCollider>().enabled = false;
			this.GetComponent<Rigidbody>().isKinematic = true;
		}
	}
	
	void LevelReset (){
		dead = false;
		curHealth = maxHealth;
		GetComponent<ToRagdollTest>().SendMessage("Dead");
		GetComponent<GTA_Controller_Network>().enabled = true;
		this.GetComponent<CapsuleCollider>().enabled = true;
		this.GetComponent<Rigidbody>().isKinematic = false;
	}
	
	void Info_Enemy(){
		int heightLettter = iName.GetComponent<TextMesh>().fontSize;
		float enemyHealth_var_x = (curHealth * (heightLettter/30))/150  + curHealth /150;
		float enemyHealth_var_y = (heightLettter/60) + 0.7f;
		iName.transform.rotation = Quaternion.Euler(0, cams.transform.rotation.eulerAngles.y, 0);
		iBarHealth.transform.rotation = Quaternion.Euler(0, cams.transform.rotation.eulerAngles.y, 0);
		if(!dead) iBarHealth.transform.localScale = new Vector3 (Mathf.Lerp(iBarHealth.transform.localScale.x, enemyHealth_var_x, Time.deltaTime * 5), Mathf.Lerp(iBarHealth.transform.localScale.y,enemyHealth_var_y/10, Time.deltaTime * 5), iBarHealth.transform.localScale.z);
		else iBarHealth.transform.localScale = new Vector3 (Mathf.Lerp(iBarHealth.transform.localScale.x, 0, Time.deltaTime * 5), iBarHealth.transform.localScale.y, iBarHealth.transform.localScale.z);
		
		if(this.gameObject.name == "Remote_Player"){
			Vector3 distance = this.transform.position - GameObject.Find("Local_Player").transform.position;
			float limitOfVision = distance.magnitude;
			iName.GetComponent<TextMesh>().text = this.myname;
			iBarHealth.GetComponent<MeshRenderer>().enabled = true;
			if(limitOfVision <= 60 && !dead){
				iName.GetComponent<MeshRenderer>().enabled = true;
				if(limitOfVision >= 5) iName.GetComponent<TextMesh>().fontSize = (int)limitOfVision * 5;
				else iName.GetComponent<TextMesh>().fontSize = 25;
				float LimitVision = (limitOfVision * 0.01f) + 0.2f;

				switch (iName.GetComponent<OrientationName>().Sequence.currentSlot){
				case Setup.XYZ_Sequence.Width_Height_Depth_OR_Depth_Height_Width:
					iName.transform.localPosition = new Vector3 
						(iName.transform.localPosition.x, 
						 Mathf.Lerp(iName.transform.localPosition.y, LimitVision, Time.deltaTime * 5), 
						 iName.transform.localPosition.z);
					break;
				case Setup.XYZ_Sequence.Width_Depth_Height_OR_Depth_Width_Height:
					iName.transform.localPosition = new Vector3 
						(iName.transform.localPosition.x, 
						 iName.transform.localPosition.y,
						 Mathf.Lerp(iName.transform.localPosition.z, LimitVision, Time.deltaTime * 5));
					break;
				case Setup.XYZ_Sequence.Height_Width_Depth_OR_Height_Depth_Width:
					iName.transform.localPosition = new Vector3 
						(Mathf.Lerp(iName.transform.localPosition.x, LimitVision, Time.deltaTime * 5), 
						 iName.transform.localPosition.y, 
						 iName.transform.localPosition.z);
					break;
				case Setup.XYZ_Sequence.Width_negativeHeight_Depth_OR_Depth_negativeHeight_Width:
					iName.transform.localPosition = new Vector3 
						(iName.transform.localPosition.x, 
						 Mathf.Lerp(iName.transform.localPosition.y, -LimitVision, Time.deltaTime * 5), 
						 iName.transform.localPosition.z);
					break;
				case Setup.XYZ_Sequence.Width_Depth_negativeHeight_OR_Depth_Width_negativeHeight:
					iName.transform.localPosition = new Vector3 
						(iName.transform.localPosition.x, 
						 iName.transform.localPosition.y,
						 Mathf.Lerp(iName.transform.localPosition.z, -LimitVision, Time.deltaTime * 5));
					break;
				case Setup.XYZ_Sequence.negativeHeight_Width_Depth_OR_negativeHeight_Depth_Width:
					iName.transform.localPosition = new Vector3 
						(Mathf.Lerp(iName.transform.localPosition.x, -LimitVision, Time.deltaTime * 5), 
						 iName.transform.localPosition.y, 
						 iName.transform.localPosition.z);
					break;
				}
			}
			else{
				iName.GetComponent<MeshRenderer>().enabled = false;
				iBarHealth.GetComponent<MeshRenderer>().enabled = false;
			}
		}
		else{
			iName.GetComponent<TextMesh>().text = null;
			iBarHealth.GetComponent<MeshRenderer>().enabled = false;
		}
	}
}