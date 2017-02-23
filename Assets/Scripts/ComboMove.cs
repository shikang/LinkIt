using UnityEngine;
using System.Collections;

public class ComboMove : MonoBehaviour
{
	const float COMBO_MOVE_TIME = 0.5f;

	private bool m_bEnter = false;
	private bool m_bExit = false;
	private bool m_bAllTheWay = false;

	private float m_fStart;
	private float m_fDestination;
	private float m_fTimer = 0.0f;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		if ( m_bEnter )
		{
			m_fTimer += Time.deltaTime;

			float factor = Mathf.Pow( Mathf.Clamp( m_fTimer / COMBO_MOVE_TIME, 0.0f, 1.0f ), 2.0f );
			Vector3 pos = transform.localPosition;
			pos.y = m_fStart - factor * m_fStart;
			transform.localPosition = pos;

			if ( factor >= 1.0f )
			{
				m_bEnter = false;
				m_fTimer = 0.0f;
			}
		}
		else if ( m_bExit )
		{
			m_fTimer += Time.deltaTime;

			float factor = Mathf.Pow( Mathf.Clamp( m_fTimer / COMBO_MOVE_TIME, 0.0f, 1.0f ), 2.0f );
			Vector3 pos = transform.localPosition;
			pos.y = factor * m_fDestination;
			transform.localPosition = pos;

			if ( factor >= 1.0f )
			{
				m_bExit = false;
				m_fTimer = 0.0f;
				Destroy( gameObject );
			}
		}
		else if ( m_bAllTheWay )
		{
			m_fTimer += Time.deltaTime;

			float factor = Mathf.Pow( Mathf.Clamp( m_fTimer / ( 1.5f * COMBO_MOVE_TIME ), 0.0f, 1.0f ), 2.0f );
			Vector3 pos = transform.localPosition;
			pos.y = m_fStart + factor * ( m_fDestination - m_fStart );
			transform.localPosition = pos;

			if ( factor >= 1.0f )
			{
				m_bAllTheWay = false;
				m_fTimer = 0.0f;
				Destroy( gameObject );
			}
		}
	}

	public void StartEnter()
	{
		m_bEnter = true;
		m_fTimer = 0.0f;
		m_fStart = transform.localPosition.y;
	}

	public void StartExit(float fDestination )
	{
		m_bExit = true;
		m_fDestination = fDestination;
	}

	public void StartAllTheWay( float fDestination )
	{
		m_bAllTheWay = true;
		m_fTimer = 0.0f;
		m_fStart = transform.localPosition.y;
		m_fDestination = fDestination;
	}
}
