using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	public const string PLAY_WITH_STRANGER_TEXT = "Play with stranger";
	public const string PLAY_WITH_FRIEND_TEXT = "Play with friend";

	public enum eScreen
	{
		MAIN_MENU,
		CO_OP,
		ITEM,
	}

	public GameObject m_Logo;
	public GameObject[] m_Screens;

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

		GoToScreen( (int)eScreen.MAIN_MENU );
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

	public void GoToScreen( int screen )
	{
		foreach ( GameObject s in m_Screens )
		{
			s.SetActive( false );
		}

		m_Screens[screen].SetActive( true );
	}

	public void ChangeOnlineButtonText( string password )
	{
		Text t = m_Screens[(int)eScreen.CO_OP].transform.FindChild("Online Button").GetComponentInChildren<Text>();
		if ( password == "" )
			t.text = PLAY_WITH_STRANGER_TEXT;
		else
			t.text = PLAY_WITH_FRIEND_TEXT;
	}
}
