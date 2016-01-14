using UnityEngine;
using System.Collections;

[System.Serializable]
public class Setup{

	public XYZ_Sequence currentSlot;

	[System.Serializable]
	public class XYZ{
		public XYZ_Sequence Orientations;
	}

	[System.Serializable]
	public enum XYZ_Sequence {
		Width_Height_Depth_OR_Depth_Height_Width,
		Height_Width_Depth_OR_Height_Depth_Width,
		Width_Depth_Height_OR_Depth_Width_Height,
		Width_negativeHeight_Depth_OR_Depth_negativeHeight_Width,
		negativeHeight_Width_Depth_OR_negativeHeight_Depth_Width,
		Width_Depth_negativeHeight_OR_Depth_Width_negativeHeight,
	}
}

public class OrientationName : MonoBehaviour {
	public Setup Sequence;
}
