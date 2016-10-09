using UnityEngine;
using System.Collections;

public class PulsingParticleEmitter : MonoBehaviour
{
	public const float MIN_SCALE = 0.1f;
	public const float MAX_SCALE = 0.7f;

	public const float MIN_X = 0.2f;		//!< In ratio
	public const float MAX_X = 1.2f;        //!< In ratio

	public const float MIN_Y = 0.3f;        //!< In ratio
	public const float MAX_Y = 1.0f;        //!< In ratio

	public const float EMIT_RATE = 2.5f;	//!< In seconds

	public GameObject m_ParticlePrefab;

	private float m_fMinX;
	private float m_fMaxX;
	private float m_fMinY;
	private float m_fMaxY;
	private float m_fTimer = 0.0f;

	// Use this for initialization
	void Start ()
	{
		m_fTimer = 0.0f;

		Vector3 halfDimension = Camera.main.ScreenToWorldPoint( new Vector3( ( Screen.width ), ( Screen.height ) ) );
		Vector3 fullDimension = 2.0f * halfDimension;

		m_fMinX = -halfDimension.x + MIN_X * fullDimension.x;
		m_fMaxX = -halfDimension.x + MAX_X * fullDimension.x;
		m_fMinY = -halfDimension.y + MIN_Y * fullDimension.y;
		m_fMaxY = -halfDimension.y + MAX_Y * fullDimension.y;

		CreateDust();
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_fTimer += Time.deltaTime;

		if ( m_fTimer >= EMIT_RATE )
		{
			m_fTimer -= EMIT_RATE;

			CreateDust();
		}
	}

	void CreateDust()
	{
		float x = Random.Range( m_fMinX, m_fMaxX );
		float y = Random.Range( m_fMinY, m_fMaxY );
		float s = Random.Range( MIN_SCALE, MAX_SCALE );

		GameObject dust = ( GameObject )Instantiate( m_ParticlePrefab, new Vector3( x, y, 0.0f ), Quaternion.identity );
		dust.transform.localScale = s * Vector3.one;
		Destroy( dust, PulsingBehaviour.LIFETIME + Time.deltaTime );
	}
}
