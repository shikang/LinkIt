using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemLibrary : MonoBehaviour
{
	public enum GemSet
	{
		GEM = 0,
		BELL,
		FRUITS,		// Apple, Blueberry, WaterMelon, Banana
		VEGGIE,		// Tomato, Eggplant, Veggie, Potato

		TOTAL
	}

	public List<GemContainerSet> m_GemsSetList;

	// Use this for initialization
	void Start ()
	{
	
	}
}
