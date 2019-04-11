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
		// Global define
		// https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
#if LINKIT_COOP
		CO_OP,
#else	// !LINKIT_COOP
		//CO_OP_DUMMY,
#endif	// LINKIT_COOP
		//ITEM,

		PLAY,
		RANKINGS,
		SHOP,
		OPTIONS,
		CREDITS,
		ACHIEVEMENTS,
		BOOSTER,
        HIGHSCORE,

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

	public GameObject m_uPlayerCoins;
    public GameObject m_ScoreText;
    public GameObject m_ComboText;

    private int m_CreditVisitNum = 0;

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

        m_CreditVisitNum = 0;

		AudioManager.Instance.UpdateBGM("Menu");
		AudioManager.Instance.PlaySoundEvent(SOUNDID.BGM);

#if !LINKIT_COOP
        Transform coopBtn = m_Screens[(int)eScreen.MAIN_MENU].transform.Find( "Co-op Button" );
		if ( coopBtn != null )
		{
			coopBtn.gameObject.SetActive( false );
		}
#endif	// !LINKIT_COOP

		GoToScreen( (int)eScreen.MAIN_MENU );
	}
	
	// Update is called once per frame
	void Update ()
	{
		AnimateGems();
		AnimationScreen();
		m_uPlayerCoins.GetComponent<Text>().text = GameData.Instance.m_Coin.ToString();
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
		AudioManager.Instance.PlaySoundEvent(SOUNDID.MENU_START);
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

		AudioManager.Instance.PlaySoundEvent(SOUNDID.MENU_CLICK);
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

#if UNITY_IOS
        if (m_CurrentScreen == (int)eScreen.ACHIEVEMENTS)
        {
            GameObject btn = GameObject.FindGameObjectWithTag( "Platform Specific Achievement Button" );
            if ( btn != null )
                btn.SetActive( false );
            //	GPManager.ShowAchievementsUI();
        }
#endif

#if UNITY_ANDROID
        if ( m_CurrentScreen == (int)eScreen.CREDITS )
        {
            ++m_CreditVisitNum;

            if ( m_CreditVisitNum == 1 )
            {
                GooglePlayService.ProgressAcheivement( GPGSIds.achievement_visit_credits_screen, 1.0f );
            }

            if ( m_CreditVisitNum <= 5 )
            {
                GooglePlayService.ProgressAcheivement( GPGSIds.achievement_visit_credits_screen_more, (float)m_CreditVisitNum/ 5.0f );
            }
        }
#endif
    }

#if LINKIT_COOP
	public void ChangeOnlineButtonText( string password )
	{
		Text t = m_Screens[(int)eScreen.CO_OP].transform.FindChild("Online Button").GetComponentInChildren<Text>();
		if ( password == "" )
			t.text = PLAY_WITH_STRANGER_TEXT;
		else
			t.text = PLAY_WITH_FRIEND_TEXT;
	}
#else  // !LINKIT_COOP
    public void ChangeOnlineButtonText(string password)
	{
		// Empty
	}
#endif  // LINKIT_COOP

	public void EnableBackButton( eScreen screen, bool enable )
	{
		if ( screen == eScreen.MAIN_MENU )
			return;

		Transform t = m_Screens[(int)screen].transform.Find( "Back Button" );
		Button b = t.GetComponent<Button>();
		b.enabled = enable;

		ButtonScript bs = t.GetComponent<ButtonScript>();
		if ( enable )
			bs.SetEnable();
		else
			bs.SetDisable();
	}

	private static void EnableButtonsImpl( bool enable )
	{
		GameObject[] objs = GameObject.FindGameObjectsWithTag( "Button" );

		foreach ( GameObject button in objs )
		{
			button.GetComponent<Button>().interactable = enable;
		}
	}

	public static void DisableButtons()
	{
		EnableButtonsImpl( false );
	}

	public static void EnableButtons()
	{
		EnableButtonsImpl( true );
	}

    public void RefreshHighScore()
    {
        if ( m_ScoreText != null )
            m_ScoreText.GetComponent<Text>().text = GameData.Instance.m_HighScore.ToString();

        if ( m_ComboText != null )
            m_ComboText.GetComponent<Text>().text = GameData.Instance.m_HighestCombo.ToString();
    }

	public void SetBGMVol(Slider slider)
	{
		GameData.Instance.m_vol_BGM = (int)(slider.value * 100.0f);
		AudioManager.Instance.SetBGMVol(slider.value * 100.0f);
	}

	public void SetSFXVol(Slider slider)
	{
		GameData.Instance.m_vol_SFX = (int)(slider.value * 100.0f);
		AudioManager.Instance.SetSFXVol(slider.value * 100.0f);
	}

	public void SetVibration(Toggle toggle)
	{
		GameData.Instance.m_VibrationDisable = toggle.isOn;
	}
}
