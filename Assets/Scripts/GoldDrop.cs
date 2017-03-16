using UnityEngine;
using System.Collections;

public class GoldDrop : MonoBehaviour
{
	// Animation constants
	public const float ANIMATION_INTERVAL = 0.5f;       //!< In seconds
	public const float ANIMATION_RATE = 0.125f;         //!< In seconds

	private bool m_bLinked = false;
	private bool m_bPetrified = false;
	//private Animator m_Animator = null;
	private SpriteRenderer m_SpriteRenderer = null;

	public Sprite[] m_Sprites;
	public Sprite[] m_LinkedSprites;
	public Sprite[] m_StonedSprites;

	// Animation variables
	private float m_fAnimationIntervalTimer = 0.0f;
	private float m_fAnimationTimer = 0.0f;
	private bool m_bAnimating = false;
	private int m_nAnimatingFrame = -1;

	// Use this for initialization
	void Start ()
	{
		//m_Animator = GetComponent<Animator>();
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		LinkGold( false );

		m_bPetrified = false;

		m_fAnimationIntervalTimer = 0.0f;
		m_fAnimationTimer = 0.0f;
		m_bAnimating = false;
		m_nAnimatingFrame = -1;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Animate();
	}

	public void LinkGold( bool link )
	{
		m_bLinked = link;
		//m_Animator.SetBool( "Shine", m_bLinked );

		m_SpriteRenderer.sprite = link ? m_LinkedSprites[0] : m_Sprites[0];
	}

	public bool GetLink()
	{
		return m_bLinked;
	}

	public void PetrifyGold()
	{
		m_bPetrified = true;

		m_SpriteRenderer.sprite = m_StonedSprites[0];
	}

	public bool GetPetrify()
	{
		return m_bPetrified;
	}

	void Animate()
	{
		if ( !m_bAnimating )
		{
			m_fAnimationIntervalTimer += Time.deltaTime;

			if ( m_fAnimationIntervalTimer >= ANIMATION_INTERVAL )
			{
				m_fAnimationIntervalTimer = 0.0f;
				m_fAnimationTimer = 0.0f;
				m_bAnimating = true;
				m_nAnimatingFrame = 0;
			}
		}
		else
		{
			m_fAnimationTimer += Time.deltaTime;

			
			if ( m_fAnimationTimer >= ANIMATION_RATE )
			{
				int frameNum = m_bPetrified ? m_StonedSprites.Length : ( m_bLinked ? m_LinkedSprites.Length : m_Sprites.Length );
				m_fAnimationTimer -= ANIMATION_RATE;
				m_nAnimatingFrame = ( m_nAnimatingFrame + 1 ) % ( frameNum + 1 );

				int frame = m_nAnimatingFrame % frameNum;

				if( m_bPetrified )
				{
					m_SpriteRenderer.sprite = m_StonedSprites[frame];
				}
				else if ( m_bLinked )
				{
					m_SpriteRenderer.sprite = m_LinkedSprites[frame];
				}
				else
				{
					m_SpriteRenderer.sprite = m_Sprites[frame];
				}

				if ( m_nAnimatingFrame == frameNum )
				{
					m_fAnimationIntervalTimer = 0.0f;
					m_fAnimationTimer = 0.0f;
					m_bAnimating = false;
					m_nAnimatingFrame = -1;
				}
			}
		}
	}
}
