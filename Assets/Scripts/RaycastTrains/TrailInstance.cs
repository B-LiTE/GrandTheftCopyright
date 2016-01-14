using UnityEngine;
using System.Collections;

public class TrailInstance : MonoBehaviour {
	public LineRenderer lr;
	public int mul = 1;
	public float random = 0.035f;
	public AnimationCurve lifeTime;
	public Color currentColor;
	public Color colorOnEnd;
	
	public void Calculate(float distance, Vector3 startPosition, Vector3 endPosition){
		int mul_current = mul == 0 ? 1 : Mathf.Abs(mul);
		int vertexCount = (Mathf.FloorToInt(distance) + 1)* mul_current;
		
		StartCoroutine(SetSegments(vertexCount, startPosition, endPosition));
		
	}
	
	void Update(){
		currentColor = Color.Lerp(currentColor, colorOnEnd, Time.deltaTime* 1.5f);
		lr.SetColors(currentColor, currentColor);
		
		if(Mathf.RoundToInt(currentColor.a) == 0)
			Destroy(gameObject, 1.0f);
	}
	
	IEnumerator SetSegments(int vc, Vector3 sp, Vector3 ep){
		for(int i = 0; i < vc; i++){
			yield return new WaitForSeconds(0.01f);
			lr.SetVertexCount(i + 1);
			float ratio = (float)i / ((float)vc - 1.0f);
			Vector3 segmentPos = Vector3.Lerp(sp, ep, ratio);
			if(i == (vc - 1))
				lr.SetPosition(i, ep);
			else if(i == 0)
				lr.SetPosition(0, sp);
			else
				lr.SetPosition(i, segmentPos + Utils.RandomVector(random));
		}
		yield break;
	}
}
