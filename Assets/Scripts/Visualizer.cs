using UnityEngine;
using System.Collections;

[System.Serializable]
public class VisualElement{
	public Color color = Color.green;
	public Transform segment;
	public float size = 0.3f;
}

public class Visualizer : MonoBehaviour {
	public VisualElement[] visualElements;
	
	void OnDrawGizmos() {
       	foreach(VisualElement vElement in visualElements){
       		Gizmos.color = vElement.color;
			Gizmos.DrawSphere(vElement.segment.position, vElement.size);
		}
    }
}
