using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GemLibrary : MonoBehaviour
{
	public enum GemSet
	{
		GEM = 0,		//!< Default
		BELL,
		//FRUITS,		// Apple, Blueberry, WaterMelon, Banana
		//VEGGIE,		// Tomato, Eggplant, Veggie, Potato

		TOTAL
	}

	public List<GemContainerSet> m_GemsSetList;

	static private GemLibrary m_Instance = null;

	static public GemLibrary Instance
	{
		get { return m_Instance; }
	}

	public List<GemContainerSet> GemsSetList
	{
		get { return m_GemsSetList; }
	}

	// Use this for initialization
	void Start ()
	{
		if ( m_Instance != null )
		{
			Destroy( this );
		}
		else
		{
			m_Instance = this;
			DontDestroyOnLoad( this );
		}
	}
}
