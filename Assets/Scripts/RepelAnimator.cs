using UnityEngine;
using System.Collections;

public class RepelAnimator : MonoBehaviour
{
	public const float LIFETIME = 0.5f;         //!< In seconds

	public Sprite[] m_Sprites;

	private float m_fAnimationTimer = 0.0f;
	private int m_nAnimatingFrame = 0;
	private int m_nFrameNum;
	private float m_fAnimationRate;

	// Use this for initialization
	void Start ()
	{
		m_fAnimationTimer = 0.0f;
		m_nAnimatingFrame = 0;
		m_nFrameNum = m_Sprites.Length;
		m_fAnimationRate = LIFETIME / m_nFrameNum;
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_fAnimationTimer += Time.deltaTime;

		if ( m_fAnimationTimer >= m_fAnimationRate )
		{
			m_fAnimationTimer -= m_fAnimationRate;
			m_nAnimatingFrame = ( m_nAnimatingFrame + 1 ) % ( m_nFrameNum + 1 );

			int frame = m_nAnimatingFrame % m_nFrameNum;

			GetComponent<SpriteRenderer>().sprite = m_Sprites[frame];
		}
	}
}
