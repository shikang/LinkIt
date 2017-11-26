using UnityEngine;
using System.Collections;

public class GemNetworkInfo : MonoBehaviour
{
#if LINKIT_COOP
	private int m_ID;
	private Vector3 m_HalfDimension;
	private Gem m_Gem = null;
	private bool m_bOtherLinked = false;

	public int ID
	{
		get
		{
			return m_ID;
		}
		set
		{
			m_ID = value;
		}
	}

	public bool OtherLinked
	{
		get
		{
			return m_bOtherLinked;
		}
	}

	// Use this for initialization
	void Start ()
	{
		m_HalfDimension = Camera.main.ScreenToWorldPoint( new Vector3( ( Screen.width ), ( Screen.height ) ) );
		m_Gem = GetComponent<Gem>();

		m_bOtherLinked = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void LinkNetworkGem( bool linked )
	{
		if ( m_bOtherLinked != linked )
		{
			GemSpawner spawner = GameObject.Find( "GemSpawner" ).GetComponent<GemSpawner>();

			// If link, scale up change sprite
			if ( linked )
			{
				spawner.SetLinkGemEffect( m_Gem );
			}
			// If not linked, scale back change sprite
			else
			{
				spawner.UnsetLinkGemEffect( m_Gem );
			}
		}
		m_bOtherLinked = linked;
	}
#endif	// LINKIT_COOP
}
