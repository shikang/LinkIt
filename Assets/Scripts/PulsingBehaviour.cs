using UnityEngine;
using System.Collections;

public class PulsingBehaviour : MonoBehaviour
{
	public const float PULSING_CYCLE = 5.0f;								//!< In seconds 
	public const float HALF_PULSING_CYCLE = PULSING_CYCLE * 0.5f;			//!< In seconds
	public const float REST_TIME = 2.0f;									//!< In seconds
	public const int CYCLE_NUM = 2;
	public const float ONE_CYCLE_TIME = PULSING_CYCLE + REST_TIME;			//!< In seconds 
	public const float LIFETIME = CYCLE_NUM * ONE_CYCLE_TIME - REST_TIME;	//!< In seconds

	static Vector2 MAX_DIRECTION = new Vector2( -0.1f, -0.3f );
	static Vector2 MIN_DIRECTION = new Vector2( -0.2f, -0.5f );

	private SpriteRenderer m_SpriteRenderer;
	private float m_fTimer = 0.0f;
	private float m_fRestTimer = 0.0f;
	private bool m_bRest = false;
	private Vector2 m_Velocity;

	// Use this for initialization
	void Start ()
	{
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_fTimer = 0.0f;
		m_fRestTimer = 0.0f;
		m_bRest = false;

		Color c = m_SpriteRenderer.color;
		c.a = 0.0f;
		m_SpriteRenderer.color = c;

		m_Velocity = Random.Range( MIN_DIRECTION.x, MAX_DIRECTION.x ) * Vector3.right + 
					 Random.Range( MIN_DIRECTION.y, MAX_DIRECTION.y ) * Vector3.up;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Pulsing
		if ( !m_bRest )
		{
			m_fTimer += Time.deltaTime;

			if ( m_fTimer >= PULSING_CYCLE )
			{
				m_bRest = true;
				m_fTimer = 0.0f;
			}

			Color c = m_SpriteRenderer.color;
			// a = -x^2 + 1
			// x = ( t - c / 2 ) / ( c / 2 )
			float x = ( m_fTimer - HALF_PULSING_CYCLE ) / HALF_PULSING_CYCLE;
			c.a = -x * x + 1.0f;
			m_SpriteRenderer.color = c;
		}
		else
		{
			m_fRestTimer += Time.deltaTime;

			if ( m_fRestTimer >= REST_TIME )
			{
				m_fRestTimer = 0.0f;
				m_bRest = false;
			}
		}

		// Moving
		Vector3 pos = transform.position;
		pos.x += m_Velocity.x * Time.deltaTime;
		pos.y += m_Velocity.y * Time.deltaTime;

		transform.position = pos;
	}
}
