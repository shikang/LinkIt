using UnityEngine;
using System.Collections;

public class Pulsing : MonoBehaviour
{
	public float m_fMinAlpha = 0.0f;
	public float m_fMaxAlpha = 1.0f;

	public float m_fPulseCycleRate = 1.0f;  //!< In Seconds

	private float m_fTimer = 0.0f;
	private bool m_bFadeIn = false;
	private bool m_bFadeOut = false;
	private SpriteRenderer m_bSpriteRenderer = null;

	// Use this for initialization
	void Start ()
	{
		m_fTimer = 0.0f;

		m_bFadeIn = false;
		m_bFadeOut = false;

		m_bSpriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ( !m_bFadeIn && !m_bFadeOut )
			return;

		m_fTimer += Time.deltaTime;

		float factor = Mathf.Pow( Mathf.Clamp( m_fTimer / m_fPulseCycleRate, 0.0f, 1.0f ), 2.0f );

		Color c = m_bSpriteRenderer.color;

		if ( m_bFadeIn )
		{
			c.a = m_fMinAlpha + factor * ( m_fMaxAlpha - m_fMinAlpha );
		}
		else if ( m_bFadeOut )
		{
			c.a = m_fMinAlpha + ( 1.0f - factor ) * ( m_fMaxAlpha - m_fMinAlpha );
		}

		m_bSpriteRenderer.color = c;

		if ( m_fTimer >= m_fPulseCycleRate )
		{
			m_bFadeIn = !m_bFadeIn;
			m_bFadeOut = !m_bFadeOut;
			m_fTimer = 0.0f;
		}
	}

	public void StartFadeIn()
	{
		m_bFadeIn = true;
		m_bFadeOut = false;

		m_fTimer = 0.0f;
	}

	public void StartFadeOut()
	{
		m_bFadeOut = true;
		m_bFadeIn = false;

		m_fTimer = 0.0f;
	}

	public void StopPulsing()
	{
		m_bFadeOut = false;
		m_bFadeIn = false;

		m_fTimer = 0.0f;
	}

	public bool IsPulsing()
	{
		return m_bFadeIn || m_bFadeOut;
	}
}
