using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public GameObject m_Logo;

	// Animation variables
	private int m_nFrameNum;
	private float m_fAnimationIntervalTimer = 0.0f;
	private float m_fAnimationTimer = 0.0f;
	private bool m_bAnimating = false;
	private int m_nAnimatingFrame = -1;

	// Use this for initialization
	void Start ()
	{
		// Initialising animation timer
		m_nFrameNum = m_Logo.GetComponent<GemSpriteContainer>().m_Sprites.Length;
		m_fAnimationIntervalTimer = 0.0f;
		m_fAnimationTimer = 0.0f;
		m_bAnimating = false;
		m_nAnimatingFrame = -1;
	}
	
	// Update is called once per frame
	void Update ()
	{
		AnimateGems();
	}

	void AnimateGems()
	{
		if ( !m_bAnimating )
		{
			m_fAnimationIntervalTimer += Time.deltaTime;

			if ( m_fAnimationIntervalTimer >= GemSpawner.ANIMATION_INTERVAL )
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

			
			if ( m_fAnimationTimer >= GemSpawner.ANIMATION_RATE )
			{
				m_fAnimationTimer -= GemSpawner.ANIMATION_RATE;
				m_nAnimatingFrame = ( m_nAnimatingFrame + 1 ) % ( m_nFrameNum + 1 );

				int frame = m_nAnimatingFrame % m_nFrameNum;

				m_Logo.GetComponent<SpriteRenderer>().sprite = m_Logo.GetComponent<GemSpriteContainer>().m_GlowSprites[frame];

				if ( m_nAnimatingFrame == m_nFrameNum )
				{
					m_fAnimationIntervalTimer = 0.0f;
					m_fAnimationTimer = 0.0f;
					m_bAnimating = false;
					m_nAnimatingFrame = -1;

					m_Logo.GetComponent<SpriteRenderer>().sprite = m_Logo.GetComponent<GemSpriteContainer>().m_Sprites[0];
				}
			}
		}
	}

	public void GoPlayAlone()
	{
		GameObject.FindGameObjectWithTag( "Transition" ).GetComponent<Transition>().StartFadeOut( GoToGame );
	}

	static void GoToGame()
	{
		SceneManager.LoadScene("Game");
	}
}
