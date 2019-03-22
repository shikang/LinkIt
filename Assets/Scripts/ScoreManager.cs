using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;

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
	public GameObject m_CoinsThisRound;

	public GameObject m_ScoreCanvas;
	public GameObject m_GachaCanvas;
	public GameObject m_HighScoreText;
    public GameObject m_PrizeButton;

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

	// COunt up Stats
	private float m_CountUp_Timer = 0.0f;
	private int m_CountUp_Score;
	private int m_CountUp_Gold;
	private int [] m_CountUp_Gems;
	private int m_CountUp_GemRed;
	private int m_CountUp_GemBlue;
	private int m_CountUp_GemGreen;
	private int m_CountUp_GemYellow;
	private int m_CountUp_GemGrey;
	private int m_CountUp_Combo;
	private int m_CountUp_Interval;

	private int m_GoldEarned;

	// Use this for initialization
	void Start ()
	{
		// Getting stats
		m_PlayerStats = GameObject.FindGameObjectWithTag( "Player Statistics" ).GetComponent<PlayerStatistics>();
		m_Score.GetComponent<Text> ().text = m_CountUp_Score.ToString ();

		m_CountUp_Gems = new int[4];

		int gemDestroyed = 0;
		for ( int i = 0; i < m_PlayerStats.m_aDestroyCount.Length; ++i )
		{
			m_CountUp_Gems[i] = 0;
			gemDestroyed += m_PlayerStats.m_aDestroyCount[i];
		}

		// Find the score without boosters, to use for gold calc
		int realScore = (int)Mathf.Round(m_PlayerStats.m_nScore / BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.ScoreMult) / BoosterManager.Instance.GetScoreMultOnce());
		m_GoldEarned = (int)Mathf.Round(realScore / 100 * BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.GoldMult) * BoosterManager.Instance.GetGoldMultOnce());

		m_CoinsThisRound.GetComponent<Text>().text = m_CountUp_Gold.ToString();
		m_LeakedCounter.GetComponent<Text>().text = m_CountUp_GemGrey.ToString();
		m_ComboCounter.GetComponent<Text> ().text = m_CountUp_Combo.ToString ();

		m_HighScoreText.SetActive(false);
		if ( GameData.Instance.m_HighScore < m_PlayerStats.m_nScore )
		{
			GameData.Instance.m_HighScore = m_PlayerStats.m_nScore;
			m_HighScoreText.SetActive(true);
		}

        if ( GameData.Instance.m_HighestCombo < m_PlayerStats.m_nMaxCombo )
		{
			GameData.Instance.m_HighestCombo = m_PlayerStats.m_nMaxCombo;
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

		if(m_PlayerStats.m_nScore > 0)
		{
			AchievementManager.Instance.AddGamesPlayed();
			AchievementManager.Instance.AddCoinsEarned(m_GoldEarned);
			AchievementManager.Instance.AddScoreEarned(m_PlayerStats.m_nScore);
			AchievementManager.Instance.AddLinkedGems(1, m_PlayerStats.m_aDestroyCount[0]);
			AchievementManager.Instance.AddLinkedGems(2, m_PlayerStats.m_aDestroyCount[1]);
			AchievementManager.Instance.AddLinkedGems(3, m_PlayerStats.m_aDestroyCount[2]);
			AchievementManager.Instance.AddLinkedGems(4, m_PlayerStats.m_aDestroyCount[3]);

			AchievementManager.Instance.AddCombo(m_PlayerStats.m_nMaxCombo);
			AchievementManager.Instance.AddCoinsEarned_PerGame(m_GoldEarned);
			AchievementManager.Instance.AddScoreEarned_PerGame(m_PlayerStats.m_nScore);
		}

		Analytics.CustomEvent("Scorecard", new Dictionary<string, object>
		{
			{ "Score", m_PlayerStats.m_nScore},
			{ "RedGem", m_PlayerStats.m_aDestroyCount[0] },
			{ "BlueGem", m_PlayerStats.m_aDestroyCount[1] },
			{ "GreenGem", m_PlayerStats.m_aDestroyCount[2] },
			{ "YellowGem", m_PlayerStats.m_aDestroyCount[3] },
			{ "GreyGem", m_PlayerStats.m_nLeakCount.ToString() },
			{ "Combo", m_PlayerStats.m_nMaxCombo },
			{ "GoldEarned", m_GoldEarned }
		});

#if LINKIT_COOP
		if( NetworkManager.IsConnected() )
		{
			NetworkManager.Disconnect();
		}
#endif   // LINKIT_COOP

		GameData.Instance.m_Coin += m_PlayerStats.m_nCoinsGain;
		m_CurrentCoins = GameData.Instance.m_Coin;
		m_nShowingCoins = GameData.Instance.m_Coin;
		m_nPrevCoins = GameData.Instance.m_Coin;
		m_fCoinsTimer = 0.0f;
		GameData.Instance.m_Coin += m_GoldEarned;

		Text coinsText = m_Coins.GetComponent<Text>();
		coinsText.text = m_nShowingCoins.ToString();// + " (+" + m_PlayerStats.m_nCoinsGain + ")";

#if UNITY_ANDROID
		GooglePlayService.PostHighScore( m_PlayerStats.m_nScore );
#elif UNITY_IOS
		GameCenterService.PostHighScore( m_PlayerStats.m_nScore );
#endif

        AchievementManager.Instance.BoosterGamesPlayed ();
		AchievementManager.Instance.BoosterPointsEarned (m_PlayerStats.m_nScore);

		// Save
		//SaveLoad.Save();
		SaveDataLoader.SaveGame();

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

		m_CountUp_Interval++;
		if( m_CountUp_Interval % 3 == 0)
			CountUpStats();

		if (Input.GetMouseButtonDown (0))
			SkipCountUp ();
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
		BoosterManager.Instance.ResetBoosterOnce();
		Adverts.Instance.RandomShowAd();
		GameObject.FindGameObjectWithTag( "Transition" ).GetComponent<Transition>().StartFadeOut( GoToHome );
	}

    public void GoDoubleCoin()
    {
        UnityEngine.Advertisements.ShowOptions so = new UnityEngine.Advertisements.ShowOptions();
        so.resultCallback = ((result) => {
            m_PrizeButton.SetActive( false );

            if (result.Equals(UnityEngine.Advertisements.ShowResult.Finished))
            {
                GameData.Instance.m_Coin += m_GoldEarned;
                SaveDataLoader.SaveGame();

                m_CoinsThisRound.GetComponent<Text>().text = (m_GoldEarned * 2).ToString();
            }
        });
        Adverts.Instance.ShowAd(AdVidType.video, so);
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

				int frame = (m_nAnimatingFrame + GemSpawner.ANIMATION_FRAME_OFFSET) % m_nFrameNum;

				for ( int i = 0; i < m_DummyGems.Length; ++ i )
				{
					if(frame < m_DummyGems[i].GetComponent<GemSpriteContainer>().m_GlowSprites.Length)
					{
						m_DummyGems[i].GetComponent<SpriteRenderer>().sprite = m_DummyGems[i].GetComponent<GemSpriteContainer>().m_GlowSprites[frame];
					}
				}

				if(frame < m_DummyGems[0].GetComponent<GemSpriteContainer>().m_StoneSprites.Length)
				{
					m_DummyLeaked.GetComponent<SpriteRenderer>().sprite = m_DummyGems[0].GetComponent<GemSpriteContainer>().m_StoneSprites[frame];
				}

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

	void CountUpStats()
	{
		if(m_CountUp_Timer < 2.0f)
			m_CountUp_Timer += Time.deltaTime;

		if(m_CountUp_Timer > 0.2f && m_CountUp_Score < m_PlayerStats.m_nScore)
		{
			m_CountUp_Score += System.Math.Max((int)(m_PlayerStats.m_nScore * Time.deltaTime), 200);

			if (m_CountUp_Score > m_PlayerStats.m_nScore)
				m_CountUp_Score = m_PlayerStats.m_nScore;

			m_Score.GetComponent<Text>().text = m_CountUp_Score.ToString ();
		}

		if(m_CountUp_Timer > 0.4f && m_CountUp_Gems[0] < m_PlayerStats.m_aDestroyCount[0])
		{
			m_CountUp_Gems[0] += System.Math.Max((int)(m_PlayerStats.m_aDestroyCount[0] * Time.deltaTime), 1);

            if (m_CountUp_Gems[0] > m_PlayerStats.m_aDestroyCount[0])
                m_CountUp_Gems[0] = m_PlayerStats.m_aDestroyCount[0];

            m_Counters[0].GetComponent<Text>().text = m_CountUp_Gems[0].ToString ();
		}

		if(m_CountUp_Timer > 0.6f && m_CountUp_Gems[1] < m_PlayerStats.m_aDestroyCount[1])
		{
            m_CountUp_Gems[1] += System.Math.Max((int)(m_PlayerStats.m_aDestroyCount[1] * Time.deltaTime), 1);

            if (m_CountUp_Gems[1] > m_PlayerStats.m_aDestroyCount[1])
                m_CountUp_Gems[1] = m_PlayerStats.m_aDestroyCount[1];

            m_Counters[1].GetComponent<Text>().text = m_CountUp_Gems[1].ToString ();
		}

		if(m_CountUp_Timer > 0.8f && m_CountUp_Gems[2] < m_PlayerStats.m_aDestroyCount[2])
		{
            m_CountUp_Gems[2] += System.Math.Max((int)(m_PlayerStats.m_aDestroyCount[2] * Time.deltaTime), 1);

            if (m_CountUp_Gems[2] > m_PlayerStats.m_aDestroyCount[2])
                m_CountUp_Gems[2] = m_PlayerStats.m_aDestroyCount[2];

            m_Counters[2].GetComponent<Text>().text = m_CountUp_Gems[2].ToString ();
		}

		if(m_CountUp_Timer > 1.0f && m_CountUp_Gems[3] < m_PlayerStats.m_aDestroyCount[3])
		{
            m_CountUp_Gems[3] += System.Math.Max((int)(m_PlayerStats.m_aDestroyCount[3] * Time.deltaTime), 1);

            if (m_CountUp_Gems[3] > m_PlayerStats.m_aDestroyCount[3])
                m_CountUp_Gems[3] = m_PlayerStats.m_aDestroyCount[3];

            m_Counters[3].GetComponent<Text>().text = m_CountUp_Gems[3].ToString ();
		}

		if(m_CountUp_Timer > 1.2f && m_CountUp_GemGrey < m_PlayerStats.m_nLeakCount)
		{
            m_CountUp_GemGrey += System.Math.Max((int)(m_PlayerStats.m_nLeakCount * Time.deltaTime), 1);

            if (m_CountUp_GemGrey > m_PlayerStats.m_nLeakCount)
                m_CountUp_GemGrey = m_PlayerStats.m_nLeakCount;
            
			m_LeakedCounter.GetComponent<Text>().text = m_CountUp_GemGrey.ToString ();
		}

		if(m_CountUp_Timer > 1.4f && m_CountUp_Combo < m_PlayerStats.m_nMaxCombo)
		{
            m_CountUp_Combo += System.Math.Max((int)(m_PlayerStats.m_nMaxCombo * Time.deltaTime), 1);

            if (m_CountUp_Combo > m_PlayerStats.m_nMaxCombo)
                m_CountUp_Combo = m_PlayerStats.m_nMaxCombo;

			m_ComboCounter.GetComponent<Text>().text = m_CountUp_Combo.ToString ();
		}

		if(m_CountUp_Timer > 1.6f && m_CountUp_Gold < m_GoldEarned)
		{
            m_CountUp_Gold += System.Math.Max((int)(m_GoldEarned * Time.deltaTime), 20);

			if (m_CountUp_Gold > m_GoldEarned)
				m_CountUp_Gold = m_GoldEarned;

			m_CoinsThisRound.GetComponent<Text>().text = m_CountUp_Gold.ToString ();
		}
	}

	void SkipCountUp()
	{
		m_CountUp_Score = m_PlayerStats.m_nScore;
		m_Score.GetComponent<Text>().text = m_CountUp_Score.ToString ();

		for(int i = 0; i < 4; ++i)
		{
			m_CountUp_Gems [i] = m_PlayerStats.m_aDestroyCount [i];
			m_Counters[i].GetComponent<Text>().text = m_CountUp_Gems[i].ToString ();
		}

		m_CountUp_GemGrey = m_PlayerStats.m_nLeakCount;
		m_LeakedCounter.GetComponent<Text>().text = m_CountUp_GemGrey.ToString ();

		m_CountUp_Combo = m_PlayerStats.m_nMaxCombo;
		m_ComboCounter.GetComponent<Text>().text = m_CountUp_Combo.ToString ();

		m_CountUp_Gold = m_GoldEarned;
		m_CoinsThisRound.GetComponent<Text>().text = m_CountUp_Gold.ToString ();
	}
}
