using UnityEngine;
using System.Collections;

public class Transition : MonoBehaviour
{
	const float FADE_TIME = 1.0f;

	bool m_bFadeIn = true;
	bool m_bFadeOut = false;
	float m_fTimer = 0.0f;

	SpriteRenderer m_Overlay;

	public delegate void FadeOut();

	public FadeOut m_FadeOut;

	// Use this for initialization
	void Start ()
	{
		m_bFadeIn = true;
		m_bFadeOut = false;
		m_fTimer = 0.0f;

		transform.FindChild( "Overlay" ).gameObject.SetActive( true );
		m_Overlay = GetComponentInChildren<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ( !m_bFadeIn && !m_bFadeOut )
			return;

		m_fTimer += Time.deltaTime;

		Color c = m_Overlay.color;
		if ( m_bFadeIn )
		{
			c.a -= Time.deltaTime / FADE_TIME;
		}
		else if ( m_bFadeOut )
		{
			c.a += Time.deltaTime / FADE_TIME;
		}
		m_Overlay.color = c;

		if ( m_fTimer >= FADE_TIME )
		{
			if ( m_bFadeIn )
			{
				m_bFadeIn = false;
			}
			else if ( m_bFadeOut )
			{
				m_bFadeOut = false;

				if ( m_FadeOut != null )
					m_FadeOut.Invoke();
			}

			m_fTimer = 0.0f;
		}
	}

	public void StartFadeOut( FadeOut del )
	{
		m_FadeOut = del;
		m_bFadeOut = true;
	}
}
