using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
	PlayerStatistics m_PlayerStats;
	GemDetails m_GemDetails;

	public GameObject m_Score;
	public GameObject[] m_Counters;
	public GameObject m_LeakedCounter;
	public GameObject m_ComboCounter;

	public GameObject[] m_DummyGems;
	public GameObject m_DummyLeaked;

	public GameObject m_Coins;

	// Animation variables
	private int m_nFrameNum;
	private float m_fAnimationIntervalTimer = 0.0f;
	private float m_fAnimationTimer = 0.0f;
	private bool m_bAnimating = false;
	private int m_nAnimatingFrame = -1;

	// Use this for initialization
	void Start ()
	{
		// Getting stats
		m_PlayerStats = GameObject.FindGameObjectWithTag( "Player Statistics" ).GetComponent<PlayerStatistics>();
		m_Score.GetComponent<Text>().text = m_PlayerStats.m_nScore.ToString();

		for ( int i = 0; i < m_PlayerStats.m_aDestroyCount.Length; ++i )
		{
			m_Counters[i].GetComponent<Text>().text = m_PlayerStats.m_aDestroyCount[i].ToString();
		}

		m_LeakedCounter.GetComponent<Text>().text = m_PlayerStats.m_nLeakCount.ToString();
		m_ComboCounter.GetComponent<Text>().text = m_PlayerStats.m_nMaxCombo.ToString();

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

		m_GemDetails = GameObject.FindGameObjectWithTag( "Gem Details" ).GetComponent<GemDetails>();

		GameData.Instance.m_Coin += m_PlayerStats.m_nCoinsGain;

		Text coinsText = m_Coins.GetComponent<Text>();
		coinsText.text = GameData.Instance.m_Coin.ToString() + " (+" + m_PlayerStats.m_nCoinsGain + ")";

		// Save
		SaveLoad.Save();
	}
	
	// Update is called once per frame
	void Update ()
	{
		AnimateGems();
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
}
