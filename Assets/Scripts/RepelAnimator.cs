using UnityEngine;
using System.Collections;

public class RepelAnimator : MonoBehaviour
{
	public const float LIFETIME = 1.3f;         //!< In seconds
	public const float ANITIME = 0.8f;			//!< In seconds
	public const float LEFTOVER = LIFETIME - ANITIME;
	public const float FADE_START_LAST_X_FRAME = 3;

	public Sprite[] m_Sprites;

	private float m_fTotalTimer = 0.0f;
	private float m_fAnimationTimer = 0.0f;
	private int m_nAnimatingFrame = 0;
	private int m_nFrameNum;
	private float m_fAnimationRate;

	// Use this for initialization
	void Start ()
	{
		m_fTotalTimer = 0.0f;
		m_fAnimationTimer = 0.0f;
		m_nAnimatingFrame = 0;
		m_nFrameNum = m_Sprites.Length;
		m_fAnimationRate = ANITIME / m_nFrameNum;
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_fTotalTimer += Time.deltaTime;
		m_fAnimationTimer += Time.deltaTime;

		if ( m_fAnimationTimer >= m_fAnimationRate )
		{
			m_fAnimationTimer -= m_fAnimationRate;
			m_nAnimatingFrame = Mathf.Min( m_nAnimatingFrame + 1, m_nFrameNum - 1 );

			int frame = Mathf.Min( m_nAnimatingFrame, m_nFrameNum - 1 );

			GetComponent<SpriteRenderer>().sprite = m_Sprites[frame];
		}

		if ( m_nAnimatingFrame >= m_nFrameNum - FADE_START_LAST_X_FRAME )
		{
			float leftTime = m_fTotalTimer - ANITIME + FADE_START_LAST_X_FRAME * m_fAnimationRate;
			float alpha = Mathf.SmoothStep( 1.0f, 0.0f, leftTime / ( LEFTOVER ) );
			Color c = GetComponent<SpriteRenderer>().color;
			c.a = alpha;
			GetComponent<SpriteRenderer>().color = c;
		}
	}
}
