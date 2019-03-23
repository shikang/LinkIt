using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GemLibrary : MonoBehaviour
{
	public enum GemSet
	{
		GEM = 0,		//!< Default
		SUITS,          //!< Heart, Spade, Clover, Diamond
        // ELEMENTS,    //!< Fire, Water Droplet, Wind, Lightning Bolt

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
			Destroy( this.gameObject );
		}
		else
		{
			m_Instance = this;
			DontDestroyOnLoad( this );
		}
	}
}
