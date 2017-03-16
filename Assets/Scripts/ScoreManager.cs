using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
	public const float SCREEN_ANIMATE_TIME = 0.125f;        //!< In seconds
	public const float TIME_TO_ACTUAL_COINS = 0.5f;			//!< In seconds

	PlayerStatistics m_PlayerStats;
	GemDetails m_GemDetails;

	public GameObject m_Score;
	public GameObject m_BestScore;
	public GameObject[] m_Counters;
	public GameObject m_LeakedCounter;
	public GameObject m_ComboCounter;

	public GameObject[] m_DummyGems;
	public GameObject m_DummyLeaked;

	public GameObject m_Coins;

	public GameObject m_ScoreCanvas;
	public GameObject m_GachaCanvas;

	// Animation variables
	private int m_nFrameNum;
	private float m_fAnimationIntervalTimer = 0.0f;
	private float m_fAnimationTimer = 0.0f;
	private bool m_bAnimating = false;
	private int m_nAnimatingFrame = -1;

	private int m_CurrentCoins = 0;
	private int m_nShowingCoins = 0;
	private int m_nPrevCoins = 0;
	private float m_fCoinsTimer = 0.0f;

	// Screens
	private bool m_bIsCurrentScreenScore = true;
	private bool m_bScreenAnimate = false;
	private float m_fScreenAnimateTimer = 0.0f;
	private float m_fScreenWidth = 0.0f;
	private float m_fScreenFrom = 0.0f;

	// Use this for initialization
	void Start ()
	{
		// Getting stats
		m_PlayerStats = GameObject.FindGameObjectWithTag( "Player Statistics" ).GetComponent<PlayerStatistics>();
		m_Score.GetComponent<Text>().text = m_PlayerStats.m_nScore.ToString();

		int gemDestroyed = 0;
		for ( int i = 0; i < m_PlayerStats.m_aDestroyCount.Length; ++i )
		{
			m_Counters[i].GetComponent<Text>().text = m_PlayerStats.m_aDestroyCount[i].ToString();
			gemDestroyed += m_PlayerStats.m_aDestroyCount[i];
		}

		m_LeakedCounter.GetComponent<Text>().text = m_PlayerStats.m_nLeakCount.ToString();
		m_ComboCounter.GetComponent<Text>().text = m_PlayerStats.m_nMaxCombo.ToString();

		if ( GameData.Instance.m_HighScore < m_PlayerStats.m_nScore )
		{
			GameData.Instance.m_HighScore = m_PlayerStats.m_nScore;
		}

		m_BestScore.GetComponent<Text>().text = "Best " + GameData.Instance.m_HighScore.ToString();

		// Initialising animation timer
		m_nFrameNum = m_PlayerStats.m_aGems[0].GetComponent<GemSpriteContainer>().m_Sprites.Length;
		for (int i = 1; i < m_PlayerStats.m_aGems.Length; ++i)
		{
			int num = m_PlayerStats.m_aGems[i].GetComponent<GemSpriteContainer>().m_Sprites.Length;
			m_nFrameNum = m_nFrameNum > num ? num : m_nFrameNum;
		}
		m_fAnimationIntervalTimer = 0.0f;
		m_fAnimationTimer = 0.0f;
		m_bAnimating = false;
		m_nAnimatingFrame = -1;

		if( NetworkManager.IsConnected() )
		{
			NetworkManager.Disconnect();
		}

		GameData.Instance.m_Coin += m_PlayerStats.m_nCoinsGain;
		m_CurrentCoins = GameData.Instance.m_Coin + gemDestroyed;
		m_nShowingCoins = GameData.Instance.m_Coin;
		m_nPrevCoins = GameData.Instance.m_Coin;
		m_fCoinsTimer = 0.0f;
		GameData.Instance.m_Coin += gemDestroyed;

		Text coinsText = m_Coins.GetComponent<Text>();
		coinsText.text = m_nShowingCoins.ToString();// + " (+" + m_PlayerStats.m_nCoinsGain + ")";

		// Save
		SaveLoad.Save();

		if ( GameObject.FindGameObjectWithTag( "Gem Details" ) != null )
			m_GemDetails = GameObject.FindGameObjectWithTag( "Gem Details" ).GetComponent<GemDetails>();

		m_bIsCurrentScreenScore = true;
		m_bScreenAnimate = false;
		m_fScreenAnimateTimer = 0.0f;
		m_fScreenWidth = m_ScoreCanvas.GetComponent<RectTransform>().sizeDelta.x;
		GoToScore();
	}
	
	// Update is called once per frame
	void Update ()
	{
		AnimateGems();
		AnimateCoins();
		AnimationScreen();
	}

	void OnDestroy()
	{
		if ( m_PlayerStats != null )
		{
			Destroy( m_PlayerStats.gameObject );
		}

		if ( m_GemDetails != null )
		{
			Destroy( m_GemDetails.gameObject );
		}
	}

	public void GoHome()
	{
		GameObject.FindGameObjectWithTag( "Transition" ).GetComponent<Transition>().StartFadeOut( GoToHome );
	}

	static void GoToHome()
	{
		SceneManager.LoadScene( "MainMenu" );
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

				for ( int i = 0; i < m_DummyGems.Length; ++ i )
				{
					m_DummyGems[i].GetComponent<SpriteRenderer>().sprite = m_DummyGems[i].GetComponent<GemSpriteContainer>().m_GlowSprites[frame];
				}

				m_DummyLeaked.GetComponent<SpriteRenderer>().sprite = m_DummyGems[0].GetComponent<GemSpriteContainer>().m_StoneSprites[frame];

				if ( m_nAnimatingFrame == m_nFrameNum )
				{
					m_fAnimationIntervalTimer = 0.0f;
					m_fAnimationTimer = 0.0f;
					m_bAnimating = false;
					m_nAnimatingFrame = -1;

					for ( int i = 0; i < m_DummyGems.Length; ++ i )
					{
						m_DummyGems[i].GetComponent<SpriteRenderer>().sprite = m_DummyGems[i].GetComponent<GemSpriteContainer>().m_Sprites[0];
					}
				}
			}
		}
	}

	void AnimateCoins()
	{
		m_fCoinsTimer += Time.deltaTime;

		if ( m_fCoinsTimer < TIME_TO_ACTUAL_COINS + Time.deltaTime )
		{
			m_fCoinsTimer = m_fCoinsTimer > TIME_TO_ACTUAL_COINS ? TIME_TO_ACTUAL_COINS : m_fCoinsTimer;
			m_nShowingCoins = (int)( ( m_fCoinsTimer / TIME_TO_ACTUAL_COINS ) * ( m_CurrentCoins - m_nPrevCoins ) ) + m_nPrevCoins;
			m_Coins.GetComponent<Text>().text = m_nShowingCoins.ToString();
		}
	}

	public void GoToGacha()
	{
		if ( !m_bIsCurrentScreenScore )
		{
			m_ScoreCanvas.SetActive( false );
			m_GachaCanvas.SetActive( true );
			return;
		}

		m_bIsCurrentScreenScore = false;

		m_ScoreCanvas.SetActive( true );
		m_GachaCanvas.SetActive( true );

		PrepareAnimateScreen( m_ScoreCanvas, m_GachaCanvas );
	}

	public void GoToScore()
	{
		if ( m_bIsCurrentScreenScore )
		{
			m_ScoreCanvas.SetActive( true );
			m_GachaCanvas.SetActive( false );
			return;
		}

		m_bIsCurrentScreenScore = true;

		m_ScoreCanvas.SetActive( true );
		m_GachaCanvas.SetActive( true );

		PrepareAnimateScreen( m_GachaCanvas, m_ScoreCanvas );
	}

	void PrepareAnimateScreen( GameObject previousScreen, GameObject currentScreen )
	{
		m_bScreenAnimate = true;
		if ( m_bScreenAnimate )
		{
			m_fScreenAnimateTimer = 0.0f;

			// Setting screen to correct position
			Vector3 prevPos = previousScreen.GetComponent<RectTransform>().localPosition;
			prevPos.x = 0.0f;
			previousScreen.GetComponent<RectTransform>().localPosition = prevPos;

			Vector3 currentPos = currentScreen.GetComponent<RectTransform>().localPosition;
			currentPos.x = m_bIsCurrentScreenScore ? -m_fScreenWidth : m_fScreenWidth;
			currentScreen.GetComponent<RectTransform>().localPosition = currentPos;

			m_fScreenFrom = currentPos.x;
		}
	}

	void AnimationScreen()
	{
		if ( m_bScreenAnimate )
		{
			GameObject previousScreen = m_bIsCurrentScreenScore ? m_GachaCanvas : m_ScoreCanvas;
			GameObject currentScreen = m_bIsCurrentScreenScore ? m_ScoreCanvas : m_GachaCanvas;

			m_fScreenAnimateTimer += Time.deltaTime;

			float factor = Mathf.Pow( Mathf.Clamp( m_fScreenAnimateTimer / SCREEN_ANIMATE_TIME, 0.0f, 1.0f ), 2.0f );
			Vector3 prevPos = previousScreen.GetComponent<RectTransform>().localPosition;
			prevPos.x = Mathf.Lerp( 0.0f, -m_fScreenFrom, factor );
			previousScreen.GetComponent<RectTransform>().localPosition = prevPos;

			Vector3 currentPos = currentScreen.GetComponent<RectTransform>().localPosition;
			currentPos.x = Mathf.Lerp( m_fScreenFrom, 0.0f, factor );
			currentScreen.GetComponent<RectTransform>().localPosition = currentPos;

			m_bScreenAnimate = m_fScreenAnimateTimer < SCREEN_ANIMATE_TIME;

			if ( !m_bScreenAnimate )
			{
				previousScreen.SetActive( false );
			}
		}
	}
}
