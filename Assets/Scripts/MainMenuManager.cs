using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	public const string PLAY_WITH_STRANGER_TEXT = "Play with stranger";
	public const string PLAY_WITH_FRIEND_TEXT = "Play with friend";

	public const float SCREEN_ANIMATE_TIME = 0.125f;		//!< In seconds

	public enum eScreen
	{
		MAIN_MENU,
		CO_OP,
		ITEM,

		TOTAL
	}

	public GameObject m_Logo;
	public GameObject[] m_Screens;

	// Animation variables
	private int m_nFrameNum;
	private float m_fAnimationIntervalTimer = 0.0f;
	private float m_fAnimationTimer = 0.0f;
	private bool m_bAnimating = false;
	private int m_nAnimatingFrame = -1;

	// Screens
	private int m_CurrentScreen;
	private int m_PreviousScreen;
	private bool m_bScreenAnimate = false;
	private float m_fScreenAnimateTimer = 0.0f;
	private float m_fScreenWidth = 0.0f;
	private float m_fScreenFrom = 0.0f;

	// Use this for initialization
	void Start ()
	{
		// Initialising animation timer
		m_nFrameNum = m_Logo.GetComponent<GemSpriteContainer>().m_Sprites.Length;
		m_fAnimationIntervalTimer = 0.0f;
		m_fAnimationTimer = 0.0f;
		m_bAnimating = false;
		m_nAnimatingFrame = -1;

		m_CurrentScreen = (int)eScreen.MAIN_MENU;
		m_bScreenAnimate = false;
		m_fScreenAnimateTimer = 0.0f;
		m_fScreenWidth = m_Screens[(int)eScreen.MAIN_MENU].GetComponent<RectTransform>().sizeDelta.x;

		GoToScreen( (int)eScreen.MAIN_MENU );
	}
	
	// Update is called once per frame
	void Update ()
	{
		AnimateGems();
		AnimationScreen();
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

	void AnimationScreen()
	{
		if ( m_bScreenAnimate )
		{
			m_fScreenAnimateTimer += Time.deltaTime;

			float factor = Mathf.Pow( Mathf.Clamp( m_fScreenAnimateTimer / SCREEN_ANIMATE_TIME, 0.0f, 1.0f ), 2.0f );
			Vector3 prevPos = m_Screens[m_PreviousScreen].GetComponent<RectTransform>().localPosition;
			prevPos.x = Mathf.Lerp( 0.0f, -m_fScreenFrom, factor );
			m_Screens[m_PreviousScreen].GetComponent<RectTransform>().localPosition = prevPos;

			Vector3 currentPos = m_Screens[m_CurrentScreen].GetComponent<RectTransform>().localPosition;
			currentPos.x = Mathf.Lerp( m_fScreenFrom, 0.0f, factor );
			m_Screens[m_CurrentScreen].GetComponent<RectTransform>().localPosition = currentPos;

			m_bScreenAnimate = m_fScreenAnimateTimer < SCREEN_ANIMATE_TIME;

			if ( !m_bScreenAnimate )
			{
				m_Screens[m_PreviousScreen].SetActive( false );
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

		m_PreviousScreen = m_CurrentScreen;
		m_CurrentScreen = screen;

		m_Screens[m_PreviousScreen].SetActive( true );
		m_Screens[m_CurrentScreen].SetActive( true );

		m_bScreenAnimate = m_PreviousScreen != m_CurrentScreen;
		if ( m_bScreenAnimate )
		{
			m_fScreenAnimateTimer = 0.0f;

			// Setting screen to correct position
			Vector3 prevPos = m_Screens[m_PreviousScreen].GetComponent<RectTransform>().localPosition;
			prevPos.x = 0.0f;
			m_Screens[m_PreviousScreen].GetComponent<RectTransform>().localPosition = prevPos;

			Vector3 currentPos = m_Screens[m_CurrentScreen].GetComponent<RectTransform>().localPosition;
			currentPos.x = m_PreviousScreen > m_CurrentScreen ? -m_fScreenWidth : m_fScreenWidth;
			m_Screens[m_CurrentScreen].GetComponent<RectTransform>().localPosition = currentPos;

			m_fScreenFrom = currentPos.x;
		}
	}

	public void ChangeOnlineButtonText( string password )
	{
		Text t = m_Screens[(int)eScreen.CO_OP].transform.FindChild("Online Button").GetComponentInChildren<Text>();
		if ( password == "" )
			t.text = PLAY_WITH_STRANGER_TEXT;
		else
			t.text = PLAY_WITH_FRIEND_TEXT;
	}

	public void EnableBackButton( eScreen screen, bool enable )
	{
		if ( screen == eScreen.MAIN_MENU )
			return;

		Transform t = m_Screens[(int)screen].transform.FindChild( "Back Button" );
		Button b = t.GetComponent<Button>();
		b.enabled = enable;

		ButtonScript bs = t.GetComponent<ButtonScript>();
		if ( enable )
			bs.SetEnable();
		else
			bs.SetDisable();
	}
}
