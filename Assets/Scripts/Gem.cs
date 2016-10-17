using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour
{
	public int m_nGemType = GemSpawner.INVALID_GEM;
	public int m_nLane = GemSpawner.INVALID_LANE;
	public bool m_bLinked = false;
	public int m_nSequenceIndex = -1;
	public bool m_nPetrified = false;

	public int GemType
	{
		get
		{
			return m_nGemType;
		}

		set
		{
			m_nGemType = value;
		}
	}

	public int Lane
	{
		get
		{
			return m_nLane;
		}

		set
		{
			m_nLane = value;
		}
	}

	public bool Linked
	{
		get
		{
			return m_bLinked;
		}

		set
		{
			m_bLinked = value;
		}
	}

	public int SequenceIndex
	{
		get
		{
			return m_nSequenceIndex;
		}

		set
		{
			m_nSequenceIndex = value;
		}
	}

	public bool Petrified
	{
		get
		{
			return m_nPetrified;
		}

		set
		{
			m_nPetrified = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnDestroy()
	{
		if ( NetworkManager.IsConnected() && !NetworkManager.IsPlayerOne() )
		{
			GameObject spawner = GameObject.Find( "GemSpawner" );
			if ( spawner )
			{
				spawner.GetComponent<GemSpawner>().RemoveNetworkGem( this );
			}
		}
	}
}
