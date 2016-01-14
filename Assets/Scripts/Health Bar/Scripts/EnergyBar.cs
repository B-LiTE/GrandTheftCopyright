/*
* Copyright (c) 2013 Mad Pixel Machine
* All Rights Reserved
*
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnergyBar : MonoBehaviour {

    
	public float valueCurrent = 50;
	public float valueMin = 0;
	public float valueMax = 100;
    
    public float ValueF {
        get {
            if (!animationEnabled) {
                return Mathf.Clamp((valueCurrent - valueMin) / (float) (valueMax - valueMin), 0, 1);
            } else {
                return Mathf.Clamp(animValueF, 0, 1);
            }
        }
        
        set {
            valueCurrent = Mathf.RoundToInt(value * (valueMax - valueMin) + valueMin);
        }
    }
    
    [HideInInspector]
    public bool animationEnabled;
    [HideInInspector]
    public float animValueF;

	public void Start (){
		if(this.gameObject.name != "Local_Player") GetComponent<EnergyBarRenderer>().enabled = false;
		if(GetComponent<NetworkView>()){
			valueMin = GetComponent<HealthSystem_Network>().minHealth;
			valueCurrent = GetComponent<HealthSystem_Network>().lostHealth;
			valueMax = GetComponent<HealthSystem_Network>().maxHealth;
		}
		else {
			valueMin = GetComponent<HealthSystem>().minHealth;
			valueCurrent = GetComponent<HealthSystem>().lostHealth;
			valueMax = GetComponent<HealthSystem>().maxHealth;
		}
	}

	public void Update() {
		if(GetComponent<NetworkView>()) valueCurrent = GetComponent<HealthSystem_Network>().lostHealth;
		else valueCurrent = GetComponent<HealthSystem>().lostHealth;
        valueCurrent = Mathf.Clamp(valueCurrent, valueMin, valueMax);
        
        if (animationEnabled) {
			valueCurrent = valueMin + (float) (animValueF * (valueMax - valueMin));
        }
    }
	 
	public void SetValueCurrent(float valueCurrent) {
        this.valueCurrent = valueCurrent;
    }
    
	public void SetValueMin(float valueMin) {
        this.valueMin = valueMin;
    }
    
	public void SetValueMax(float valueMax) {
        this.valueMax = valueMax;
    }
    
	public void SetValueF(float valueF) {
        ValueF = valueF;
    }
}