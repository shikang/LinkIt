using UnityEngine;
using System.Collections;

public class Link : MonoBehaviour
{
	private bool m_bLinking = false;
	private bool m_bCheckForDestroy = false;
	private Vector3 m_PreviousPoint;
	private Vector3 m_CurrentPoint;
	private int m_nLinkType = GemSpawner.INVALID_GEM;
	private GemSpawner m_GemSpawner;
	private ParticleSystem m_DustEmitter;
	private TrailRenderer m_TrailRenderer;

	public bool Linking
	{
		get
		{
			return m_bLinking;
		}
	}

	public bool CheckForDestroy
	{
		get
		{
			return m_bCheckForDestroy;
		}
		set
		{
			m_bCheckForDestroy = value;
		}
	}

	public Vector3 PreviousPoint
	{
		get
		{
			return m_PreviousPoint;
		}
	}

	public Vector3 CurrentPoint
	{
		get
		{
			return m_CurrentPoint;
		}
	}

	public int LinkType
	{
		get
		{
			return m_nLinkType;
		}
	}

	// Use this for initialization
	void Start ()
	{
		m_bLinking = false;
		m_bCheckForDestroy = false;
		m_nLinkType = GemSpawner.INVALID_GEM;
		m_GemSpawner = GameObject.Find("GemSpawner").GetComponent<GemSpawner>();
		m_DustEmitter = GameObject.Find("Fairy Dust").GetComponent<ParticleSystem>();
		m_TrailRenderer = GetComponent<TrailRenderer>();
	}

	public void BreakLink()
	{
		m_TrailRenderer.Clear();

		m_bCheckForDestroy = m_bLinking;
		m_bLinking = false;
		m_nLinkType = GemSpawner.INVALID_GEM;

		// Get the material list of the trail as per the scripting API.
		Material trail = m_TrailRenderer.material;

		// Set the color of the material to tint the trail.
		trail.SetColor( "_Color", m_GemSpawner.m_NeutralColor);

		m_DustEmitter.startColor = m_GemSpawner.m_NeutralColor;

		// gameObject.SetActive( false );
	}

	public void StartLink()
	{
		Debug.Log("Start Link!");
		Vector3 mousePos = Input.mousePosition;
		Vector3 pos = Camera.main.ScreenToWorldPoint( mousePos );
		transform.position = new Vector3 ( pos.x, pos.y, 0.0f );

		m_DustEmitter.Stop();
		m_DustEmitter.transform.position = new Vector3(pos.x, pos.y, 0.0f);
		m_DustEmitter.Play();

		m_TrailRenderer.Clear();

		// gameObject.SetActive( true );

		m_bLinking = true;
		m_PreviousPoint = new Vector3 ( pos.x, pos.y, 0.0f );
		m_CurrentPoint = new Vector3 ( pos.x, pos.y, 0.0f );
	}
	
	// Link
	public void DrawLink ()
	{
		// Left button up
		if ( Input.GetMouseButtonUp( 0 ) )
		{
			BreakLink();
		}
		else if ( /*Input.GetMouseButtonDown( 0 ) ||*/ Input.GetMouseButton( 0 ) )
		{
			/*
			if ( Input.GetMouseButtonDown( 0 ) )
				m_bLinking = true;
			*/

			if ( m_bLinking )
			{
				Vector3 mousePos = Input.mousePosition;
				Vector3 pos = Camera.main.ScreenToWorldPoint( mousePos );
				transform.position = new Vector3 ( pos.x, pos.y, 0.0f );

				m_DustEmitter.transform.position = new Vector3 ( pos.x, pos.y, 0.0f );

				/*
				if ( Input.GetMouseButtonDown( 0 ) )
				{
					m_PreviousPoint = new Vector3 ( pos.x, pos.y, 0.0f );
				}
				else
				{
					m_PreviousPoint = m_CurrentPoint;
				}
				*/

				m_PreviousPoint = m_CurrentPoint;
				m_CurrentPoint = new Vector3 ( pos.x, pos.y, 0.0f );
			}
		}
	}

	// Change link colour
	public void ChangeLinkColor( int linkType )
	{
		if ( linkType < 0 || linkType >= m_GemSpawner.m_LinkColours.Length )
		{
			Debug.Log ( "Invalid link colour!" ); 
			return;
		}

		if ( m_nLinkType == linkType )
			return;

		m_nLinkType = linkType;

		// Get the material list of the trail as per the scripting API.
		Material trail = m_TrailRenderer.material;

		// Set the color of the material to tint the trail.
		trail.SetColor( "_Color", m_GemSpawner.m_LinkColours[linkType] );

		m_DustEmitter.startColor = m_GemSpawner.m_LinkColours[linkType];
	}

	public void SetLinkOpacity( float alpha )
	{
		if ( m_TrailRenderer == null )
			m_TrailRenderer = GetComponent<TrailRenderer>();

		// Get the material list of the trail as per the scripting API.
		Material trail = m_TrailRenderer.material;

		Color c = trail.GetColor( "_Color" );
		c.a = alpha;

		// Set the color of the material to tint the trail.
		trail.SetColor( "_Color", c );
	}
}
