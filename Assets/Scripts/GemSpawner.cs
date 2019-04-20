//#define USE_SINGLE_REPEL
//#define DEBUG_SPAWN

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Analytics;

public class GemSpawner : MonoBehaviour
{
	public static Vector3 FRONT_OFFSET = Vector3.back;

	// Spawning constants
	public const float GAME_START_DELAY = 2.0f;			//!< In seconds
	public const int LANE_NUM = 5;
	public const int LOOKBACK_NUM = 3;
	public const float BASE_SPAWN_RATE = 1.0f;			//!< In seconds
	public const float BASE_GEM_DROP_TIME = 5.0f;       //!< In seconds
	public const float SPAWN_RATE_GROWTH = 0.06f;       //!< In seconds
	public const float GEM_DROP_TIME_GROWTH = 0.125f;   //!< In seconds
	public const int GEM_LOOKBACK_NUM = 1;
	public const float SPAWN_DANGER_AREA = 0.6f;        //!< Percentage from bottom

	// Spawning gold contants
	public const float GOLD_SPAWN_INTERVAL = 30.0f;		//!< In seconds
	public const float GOLD_SPAWN_CHANCE = 0.08f;       //!< Percentage (Over 1.0f)
	public const int GOLD_DROP_AMOUNT = 250;

	// Type constants
	public const int INVALID_LANE = -1;
	public const int INVALID_GEM = -1;

	// Animation constants
	public const float ANIMATION_INTERVAL = 2.0f;		//!< In seconds
	public const float ANIMATION_RATE = 0.125f;         //!< In seconds
    public const int ANIMATION_FRAME_OFFSET = 0;        //!< In seconds

    public const float TIME_TO_ACTUAL_POINTS = 0.5f;    //!< In seconds
	public const float TIME_TO_ACTUAL_COMBO = 0.25f;    //!< In seconds
	public const float TIME_TO_COMBO_FADE = 2.0f;       //!< In seconds
	public const float TIME_TO_PRAISE_FADE = 2.0f;      //!< In seconds

	public const float PRAISE_START_Y = 218.0f;
	public const float PRAISE_MOVE_DISTANCE = 0.4f;

	public const float COMBO_FADE_TIME_RECIPROCAL = 1.0f / TIME_TO_COMBO_FADE;

	public const float GAMEOVER_ANIMATION = 2.0f;       //!< In seconds

	public const float HEALTH_LOW_OVERLAY_FADE_TIME = 0.5f;  //!< In seconds
	public const float HIGH_COMBO_OVERLAY_FADE_TIME = 0.5f;  //!< In seconds
	public const float HIGH_COMBO_SPECULAR_INTERVAL = 10.0f;  //!< In seconds

	// Game constants
	public const float UNLINKABLE_ZONE = 0.1f;          //!< Percentage from bottom

	public const float LINKED_SCALE_FACTOR = 1.25f;

	public const int PER_GEM_POINTS = 100;

	public const int MAX_LEVEL = ( ( int )SpawnPattern.DifficultyLevel.Total - 1 ) * 3;

	public const int HEALTH_LOST_PER_GEM = 10;
	public const int HEALTH_GAIN_PER_LINK = 1;
	public const int MAX_HEALTH = 100;
	public const int LOW_HEALTH = (int)( 0.3f * MAX_HEALTH );

	public const int HIGH_COMBO = 20;
	public readonly int[] COMBO_MULTIPLIER_ARRAY = { HIGH_COMBO, 3 * HIGH_COMBO, 8 * HIGH_COMBO };

	public readonly string[] PRAISE_ARRAY = { "", "", "", "Good", "Great", "Incredible!", "Awesome!" };

	bool m_bGameStart = false;
	bool isPraiseUp = false;

	// Link
	public GameObject m_LinkPrefab;
	private GameObject m_LinkObject;
	public Color[] m_LinkColours;
	public Color m_NeutralColor;
	public Color m_GoldColor;
	private Link m_Link;
	private List<Gem> m_LinkedGem;
    public GameObject[] m_AfterLink;

    // Spawning information
    public GameObject[] m_aGemList;                     //!< Prefab to create from
#if USE_SINGLE_REPEL
	public GameObject m_Repel;                          //!< Prefab for repel
#else
	public GameObject[] m_aRepels;                      //!< Prefab for repel
#endif
	private int m_nGemTypeNum = 0;						//!< Number of gem types
	private Vector3 m_HalfDimension;
	private float m_fLaneWidth;
	private float m_fBaseGemDropSpeed;
	private Vector3 m_DefaultGemScale;
	private Vector3 m_LinkedGemScale;
    private Vector3 m_DefaultGoldScale;
    private Vector3 m_LinkedGoldScale;

    // Spawning gold info
    public GameObject m_GoldPrefab;
	private float m_GoldIntervalTimer;
	private GameObject m_GoldObject;

	// Spawning history variables
	private List<int> m_PreviousLanes;
	private int m_nLastInsertedLaneIndex = -1;
	//private List<int> m_PreviousGems;
	private SpawnPattern m_SpawnPattern;
	private List<int> m_lSequence;
	private List<int> m_mSequenceGemTypeMap;
	private int m_nSequenceIndex;
	private int m_nSequenceIndexFromList;
	private List<int> m_lFailedSequenceCount;
	private float m_SpawnDangerArea = 0.0f;

	// Spawning interval
	private float m_fSpawnRate = BASE_SPAWN_RATE;
	private float m_fSpawnTimer = 0.0f;

	// Spawning live information (updating...)
	private int[] m_aGemCount;											//!< Keep track of gem count sorted by color
	private int m_nTotalGemCount = 0;
	private List< List<GameObject> > m_Gems;							//!< Gems in game
	private List<GameObject> m_StonedGems;								//!< Unlinkable gems
	private List<GameObject> m_GemsToBeRemoved;							//!< Gems to be removed after a loop
	private List<GameObject> m_GemsToBeDestroyed;                       //!< Gems to be destroyed after a loop
#if LINKIT_COOP
	private List<GameObject> m_NetworkGemsToBeDestroyed;                //!< Delayed destroy because the other player could be linking it
#endif	// LINKIT_COOP

	// Gem sprites
	private GemDetails m_GemDetails;

	// Life line variables
	public Color[] m_LineLineColors;
	public GameObject m_LineLine;

	// Animation variables
	private int m_nFrameNum;
	private float m_fAnimationIntervalTimer = 0.0f;
	private float m_fAnimationTimer = 0.0f;
	private bool m_bAnimating = false;
	private int m_nAnimatingFrame = -1;

	// Particle effect
	public GameObject m_GemExplosionPrefab;
	public GameObject m_LevelUpOverlayPrefab;

	// Overlays
	public GameObject m_HealthLowOverlay;
	private float m_HealthLowTimer = 0.0f;
	public GameObject m_HighComboZone;
	private float m_HighComboZoneTimer = 0.0f;
	public GameObject m_HighComboEffectLeft;
	public GameObject m_HighComboEffectRight;
    public GameObject m_HighComboStripEffectLeft;
    public GameObject m_HighComboStripEffectRight;

    public GameObject m_HighComboStrip;
	private Vector3 m_HighComboStripPos;
	private Vector3 m_HighComboStripScale;
	public GameObject m_HighComboSpecular;
	private Vector3 m_HighComboSpecularPos;
	private Vector3 m_HighComboSpecularScale;
	private GameObject m_CurrentHighComboStrip;
	private float m_HighComboStripSpecularTimer;
	private float m_HighComboEffectStartSpeed;
    private float m_HighComboStripEffectStartSpeed;

    // Player stats
    private int m_nLevel = 0;
	private int m_nPoints = 0;
	private int m_nShowingPoints = 0;
	private int m_nPrevPoints = 0;
	private float m_fPointTimer = 0.0f;
	private int m_nHealth = (int)(MAX_HEALTH * BoosterManager.Instance.GetMoreHealthOnce());
	private PlayerStatistics m_PlayerStats;
	private bool m_bGameover = false;
	private int m_nCurrentCombo = 0;
	private int m_nMaxCombo = 0;
	private int m_nShowingCombo = 0;
	private int m_nPrevCombo = 0;
	private float m_fComboTimer = 0.0f;
	//private float m_nComboOpacity = 0.0f;
	private float m_fPraiseTimer = 0.0f;
	private Vector3 m_PraisePos;
	private float m_fShowMultiplierTimer = 0.0f;
	private Vector3 m_ShowMultiplierPos;
	private int m_nHighComboMultiplierIndex = 0;

	private bool m_bIsBreakingCombo;
	private bool m_bIsStartingCombo;
	private bool m_bScoreText_IsAnim = true;
	private bool m_bComboText_IsAnim = true;

	// Gameover
	private float m_fGameoverTimer = 0.0f;

	// Texts
	public GameObject m_GameCanvas;
	public GameObject m_ScoreText;
	public GameObject m_LevelText;		//!< Debug
	public GameObject m_HealthText;     //!< Debug
	public GameObject m_ComboText;
	public GameObject m_PraiseText;
	public GameObject m_MultiplierText;
#if LINKIT_COOP
	public GameObject m_DisconnectText;
#endif	// LINKIT_COOP
	public GameObject m_PointsGain;

	public bool m_bIsPaused;

#if LINKIT_COOP
	// Network
	public GameObject m_NetworkGameTimerPrefab;
	NetworkGameTime m_NetworkGameTimer = null;
	NetworkGameLogic m_Network = null;
	bool m_bOriginalPlayerOne = true;
	Queue<int> m_UnusedGemIDList;
	int m_NextGemID = 0;
#endif  // LINKIT_COOP

	// Use this for initialization
	void Start ()
	{
		m_bGameStart = false;
		m_bIsPaused = false;

		m_ComboText.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

		// Booster check for analytics
		Analytics.CustomEvent("UsedBoostersLvl", new Dictionary<string, object>
		{
			{"ScoreMult_Once", GameData.Instance.m_Boost_ScoreMultOnce},
			{"GoldMult_Once", GameData.Instance.m_Boost_GoldMultOnce},
			{"MoreHealthOnce", GameData.Instance.m_Boost_MoreHealthOnce},
			{"ScoreMult", GameData.Instance.m_Boost_ScoreMult},
			{"GoldMult", GameData.Instance.m_Boost_GoldMult},
			{"Shield", GameData.Instance.m_Boost_Shield},
			{"SlowerGems", GameData.Instance.m_Boost_SlowerGems},
			{"BiggerGems", GameData.Instance.m_Boost_BiggerGems}
		});
				
		// Initialise link
		CreateLink();
		m_Link = m_LinkObject.GetComponent<Link>();
		m_LinkedGem = new List<Gem>();

		// Get spawning information
		m_HalfDimension = Camera.main.ScreenToWorldPoint( new Vector3( ( Screen.width ), ( Screen.height ) ) );
		m_fLaneWidth = m_HalfDimension.x * 2.0f / LANE_NUM;
		//m_fBaseGemDropSpeed = m_HalfDimension.y * 2.0f / BASE_GEM_DROP_TIME;

		// Gem details
		GameObject gemDetailsObject = GameObject.FindGameObjectWithTag( "Gem Details" );
		if ( gemDetailsObject != null )
		{
			m_GemDetails = gemDetailsObject.GetComponent<GemDetails>();
			if (m_GemDetails != null)
			{
				Debug.Log("Reset explosion!");
				m_GemExplosionPrefab = m_GemDetails.m_GemSet.m_Explosion;
			}
		}

		//m_nGemTypeNum = m_aGemList.Length;
		m_nGemTypeNum = GemContainerSet.GEM_SET_NUM;

		// Initialise spawning history variables
		m_PreviousLanes = Enumerable.Repeat( INVALID_LANE, LOOKBACK_NUM ).ToList();
		//m_PreviousGems = new List<int>();
		m_SpawnPattern = GetComponent<SpawnPattern>();
		m_lSequence = new List<int>();
		m_mSequenceGemTypeMap = new List<int>();
		m_nSequenceIndex = 0;
		m_nSequenceIndexFromList = -1;
		m_SpawnDangerArea = ( SPAWN_DANGER_AREA * m_HalfDimension.y * 2.0f ) + -m_HalfDimension.y;

		m_nLastInsertedLaneIndex = -1;

		// Initialising spawning timer
		m_fSpawnTimer = 0.0f;
		//m_fSpawnRate = BASE_SPAWN_RATE;

		// Initialising spawning live information
		m_aGemCount = Enumerable.Repeat( 0, m_nGemTypeNum ).ToArray();
		m_nTotalGemCount = 0;
		m_Gems = new List< List< GameObject > >();
		for ( int i = 0; i < LANE_NUM; ++i )
		{
			m_Gems.Add( new List< GameObject >() );
		}
		m_StonedGems = new List<GameObject>();
		m_GemsToBeRemoved = new List<GameObject>();
		m_GemsToBeDestroyed = new List<GameObject>();
#if LINKIT_COOP
		if ( NetworkManager.IsConnected() )
			m_NetworkGemsToBeDestroyed = new List<GameObject>();
#endif  // LINKIT_COOP

		// Initialise gold drop
		m_GoldIntervalTimer = 0.0f;
		m_GoldObject = null;

		// Initialise life line
		if ( m_LineLine != null )
		{
			m_LineLine.transform.position = new Vector3( 0.0f, ( UNLINKABLE_ZONE * m_HalfDimension.y * 2.0f ) + -m_HalfDimension.y, 0.0f ) + FRONT_OFFSET;
		}

		// Initialising animation timer
		/*
		m_nFrameNum = m_aGemList[0].GetComponent<GemSpriteContainer>().m_Sprites.Length;
		for ( int i = 1; i < m_aGemList.Length; ++i )
		{
			int num = m_aGemList[i].GetComponent<GemSpriteContainer>().m_Sprites.Length;
			m_nFrameNum = m_nFrameNum > num ? num : m_nFrameNum;
		}
		*/
		if ( m_GemDetails != null )
		{
			m_nFrameNum = m_GemDetails.GetComponent< GemDetails >().m_GemSet.GetGemContainer( 0 ).Length;
			for (int i = 1; i < GemContainerSet.GEM_SET_NUM; ++i)
			{
				int num = m_GemDetails.GetComponent< GemDetails >().m_GemSet.GetGemContainer( i ).Length;
				m_nFrameNum = m_nFrameNum > num ? num : m_nFrameNum;
			}
		}
		else
		{
			m_nFrameNum = m_aGemList[0].GetComponent<GemSpriteContainer>().m_Sprites.Length;
			for ( int i = 1; i < m_aGemList.Length; ++i )
			{
				int num = m_aGemList[i].GetComponent<GemSpriteContainer>().m_Sprites.Length;
				m_nFrameNum = m_nFrameNum > num ? num : m_nFrameNum;
			}
		}

		m_DefaultGemScale = m_aGemList[0].transform.localScale * BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.BiggerGems);
        if ( ( (float)Screen.width / (float)Screen.height ) < ( 9.0f / 16.0f ) )
        {
            m_DefaultGemScale *= ( (float)Screen.width / (float)Screen.height ) / ( 9.0f / 16.0f );
        }
		m_LinkedGemScale = LINKED_SCALE_FACTOR * m_DefaultGemScale;

        m_DefaultGoldScale = m_GoldPrefab.transform.localScale * BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.BiggerGems);
        if ( ( (float)Screen.width / (float)Screen.height ) < ( 9.0f / 16.0f ) )
        {
            m_DefaultGoldScale *= ( (float)Screen.width / (float)Screen.height ) / ( 9.0f / 16.0f );
        }
        m_LinkedGoldScale = LINKED_SCALE_FACTOR * m_DefaultGoldScale;

        m_fAnimationIntervalTimer = 0.0f;
		m_fAnimationTimer = 0.0f;
		m_bAnimating = false;
		m_nAnimatingFrame = -1;

		// Overlays
		{
			SpriteRenderer sr = m_HealthLowOverlay.GetComponent<SpriteRenderer>();
			Color c = sr.color;
			c.a = 0.0f;
			sr.color = c;

			m_HealthLowOverlay.SetActive( true );
			m_HealthLowTimer = 0.0f;
		}
		{
			SpriteRenderer sr = m_HighComboZone.GetComponent<SpriteRenderer>();
			Color c = sr.color;
			c.a = 0.0f;
			sr.color = c;

			m_HighComboZone.SetActive( true );
			m_HighComboZoneTimer = 0.0f;

			m_HighComboStripPos = m_HighComboStrip.transform.localPosition;
			m_HighComboSpecularPos = m_HighComboSpecular.transform.localPosition;

			m_HighComboStripScale = m_HighComboStrip.transform.localScale;
			m_HighComboSpecularScale = m_HighComboSpecular.transform.localScale;

			m_CurrentHighComboStrip = null;
			m_HighComboStripSpecularTimer = 0.0f;

			m_HighComboEffectLeft.GetComponent<ParticleSystem>().Stop();
			m_HighComboEffectRight.GetComponent<ParticleSystem>().Stop();
            m_HighComboStripEffectLeft.GetComponent<ParticleSystem>().Stop();
            m_HighComboStripEffectRight.GetComponent<ParticleSystem>().Stop();

            if ( ( (float)Screen.width / (float)Screen.height ) > ( 9.0f / 16.0f ) )
            {
                Vector3 position = m_HighComboStripEffectLeft.transform.position;
                position.x = ( ( (float)Screen.width / (float)Screen.height ) / ( 9.0f / 16.0f ) ) * position.x;
                m_HighComboStripEffectLeft.transform.position = position;
            }

            if ( ( (float)Screen.width / (float)Screen.height ) > ( 9.0f / 16.0f ) )
            {
                Vector3 position = m_HighComboStripEffectRight.transform.position;
                position.x = ( ( (float)Screen.width / (float)Screen.height ) / ( 9.0f / 16.0f ) ) * position.x;
                m_HighComboStripEffectRight.transform.position = position;
            }

            m_HighComboEffectStartSpeed = m_HighComboEffectLeft.GetComponent<ParticleSystem>().startSpeed;
            m_HighComboStripEffectStartSpeed = m_HighComboStripEffectLeft.GetComponent<ParticleSystem>().startSpeed;
        }

		// Initialise player's stats
		m_nLevel = 0;
		m_nPoints = 0;
		m_nShowingPoints = 0;
		m_nPrevPoints = 0;
		m_fPointTimer = 0.0f;
		m_nHealth = (int)(MAX_HEALTH * BoosterManager.Instance.GetMoreHealthOnce());
		m_nCurrentCombo = 0;
		m_nMaxCombo = 0;
		m_nShowingCombo = 0;
		m_nPrevCombo = 0;
		m_fComboTimer = 0.0f;
		m_fPraiseTimer = 0.0f;
		m_PraisePos = m_PraiseText.transform.position;
		m_fShowMultiplierTimer = 0.0f;
		m_ShowMultiplierPos = m_MultiplierText.transform.position;
		m_fSpawnRate = BASE_SPAWN_RATE - m_nLevel * SPAWN_RATE_GROWTH;
        m_fSpawnRate *= 1.0f / BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.SlowerGems);
        m_fBaseGemDropSpeed = m_HalfDimension.y * 2.0f / ( BASE_GEM_DROP_TIME - ( m_nLevel / 2 ) * GEM_DROP_TIME_GROWTH );
		m_fBaseGemDropSpeed *= BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.SlowerGems);

		m_nHighComboMultiplierIndex = 0;

		m_PlayerStats = GameObject.FindGameObjectWithTag( "Player Statistics" ).GetComponent<PlayerStatistics>();

		m_PlayerStats.m_aGems = m_aGemList;
		m_PlayerStats.m_aDestroyCount = new int [m_aGemList.Length];
		for (int i = 0; i < m_aGemList.Length; ++i)
		{
			m_PlayerStats.m_aDestroyCount[i] = 0;
		}

		// Texts and display
		m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
		m_ComboText.GetComponent<Text>().text = "Combo\n" + m_nCurrentCombo.ToString();
		{
			Color c = m_ComboText.GetComponent<Text>().color;
			c.a = 0.0f;
			m_ComboText.GetComponent<Text>().color = c;
		}
		{
			m_PraiseText.GetComponent<Text>().text = "";

			Color c = m_PraiseText.GetComponent<Text>().color;
			c.a = 0.0f;
			m_PraiseText.GetComponent<Text>().color = c;
		}
		{
			m_MultiplierText.GetComponent<Text>().text = "";

			Color c = m_MultiplierText.GetComponent<Text>().color;
			c.a = 0.0f;
			m_MultiplierText.GetComponent<Text>().color = c;
		}
		m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();

		// Gameover
		m_bGameover = false;
		m_fGameoverTimer = 0.0f;

		// Transition Overlay
		GameObject transition = GameObject.FindGameObjectWithTag( "Transition" );
		transition.transform.position += 5.0f * FRONT_OFFSET;

#if LINKIT_COOP
		if ( NetworkManager.IsConnected() )
		{
			m_Network = GameObject.Find( "Network Manager" ).GetComponent<NetworkGameLogic>();
			m_bOriginalPlayerOne = NetworkManager.IsPlayerOne();

			if ( NetworkManager.IsPlayerOne() )
			{
				GameObject timer = PhotonNetwork.Instantiate( m_NetworkGameTimerPrefab.name, Vector3.zero, Quaternion.identity, 0 );
				m_NetworkGameTimer = timer.GetComponent<NetworkGameTime>();

				m_UnusedGemIDList = new Queue<int>();
				m_NextGemID = 1;
			}
		}
		m_DisconnectText.SetActive( false );
#endif	// LINKIT_COOP

		// @debug
		/*
		{
			m_nLevel = MAX_LEVEL;
			m_LevelText.GetComponent<Text>().text = m_nLevel.ToString();

			//m_fSpawnRate -= SPAWN_RATE_GROWTH;
			m_fSpawnRate = BASE_SPAWN_RATE - m_nLevel * SPAWN_RATE_GROWTH;
			m_fBaseGemDropSpeed = m_HalfDimension.y * 2.0f / ( BASE_GEM_DROP_TIME - ( m_nLevel / 2 ) * GEM_DROP_TIME_GROWTH );

			Debug.Log( "Spawn rate: " + m_fSpawnRate );
			Debug.Log( "Drop rate: " + m_fBaseGemDropSpeed );
		}
		*/
	}

	public void PrepareFailList()
	{
		m_lFailedSequenceCount = new List<int>();
		m_SpawnPattern = GetComponent<SpawnPattern>();
		for ( int i = 0; i < m_SpawnPattern.GetSpawnSeqencesNum(); ++i )
		{
			m_lFailedSequenceCount.Add( 0 );
		}
	}

	void OnApplicationQuit()
	{
		Debug.Log( "---- Statistics ----" );
		Debug.Log( "Level: " + m_nLevel );
		for ( int i = 0; i < m_lFailedSequenceCount.Count; ++i )
		{
			Debug.Log( "Sequence [" + i + "]: " + m_lFailedSequenceCount[i] );
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (m_bIsPaused)
			return;
		
		Spawn();

		if ( Input.GetMouseButtonDown( 0 ) )
			m_Link.StartLink();

		if ( m_Link.Linking )
			m_Link.DrawLink();

		UnlinkGems();
		UpdateGems();
		UpdateGoldDrop();
		AnimateGems();
		AnimatePoints();
		AnimateCombo();
		AnimatePraise();
		AnimateShowMultiplier();
		AnimateOverlays();

		CheckGameOver();
		UpdateGameover();

		if(m_bIsBreakingCombo)
		{
			Color c = m_ComboText.GetComponent<Text>().color;
			c.a -= Time.deltaTime * 5.0f;
			m_ComboText.GetComponent<Text>().color = c;

			m_ComboText.transform.localScale *= 0.98f;

			if (c.a <= 0.05f)
				m_bIsBreakingCombo = false;
		}

		if(m_bIsStartingCombo)
		{
			Color c = m_ComboText.GetComponent<Text>().color;
			c.a += Time.deltaTime * 5.0f;
			m_ComboText.GetComponent<Text>().color = c;

			m_ComboText.transform.localScale *= 1.02f;

			if (c.a >= 0.6f)
			{
				m_bIsStartingCombo = false;
				m_ComboText.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
			}
		}
	}

	// -------------------------------- Updating functions --------------------------------------------
	void Spawn()
	{
#if LINKIT_COOP
		if ( NetworkManager.IsConnected() && !NetworkManager.IsPlayerOne() )
			return;
#endif	// LINKIT_COOP

		m_fSpawnTimer += Time.deltaTime;

		if ( !m_bGameStart )
		{
			GameObject tutorialManager = GameObject.FindGameObjectWithTag( "Tutorial Manager" );
			if ( tutorialManager != null && tutorialManager.GetActive() )
			{
				m_fSpawnTimer = 0.0f;
			}

			if ( m_fSpawnTimer >= GAME_START_DELAY )
			{
				m_fSpawnTimer -= GAME_START_DELAY;
				m_bGameStart = true;
			}
		}
		else if ( m_fSpawnTimer >= m_fSpawnRate )
		{
			m_fSpawnTimer -= m_fSpawnRate;

			// Spawning logic
			//Debug.Log( "Random lane: " + GetRandomLane() );
			//Debug.Log( "Random gem: " + GetRandomGem() );

			int gemType = GetRandomGem();
			int lane = GetRandomLane();

			if ( gemType == m_nGemTypeNum )
			{
				// Spawn gold
				CreateGoldDrop( lane );
			}
			else
			{
				CreateGem( lane, gemType );
			}
		}
	}

	void UpdateGems()
	{
#if LINKIT_COOP
		bool connected = NetworkManager.IsConnected();
#endif	// LINKIT_COOP

		List< List< Gem > > linkedGemList = new List< List< Gem > >();
		List< List< Gem > > unLinkedGemList = new List< List< Gem > >();
		for ( int i = 0; i < LANE_NUM; ++i )
		{
			linkedGemList.Add( new List<Gem>() );
			unLinkedGemList.Add( new List<Gem>() );
		}

		for ( int i = 0; i < LANE_NUM; ++i )
		{
			for ( int j = 0; j < m_Gems[i].Count; ++j )
			{
				Gem g = m_Gems[i][j].GetComponent<Gem>();

				// Linked gems don't move
				if ( g.Linked )
				{
					linkedGemList[i].Add( g );
					continue;
				}

#if LINKIT_COOP
				// Other player's linked gem
				if ( connected && g.GetComponent<GemNetworkInfo>().OtherLinked )
				{
					continue;
				}
#endif	// LINKIT_COOP

				// Check for linking
				if ( !m_bGameover && m_Link.Linking && LinkGem( m_Gems[i][j] ) )
				{
					AudioManager.Instance.PlaySoundEvent(SOUNDID.GEM_LINK);
					linkedGemList[i].Add( g );
					continue;
				}

				// Move gem
				Vector3 pos = m_Gems[i][j].transform.position;
				pos.y -= m_fBaseGemDropSpeed * Time.deltaTime;
				m_Gems[i][j].transform.position = pos;

				if ( g.Petrified )
				{
					continue;
				}

				// Check for gem in unlinkable zone
				if ( IsGemStoned( g ) )
				{
					m_GemsToBeRemoved.Add( m_Gems[i][j] );
				}
				else
				{
					unLinkedGemList[i].Add( g );
				}
			}
		}
		

		// Check for link breaking
		for ( int i = 0; i < LANE_NUM; ++i )
		{
			for ( int j = 0; j < linkedGemList[i].Count; ++j )
			{
				for ( int k = 0; k < unLinkedGemList[i].Count; ++k )
				{
					if( DidGemCollide( linkedGemList[i][j], unLinkedGemList[i][k] ) )
					{
						AudioManager.Instance.PlaySoundEvent(SOUNDID.GEM_TOUCHED);
						Debug.Log( "Lane " + i + " occur a link breaking" );
						m_LinkedGem.Remove( linkedGemList[i][j] );
						UnlinkGem( linkedGemList[i][j], true );
						CreateRepel( unLinkedGemList[i][k] );
						break;
					}
				}
			}
		}

		// Stoned gems
		for ( int j = 0; j < m_StonedGems.Count; ++j )
		{
			Vector3 pos = m_StonedGems[j].transform.position;
			pos.y -= m_fBaseGemDropSpeed * Time.deltaTime;
			m_StonedGems[j].transform.position = pos;

			// Check for gem out of screen
			if ( pos.y < -m_HalfDimension.y )
			{
				m_GemsToBeDestroyed.Add( m_StonedGems[j] );
			}
		}

		// Cleanup
		for ( int k = 0; k < m_GemsToBeRemoved.Count; ++k )
		{
			PetrifyGem( m_GemsToBeRemoved[k].GetComponent<Gem>() );
		}
		m_PlayerStats.m_nLeakCount += !m_bGameover ? m_GemsToBeRemoved.Count : 0;
		m_GemsToBeRemoved.Clear();

		int healthReduce = m_GemsToBeDestroyed.Count * HEALTH_LOST_PER_GEM;

		if ( m_GemsToBeDestroyed.Count > 0 )
			BreakCombo();

		m_nHealth -= (int)(healthReduce * BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.Shield));
		m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
		m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();
		for ( int k = 0; k < m_GemsToBeDestroyed.Count; ++k )
		{
			DestroyGem( m_GemsToBeDestroyed[k].GetComponent<Gem>() );
		}
		m_GemsToBeDestroyed.Clear();
	}

	void AnimateGems()
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
				m_fAnimationTimer -= ANIMATION_RATE;
				m_nAnimatingFrame = ( m_nAnimatingFrame + 1 ) % ( m_nFrameNum + 1 );

				int frame = ( m_nAnimatingFrame + ANIMATION_FRAME_OFFSET ) % m_nFrameNum;

				for ( int i = 0; i < LANE_NUM; ++ i )
				{ 
					for ( int j = 0; j < m_Gems[i].Count; ++j )
					{
						GameObject gem = m_Gems[i][j];

						if ( gem.GetComponent<Gem>().Linked )
						{
							gem.GetComponent<SpriteRenderer>().sprite = gem.GetComponent<GemSpriteContainer>().m_GlowSprites[frame];
						}
						else if( gem.GetComponent<Gem>().Petrified )
						{
							gem.GetComponent<SpriteRenderer>().sprite = gem.GetComponent<GemSpriteContainer>().m_StoneSprites[frame];
						}
						else
						{
							gem.GetComponent<SpriteRenderer>().sprite = gem.GetComponent<GemSpriteContainer>().m_Sprites[frame];
						}
					}
				}

				if ( m_nAnimatingFrame == m_nFrameNum )
				{
					m_fAnimationIntervalTimer = 0.0f;
					m_fAnimationTimer = 0.0f;
					m_bAnimating = false;
					m_nAnimatingFrame = -1;
				}
			}
		}
	}

	// -------------------------------- Helper functions --------------------------------------------
	bool LinkGem ( GameObject gem )
	{
		Gem g = gem.GetComponent<Gem>();
		Collider2D linkCollider = null;

		bool want = m_Link.LinkType == INVALID_GEM || ( m_Link.LinkType == g.GemType && !g.Petrified );
		if ( want )
			linkCollider = gem.GetComponentInChildren<BoxCollider2D>();
		else
			linkCollider = gem.GetComponentInChildren<CircleCollider2D>();

		bool linked = CheckLinkLinked( linkCollider );

		// Link logic
		if ( linked )
		{
			// Linkable
			if ( want )
			{
				SetLinkGemEffect( g );

				m_LinkedGem.Add( g );

				m_Link.ChangeLinkColor( g.GemType );

#if LINKIT_COOP
				if ( NetworkManager.IsConnected() )
					m_Network.LinkNetworkGem( g, true, m_NetworkGameTimer.GetGameTime() ); //, ( g.transform.position.y - -m_HalfDimension.y ) / ( m_HalfDimension.y * 2.0f ) );
#endif  // LINKIT_COOP
			}
			// Not linkable
			else
			{
				m_Link.BreakLink();

				// Reset combo count
				BreakCombo();

				// Repel effect
				CreateRepel( g );

#if LINKIT_COOP
				// RPC create repel
				if ( NetworkManager.IsConnected() )
				{
					m_Network.CreateRepel( g );
				}
#endif	// LINKIT_COOP
			}
		}

		return linked;
	}

	bool CheckLinkLinked( Collider2D collider )
	{
		Vector3 dir = m_Link.CurrentPoint - m_Link.PreviousPoint;
		float length = dir.magnitude;
		bool linked = false;

		if ( length >= float.Epsilon )
		{
			dir.Normalize();

			Ray ray = new Ray( m_Link.PreviousPoint, dir );
			float distance = 0.0f;
			bool intersected = collider.bounds.IntersectRay( ray, out distance );
			linked = intersected ? distance <= length : false;
		}
		else
		{
			// Test for point inside bound
			linked = collider.bounds.Contains( m_Link.CurrentPoint );
		}

		return linked;
	}

	public void SetLinkGemEffect( Gem gem )
	{
		gem.Linked = true;
		gem.GetComponent<SpriteRenderer>().sprite = gem.GetComponent<GemSpriteContainer>().m_GlowSprites[0];
		ScaleLinkEffect( gem.transform );
	}

	public void ScaleLinkEffect( Transform t )
	{
		t.localScale = m_LinkedGemScale;
	}

	public void UnsetLinkGemEffect( Gem gem )
	{
		gem.Linked = false;
		gem.GetComponent<SpriteRenderer>().sprite = gem.GetComponent<GemSpriteContainer>().m_Sprites[0];
		UnscaleLinkEffect( gem.transform );
	}

	public void UnscaleLinkEffect( Transform t )
	{
		t.localScale = m_DefaultGemScale;
	}

	void AddGainPointsEffect( Vector3 position, int gemType, int eachGain )
	{
		// Particles
		GameObject explosion = ( GameObject )Instantiate( m_GemExplosionPrefab, position, Quaternion.identity );
		ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
		ps.startColor = m_LinkColours[gemType];
		Destroy( explosion, ps.duration + ps.startLifetime + Time.deltaTime );

		if ( eachGain > 0 )
		{
			// Points animation
			GameObject pg = new GameObject( "Points Gain Text" );
			pg.transform.SetParent( m_GameCanvas.transform );

			RectTransform pgtrans = pg.AddComponent<RectTransform>();
			pgtrans.anchoredPosition = m_PointsGain.GetComponent<RectTransform>().anchoredPosition;
			pgtrans.localPosition = Vector3.zero;
			pgtrans.localScale = m_PointsGain.GetComponent<RectTransform>().localScale;
			pgtrans.localRotation = m_PointsGain.GetComponent<RectTransform>().localRotation;
			pgtrans.sizeDelta = m_PointsGain.GetComponent<RectTransform>().sizeDelta;

			Text pgtext = pg.AddComponent<Text>();
			pgtext.text = eachGain.ToString();
			pgtext.fontSize = m_PointsGain.GetComponent<Text>().fontSize;
			pgtext.font = m_PointsGain.GetComponent<Text>().font;
			pgtext.alignment = TextAnchor.MiddleCenter;
			pgtext.color = m_LinkColours[gemType];

			pg.AddComponent<PointsGain>();
			pg.transform.position = position + 0.2f * Vector3.up;

			Destroy( pg, ps.duration + PointsGain.LIFETIME );
		}

        {
            GameObject afterlink = ( GameObject )Instantiate( m_AfterLink[gemType], position, Quaternion.identity );

            AfterLink af = afterlink.GetComponent<AfterLink>();
            af.SetTarget( m_ScoreText.transform.TransformPoint( Vector3.zero ) + ( m_ScoreText.transform.localScale.x * Vector3.right ) );
        }
	}

	void AddGainGoldEffect( Vector3 position )
	{
		// Particles
		GameObject explosion = ( GameObject )Instantiate( m_GemExplosionPrefab, position, Quaternion.identity );
		ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
		ps.startColor = m_GoldColor;
		Destroy( explosion, ps.duration + ps.startLifetime + Time.deltaTime );

		// Text animation
		GameObject pg = new GameObject( "Gold Gain Text" );
		pg.transform.SetParent( m_GameCanvas.transform );

		RectTransform pgtrans = pg.AddComponent<RectTransform>();
		pgtrans.anchoredPosition = m_PointsGain.GetComponent<RectTransform>().anchoredPosition;
		pgtrans.localPosition = Vector3.zero;
		pgtrans.localScale = m_PointsGain.GetComponent<RectTransform>().localScale;
		pgtrans.localRotation = m_PointsGain.GetComponent<RectTransform>().localRotation;
		pgtrans.sizeDelta = m_PointsGain.GetComponent<RectTransform>().sizeDelta;

		Text pgtext = pg.AddComponent<Text>();
		pgtext.text = GOLD_DROP_AMOUNT.ToString();
		pgtext.fontSize = m_PointsGain.GetComponent<Text>().fontSize;
		pgtext.font = m_PointsGain.GetComponent<Text>().font;
		pgtext.alignment = TextAnchor.MiddleCenter;
		pgtext.color = m_GoldColor;

		pg.AddComponent<PointsGain>();
		pg.transform.position = position + 0.2f * Vector3.up;

		Destroy( pg, ps.duration + PointsGain.LIFETIME );

        {
            GameObject afterlink = ( GameObject )Instantiate( m_AfterLink[m_AfterLink.Count() - 1], position, Quaternion.identity );

            AfterLink af = afterlink.GetComponent<AfterLink>();
            af.SetTarget( m_ScoreText.transform.TransformPoint( Vector3.zero ) + ( m_ScoreText.transform.localScale.x * Vector3.right ) );
        }
	}

	int GetPointsGain( int num, int multiplier )
	{
		int pointsGain = num * PER_GEM_POINTS + num * ( ( num - 3 ) * ( PER_GEM_POINTS / 10 ) );
		return multiplier * pointsGain;
	}

	void StartRollingPoints( int pointsGain )
	{
		m_fPointTimer = 0.0f;
		m_nPrevPoints = m_nPoints;
		m_nPoints += pointsGain;
	}

	void StartRollingCombo( int comboGain )
	{
		m_fComboTimer = 0.0f;
		m_nPrevCombo = m_nCurrentCombo;
		m_nCurrentCombo += comboGain;

		if ( m_nCurrentCombo >= HIGH_COMBO && m_CurrentHighComboStrip == null )
		{
			CreateComboStrip();
			AudioManager.Instance.PlaySoundEvent(SOUNDID.FEVER_ENTER);
			AudioManager.Instance.PlaySoundEvent(SOUNDID.FEVER_SUSTAIN);
			m_HighComboZone.GetComponent<HighComboColor>().SetComboColor( 0 );
		}

		if ( m_nHighComboMultiplierIndex < COMBO_MULTIPLIER_ARRAY.Length && 
			 m_nCurrentCombo >= COMBO_MULTIPLIER_ARRAY[m_nHighComboMultiplierIndex] && 
			 m_CurrentHighComboStrip!= null )
		{
			m_CurrentHighComboStrip.GetComponent<HighComboColor>().SetComboColor( m_nHighComboMultiplierIndex );
			m_HighComboZone.GetComponent<HighComboColor>().SetComboColor( m_nHighComboMultiplierIndex );
			m_HighComboEffectLeft.GetComponent<ParticleSystem>().startSpeed = m_HighComboEffectStartSpeed + m_nHighComboMultiplierIndex * 0.5f;
			m_HighComboEffectRight.GetComponent<ParticleSystem>().startSpeed = m_HighComboEffectStartSpeed + m_nHighComboMultiplierIndex * 0.5f;
            m_HighComboStripEffectLeft.GetComponent<ParticleSystem>().startSpeed = m_HighComboStripEffectStartSpeed + m_nHighComboMultiplierIndex * 0.5f;
            m_HighComboStripEffectRight.GetComponent<ParticleSystem>().startSpeed = m_HighComboStripEffectStartSpeed + m_nHighComboMultiplierIndex * 0.5f;
            m_HighComboStripEffectLeft.GetComponent<ParticleSystem>().startColor = m_CurrentHighComboStrip.GetComponent<HighComboColor>().GetComboParticleColor( m_nHighComboMultiplierIndex );
            m_HighComboStripEffectRight.GetComponent<ParticleSystem>().startColor = m_CurrentHighComboStrip.GetComponent<HighComboColor>().GetComboParticleColor( m_nHighComboMultiplierIndex );
            ++m_nHighComboMultiplierIndex;

			StartShowingMultiplier( m_nHighComboMultiplierIndex + 1 );
		}

		//if ( m_nCurrentCombo >= HIGH_COMBO )
		//{
		//	CreateComboSpecular();
		//}

		m_bIsBreakingCombo = false;
		m_bIsStartingCombo = true;
		m_ComboText.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		//m_nComboOpacity = 1.0f;
	}

	void StartPraising( int gemLinked )
	{
		m_fPraiseTimer = 0.0f;

		Color c = m_PraiseText.GetComponent<Text>().color;
		c.a = 1.0f;
		m_PraiseText.GetComponent<Text>().color = c;

		int index = Math.Min( Math.Max( gemLinked - 1, 0 ), PRAISE_ARRAY.Length - 1 );
		m_PraiseText.GetComponent<Text>().text = PRAISE_ARRAY[index];

		m_PraiseText.transform.position = m_PraisePos;
	}

	void StartShowingMultiplier( int multiplier )
	{
		m_fShowMultiplierTimer = 0.0f;

		Color c = m_MultiplierText.GetComponent<Text>().color;
		c.a = 1.0f;
		m_MultiplierText.GetComponent<Text>().color = c;

		m_MultiplierText.GetComponent<Text>().text = "x" + multiplier;

		m_MultiplierText.transform.position = m_ShowMultiplierPos;
	}

	void CheckLevelUp()
	{
		if ( GetLevelUpRequiredPoints( m_nLevel ) <= m_nPoints )
		{
			m_nLevel++;
			m_LevelText.GetComponent<Text>().text = m_nLevel.ToString();

			if ( m_nLevel <= MAX_LEVEL )
			{
				//m_fSpawnRate -= SPAWN_RATE_GROWTH;
				m_fSpawnRate = BASE_SPAWN_RATE - m_nLevel * SPAWN_RATE_GROWTH;
                m_fSpawnRate *= 1.0f / BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.SlowerGems);
                m_fBaseGemDropSpeed = m_HalfDimension.y * 2.0f / ( BASE_GEM_DROP_TIME - ( m_nLevel / 2 ) * GEM_DROP_TIME_GROWTH );
                m_fBaseGemDropSpeed *= BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.SlowerGems);

                Debug.Log( "Spawn rate: " + m_fSpawnRate );
				Debug.Log( "Drop rate: " + m_fBaseGemDropSpeed );
			}

			// level up animation
			GameObject levelup = ( GameObject )Instantiate( m_LevelUpOverlayPrefab, Vector3.zero, Quaternion.identity );
			ParticleSystem ps = levelup.GetComponentInChildren<ParticleSystem>();
			Destroy( levelup, LevelUpAnimation.ANIM_TIME + ps.startLifetime + Time.deltaTime );
		}
	}

	bool UnlinkGems()
	{
		if ( m_bGameover || !m_Link.CheckForDestroy )
			return false;

        int minLinkCount = 3;
        if ( m_GoldObject != null )
        {
            GoldDrop gd = m_GoldObject.GetComponent<GoldDrop>();
            minLinkCount = gd.GetLink() ? minLinkCount - 1 : minLinkCount;
        }

		int multiplier = m_nHighComboMultiplierIndex + 1;
		bool destroy = m_LinkedGem.Count >= minLinkCount;

		if (!destroy)
		{
			AudioManager.Instance.PlaySoundEvent(SOUNDID.GEM_LINK_FAIL);
		}
		if ( destroy
#if LINKIT_COOP
			&& ( !NetworkManager.IsConnected() || NetworkManager.IsPlayerOne() ) 
#endif  // LINKIT_COOP
			)
		{
			// Points
			int pointsGain = GetPointsGain( m_LinkedGem.Count, multiplier );
			pointsGain = (int)(pointsGain * BoosterManager.Instance.GetScoreMultOnce ());
			pointsGain *= (int)(BoosterManager.Instance.GetBoostValue (BOOSTERTYPE.ScoreMult));
			int eachGain = pointsGain / m_LinkedGem.Count;
			StartRollingPoints( pointsGain );
			m_PlayerStats.m_nScore = m_nPoints;

			// Combo
			//m_nCurrentCombo += m_LinkedGem.Count;
			m_PlayerStats.m_nMaxCombo = m_nMaxCombo = Math.Max( m_nMaxCombo, m_nCurrentCombo );
			StartRollingCombo( m_LinkedGem.Count );

			// Praise
			StartPraising( m_LinkedGem.Count );

			// Health
			m_nHealth += HEALTH_GAIN_PER_LINK + ( m_LinkedGem.Count - minLinkCount ) * HEALTH_GAIN_PER_LINK;
			m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
			m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();

			// Level
			CheckLevelUp();

			foreach ( Gem g in m_LinkedGem )
			{
				m_PlayerStats.m_aDestroyCount[g.GemType]++;

				AddGainPointsEffect( g.transform.position, g.GemType, eachGain );
			}

			AudioManager.Instance.PlaySoundEvent(SOUNDID.GEM_LINK_SUCCEED);
			VibManager.Instance.StartVib(VIBSTR.SMALL);
		}

#if LINKIT_COOP
		// Multiplayer logic
		if ( NetworkManager.IsConnected() )
		{
			if ( destroy )
			{
				// RPC destroy gem
				m_Network.DestroyNetworkGems( m_LinkedGem, multiplier );
			}
			else
			{
				if ( m_LinkedGem.Count > 0 )
				{
					// RPC unlink gem
					m_Network.UnlinkNetworkGems( m_LinkedGem, m_NetworkGameTimer.GetGameTime() );
				}
			}
		}
#endif	// LINKIT_COOP

#if LINKIT_COOP
		if ( !destroy
			|| ( destroy && ( !NetworkManager.IsConnected() || NetworkManager.IsPlayerOne() ) ) 
			)
#endif	// LINKIT_COOP
		{
			foreach ( Gem g in m_LinkedGem )
			{
				UnlinkGem( g, destroy );
			}
		}

		m_LinkedGem.Clear();
		m_Link.CheckForDestroy = false;

		UnlinkGold( destroy );

		return true;
	}

	int GetLevelUpRequiredPoints( int nLevel )
	{
        //return ( nLevel + 1 ) * 2000 + ( nLevel * nLevel * 1000 );
        Debug.Log( "Level " + nLevel + " requirements: " + ( ( nLevel + 1 ) * 200 + ( nLevel * nLevel * Math.Max( (int)( 0.5f * nLevel ), 1 ) * 100 ) ) );
		return ( nLevel + 1 ) * 200 + ( nLevel * nLevel * Math.Max( (int)( 0.5f * nLevel ), 1 ) * 100 );
	}

	int GetRandomLane()
	{
		// Get a range excluding these numbers
		var range = Enumerable.Range( 0, LANE_NUM ).Where( i => !m_PreviousLanes.Contains( i ) );

		// On assumption 'LOOKBACK_NUM < LANE_NUM'
		var rand = new System.Random();
		int index = rand.Next( 0, LANE_NUM - m_PreviousLanes.Count );
		int lane = range.ElementAt( index );

		int laneIndex = ( m_nLastInsertedLaneIndex + 1 ) % LOOKBACK_NUM;
		m_PreviousLanes[laneIndex] = lane;
		m_nLastInsertedLaneIndex = laneIndex;

		return lane;
	}

	int GetRandomGem()
	{
		bool getNextSeq = m_lSequence.Count <= m_nSequenceIndex;
		// Get the next sequence
		if ( getNextSeq )
		{
			m_mSequenceGemTypeMap.Clear();
			m_nSequenceIndex = 0;

			m_lSequence = m_SpawnPattern.GetSequence( m_nLevel, ref m_nSequenceIndexFromList );

			// Determine gem type
			while ( m_mSequenceGemTypeMap.Count != m_nGemTypeNum )
			{
				// Get a range excluding these numbers
				var range = Enumerable.Range( 0, m_nGemTypeNum ).Where( i => !m_mSequenceGemTypeMap.Contains( i ) );

				// On assumption 'LOOKBACK_NUM < LANE_NUM'
				var rand = new System.Random();
				int index = rand.Next( 0, m_nGemTypeNum - m_mSequenceGemTypeMap.Count );
				int gemType = range.ElementAt( index );

				m_mSequenceGemTypeMap.Add( gemType );
			}

			/*
			string log = "[ ";
			for ( int j = 0; j < m_mSequenceGemTypeMap.Count; ++j )
			{
				log += m_mSequenceGemTypeMap[j] + " ";
			}
			log += "]";
			Debug.Log( "Gem type map: " + log );
			*/
		}

		SortedDictionary<float, int> gemTypeToCheck = new SortedDictionary<float, int>();	// key: y, value: gem type
		// Check for unlinkable gem
		for ( int i = 0; i < LANE_NUM; ++i )
		{
			for ( int j = 0; j < m_Gems[i].Count; ++j )
			{
				Gem g = m_Gems[i][j].GetComponent<Gem>();

				if( g.transform.position.y <= m_SpawnDangerArea )
				{
					if ( m_aGemCount[g.GemType] < 3 )
					{
						if ( !gemTypeToCheck.ContainsValue( g.GemType ) )
							gemTypeToCheck.Add( g.transform.position.y, g.GemType );
					}
				}
				else
				{
					break;
				}
			}
		}

#if DEBUG_SPAWN
		SpawnIndicator spawnIndicator = GameObject.Find( "Spawn Indicator" ).GetComponent<SpawnIndicator>();
		for ( int i = 0; i < 10; ++i )
		{
			spawnIndicator.SetGemTypeCheck( i, 0, false, false );
		}

		int spawnIndicateCounter = 9;
#endif
		//for ( int i = 0; i < m_aGemCount.Length; ++i )
		foreach ( KeyValuePair<float, int> entry in gemTypeToCheck )
		{
			int gemType = entry.Value;
			// If there is a gem there is unlinkable. Spawn
			if ( m_aGemCount[gemType] > 0 && m_aGemCount[gemType] < 3 )
			{
				int nLookBackNum = Math.Min( GEM_LOOKBACK_NUM, m_lSequence.Count - m_nSequenceIndex );
				bool bContainGemType = false;
				for ( int j = 0; j < nLookBackNum; ++j )
				{
					if ( m_lSequence[m_nSequenceIndex + j] == gemType )
					{
						bContainGemType = true;
						break;
					}
				}

				// Spawn
				if ( !bContainGemType )
				{
#if DEBUG_SPAWN
					spawnIndicator.SetGemTypeCheck( spawnIndicateCounter--, gemType, true, true );
#endif
					return gemType;
				}
				else
				{
#if DEBUG_SPAWN
					spawnIndicator.SetGemTypeCheck( spawnIndicateCounter--, gemType, true, false );
#endif
					// Check the first one only
					break;
				}
			}
		}

		// Check number of different leak gems and speed up spawn timer if need be
		//m_fSpawnTimer;

		// Sequence end 
		if ( getNextSeq && m_GoldObject == null && m_GoldIntervalTimer >= GOLD_SPAWN_INTERVAL )
		{
			float range = UnityEngine.Random.Range( 0.0f, 1.0f );
			if ( range <= GOLD_SPAWN_CHANCE )
			{
				m_GoldIntervalTimer = 0.0f;	// Reset gold timer
				return m_nGemTypeNum;		// Return size to indicate spawn gold
			}
		}
		

		int gemMapKey = m_lSequence[m_nSequenceIndex];
		m_nSequenceIndex++;

		return m_mSequenceGemTypeMap[gemMapKey];

		// Old
		/*
		bool random = true;

		if ( m_PreviousGems.Count > 0 )
		{
			random = ( m_PreviousGems.Count == 1 && m_nTotalGemCount < 3 ) ? Random.Range( 0, m_nGemTypeNum - 1 ) == 0 : false;
		}

		int gem = ( random ) ? Random.Range( 0, m_nGemTypeNum ) : m_PreviousGems[0];

		// If linkable, don't need to force generate same gem the next time round
		if ( m_aGemCount[gem] + 1 >= 3 )
		{
			m_PreviousGems.Remove( gem );
		}
		else
		{
			if( !m_PreviousGems.Contains( gem ) )
			{
				m_PreviousGems.Add( gem );
			}
		}

		return gem;
		*/
	}

	public float GetGemX( int lane )
	{
		return ( lane + 0.5f ) * m_fLaneWidth + -m_HalfDimension.x;
	}

	void CreateGem( int lane, int gemType )
	{
		GameObject gem = ( GameObject )Instantiate( m_aGemList[gemType], new Vector3( GetGemX( lane ), m_HalfDimension.y ), Quaternion.identity );
		gem.GetComponent<Gem>().Lane = lane;
		gem.GetComponent<Gem>().GemType = gemType;
		gem.GetComponent<Gem>().SequenceIndex = m_nSequenceIndexFromList;
		gem.transform.localScale *= BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.BiggerGems);
        if ( ( (float)Screen.width / (float)Screen.height ) < ( 9.0f / 16.0f ) )
        {
            gem.transform.localScale *= ( (float)Screen.width / (float)Screen.height ) / ( 9.0f / 16.0f );
        }
#if LINKIT_COOP
		if ( NetworkManager.IsConnected() )
		{
			int id = 0;
			if ( m_UnusedGemIDList.Count > 50 )
			{
				id = m_UnusedGemIDList.Dequeue();
			}
			else
			{
				id = m_NextGemID++;
			}

			gem.GetComponent<GemNetworkInfo>().ID = id;
			m_Network.SpawnNetworkGem( gem.GetComponent<Gem>(), m_NetworkGameTimer.GetGameTime() );
		}
		else
		{
			Destroy ( gem.GetComponent<GemNetworkInfo>() );
		}
#else	// !LINKIT_COOP
		Destroy ( gem.GetComponent<GemNetworkInfo>() );
#endif	// LINKIT_COOP

		if ( m_GemDetails != null )
		{
			SetGemSpriteContainer( gem.GetComponent<GemSpriteContainer>(), gemType );
		}

		// Updating live info
		m_aGemCount[gemType]++;
		m_nTotalGemCount++;
		m_Gems[lane].Add( gem );
	}

	void CreateGoldDrop( int lane )
	{
		m_GoldObject = ( GameObject )Instantiate( m_GoldPrefab, new Vector3( GetGemX( lane ), m_HalfDimension.y ), Quaternion.identity );
        m_GoldObject.transform.localScale *= BoosterManager.Instance.GetBoostValue(BOOSTERTYPE.BiggerGems);
        if ( ( (float)Screen.width / (float)Screen.height ) < ( 9.0f / 16.0f ) )
        {
            m_GoldObject.transform.localScale *= ( (float)Screen.width / (float)Screen.height ) / ( 9.0f / 16.0f );
        }
#if LINKIT_COOP
		if ( NetworkManager.IsConnected() && NetworkManager.IsPlayerOne() )
		{
			// RPC spawn gold
			m_Network.SpawnNetworkGoldDrop( lane, m_NetworkGameTimer.GetGameTime() );
		}
#endif	// LINKIT_COOP
	}

	void DestroyGoldDrop()
	{
		if ( m_GoldObject == null )
			return;

		Destroy( m_GoldObject );
		m_GoldObject = null;
	}

#if LINKIT_COOP
	public void AddNetworkGameTimer( NetworkGameTime networkGameTimer )
	{
		m_NetworkGameTimer = networkGameTimer;
	}

	public void SpawnNetworkGem( int gemID, int gemType, int lane, float spawnTime )
	{
		GameObject gem = ( GameObject )Instantiate( m_aGemList[gemType], new Vector3( GetGemX( lane ), m_HalfDimension.y ), Quaternion.identity );
		gem.GetComponent<Gem>().Lane = lane;
		gem.GetComponent<Gem>().GemType = gemType;
		gem.GetComponent<Gem>().SequenceIndex = m_nSequenceIndexFromList;
		gem.GetComponent<GemNetworkInfo>().ID = gemID;

		if ( m_GemDetails != null )
		{
			SetGemSpriteContainer( gem.GetComponent<GemSpriteContainer>(), gemType );
		}

		// Move gem
		float timeElapsed = m_NetworkGameTimer.GetGameTime() - spawnTime;
		Vector3 pos = gem.transform.position;
		pos.y -= m_fBaseGemDropSpeed * timeElapsed;
		gem.transform.position = pos;

		// Updating live info
		m_aGemCount[gemType]++;
		m_nTotalGemCount++;
		m_Gems[lane].Add( gem );

		CheckDelayedTime( spawnTime );
	}

	// Use by Network Gem (NOT USED)
	public void AddNetworkGem( NetworkGem ng )
	{
		Gem gem = ng.GetComponent<Gem>();
		int lane = gem.Lane;
		//int gemType = gem.GemType;

		// Change to user sprite
		if ( m_GemDetails != null )
		{
			SetGemSpriteContainer( gem.GetComponent<GemSpriteContainer>(), gem.GemType );
		}

		// Updating live info
		//m_aGemCount[gemType]++;
		//m_nTotalGemCount++;
		m_Gems[lane].Add( gem.gameObject );
	}

	public void UnlinkNetworkGems( string[] ids, string[] lanes, float linkTime )
	{
		int size = ids.Length;
		for ( int i = 0; i < size; ++i )
		{
			string id = ids[i];
			string lane = lanes[i];
			LinkNetworkGem( Convert.ToInt32( id ), Convert.ToInt32( lane ), false, linkTime );
		}

		CheckDelayedTime( linkTime );
	}

	public void LinkNetworkGem( int id, int lane, bool link, float linkTime )
	{
		if ( link )
		{
			CheckDelayedTime( linkTime );
		}

		if ( lane >= 0 || lane < LANE_NUM )
		{
			for ( int j = 0; j < m_Gems[lane].Count; ++j )
			{
				GemNetworkInfo info = m_Gems[lane][j].GetComponent<GemNetworkInfo>();
				if ( info && info.ID == id )
				{
					info.LinkNetworkGem( link );

					if ( link )
					{
						float timeAgo = linkTime - m_NetworkGameTimer.GetGameTime();
						Vector3 pos = info.transform.position;
						pos.y += m_fBaseGemDropSpeed * timeAgo;
					}
					else
					{
						float timeElapsed = m_NetworkGameTimer.GetGameTime() - linkTime;
						Vector3 pos = info.transform.position;
						pos.y -= m_fBaseGemDropSpeed * timeElapsed;
						info.transform.position = pos;
					}
					return;
				}
			}
		}

		Gem promoteGem = null;
		for ( int j = 0; j < m_StonedGems.Count; ++j )
		{
			GemNetworkInfo info = m_StonedGems[j].GetComponent<GemNetworkInfo>();
			if ( info && info.ID == id )
			{
				info.LinkNetworkGem( link );

				if ( link )
				{
					float timeAgo = linkTime - m_NetworkGameTimer.GetGameTime();
					Vector3 pos = info.transform.position;
					pos.y += m_fBaseGemDropSpeed * timeAgo;

					// Promote if gem is not stoned
					if ( !IsGemStoned( info.GetComponent<Gem>() ) )
					{
						promoteGem = info.GetComponent<Gem>();
						break;
					}
				}
				// Ignore unlink portion since gem is already unlinkable
				return;
			}
		}

		if ( promoteGem != null )
		{
			UnPetrifyGem( promoteGem );
		}

		if ( !link )
		{
			GameObject dead = null;
			for ( int j = 0; j < m_NetworkGemsToBeDestroyed.Count; ++j )
			{
				GemNetworkInfo info = m_NetworkGemsToBeDestroyed[j].GetComponent<GemNetworkInfo>();
				if ( info && info.ID == id )
				{
					dead = m_NetworkGemsToBeDestroyed[j];
					break;
				}
			}

			if ( dead )
			{
				m_NetworkGemsToBeDestroyed.Remove( dead );
				m_StonedGems.Add( dead );
			}
		}
	}

	public void DestroyNetworkGems( string[] ids, string[] lanes, int multiplier )
	{
		if ( NetworkManager.IsConnected() && NetworkManager.IsPlayerOne() )
		{
			m_Network.DestroyNetworkGems( string.Join( ",", ids ), string.Join( ",", lanes ), multiplier );
		}

		// Sychron point
		int pointsGain = GetPointsGain( ids.Length, multiplier );
		int eachGain = pointsGain / ids.Length;
		StartRollingPoints( pointsGain );
		m_PlayerStats.m_nScore = m_nPoints;

		// Sychron Combo
		//m_nCurrentCombo += ids.Length;
		m_PlayerStats.m_nMaxCombo = m_nMaxCombo = Math.Max( m_nMaxCombo, m_nCurrentCombo );
		StartRollingCombo( ids.Length );

		// Sychron health
		m_nHealth += HEALTH_GAIN_PER_LINK + ( ids.Length - 3 ) * HEALTH_GAIN_PER_LINK;
		m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
		m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();

		// Synchron level
		CheckLevelUp();

		int size = ids.Length;
		for ( int i = 0; i < size; ++i )
		{
			string id = ids[i];
			string lane = lanes[i];
			DestroyNetworkGem( Convert.ToInt32( id ), Convert.ToInt32( lane ), eachGain );
		}
	}

	public void DestroyNetworkGem( int id, int lane, int eachGain )
	{
		Gem g = null;
		if ( lane >= 0 || lane < LANE_NUM )
		{
			for ( int j = 0; j < m_Gems[lane].Count; ++j )
			{
				GemNetworkInfo info = m_Gems[lane][j].GetComponent<GemNetworkInfo>();
				if ( info && info.ID == id )
				{
					g = info.GetComponent<Gem>();
					break;
				}
			}
		}

		if ( g == null )
		{
			for ( int j = 0; j < m_StonedGems.Count; ++j )
			{
				GemNetworkInfo info = m_StonedGems[j].GetComponent<GemNetworkInfo>();
				if ( info && info.ID == id )
				{
					g = info.GetComponent<Gem>();
					break;
				}
			}

			if ( g != null )
			{
				//m_StonedGems.Remove( g.gameObject );
				//m_Gems[g.Lane].Add( g.gameObject );
				UnPetrifyGem( g );
			}
		}

		if ( g == null )
		{
			for ( int j = 0; j < m_NetworkGemsToBeDestroyed.Count; ++j )
			{
				GemNetworkInfo info = m_NetworkGemsToBeDestroyed[j].GetComponent<GemNetworkInfo>();
				if ( info && info.ID == id )
				{
					g = m_NetworkGemsToBeDestroyed[j].GetComponent<Gem>();
					break;
				}
			}

			if ( g != null )
			{
				m_NetworkGemsToBeDestroyed.Remove( g.gameObject );
				UnPetrifyGem( g );
				//m_Gems[g.Lane].Add( g.gameObject );
			}
		}

		if ( g != null )
		{
			UnlinkGem( g, true );

			m_PlayerStats.m_aDestroyCount[g.GemType]++;

			// Particle effect
			AddGainPointsEffect( g.transform.position, g.GemType, eachGain );
		}
	}

	public void CreateNetworkRepel( int id, int lane )
	{
		if ( lane >= 0 || lane < LANE_NUM )
		{
			for ( int j = 0; j < m_Gems[lane].Count; ++j )
			{
				GemNetworkInfo info = m_Gems[lane][j].GetComponent<GemNetworkInfo>();
				if ( info && info.ID == id )
				{
					CreateRepel( info.GetComponent<Gem>() );
					BreakCombo();
					return;
				}
			}
		}

		for ( int j = 0; j < m_StonedGems.Count; ++j )
		{
			GemNetworkInfo info = m_StonedGems[j].GetComponent<GemNetworkInfo>();
			if ( info && info.ID == id )
			{
				CreateRepel( info.GetComponent<Gem>() );
				BreakCombo();
				return;
			}
		}
	}

	public void UpdateNetworkHealth( int healthGain )
	{
		m_nHealth += healthGain;
		m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
		m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();
		m_GemsToBeDestroyed.Clear();

		if( healthGain < 0 )
		{
			BreakCombo();
		}
	}

	public void CheckDelayedTime( float timeSent )
	{
		float timeReceived = m_NetworkGameTimer.GetGameTime();
		if ( Mathf.Abs( timeReceived - timeSent ) > NetworkGameTime.LAGGY_TIME_DIFF )
		{
			// Sent request to sync combo, health and score
			m_Network.RequestSyncInfo();
		}
	}

	public void RequestSyncInfo()
	{
		// Sent info to sync combo, health and score
		m_Network.SendSyncInfo( m_nCurrentCombo, m_nHealth, m_nPoints );
	}

	public void SyncInfo(  int combo, int health, int points  )
	{
		m_nCurrentCombo = combo;
		m_nHealth = health;
		m_nPoints = points;
	}

	public void SpawnNetworkGoldDrop( int lane, float spawnTime )
	{
		if ( m_GoldObject != null )
			return;

		CreateGoldDrop( lane );

		// Move gold
		float timeElapsed = m_NetworkGameTimer.GetGameTime() - spawnTime;
		Vector3 pos = m_GoldObject.transform.position;
		pos.y -= m_fBaseGemDropSpeed * timeElapsed;
		m_GoldObject.transform.position = pos;

		CheckDelayedTime( spawnTime );
	}
#endif	// LINKIT_COOP

	public void PetrifyGem( Gem gem )
	{
		AudioManager.Instance.PlaySoundEvent(SOUNDID.GEM_DROPPED);
		m_aGemCount[gem.GemType]--;
		m_nTotalGemCount--;
		m_Gems[gem.Lane].Remove( gem.gameObject );
		m_StonedGems.Add( gem.gameObject );
		gem.GetComponent<SpriteRenderer>().sprite = gem.GetComponent<GemSpriteContainer>().m_StoneSprites[0];
		gem.Petrified = true;

#if LINKIT_COOP
		if ( !NetworkManager.IsConnected() || NetworkManager.IsPlayerOne() )
			m_lFailedSequenceCount[gem.GetComponent<Gem>().SequenceIndex]++;
#else	// !LINKIT_COOP
		m_lFailedSequenceCount[gem.GetComponent<Gem>().SequenceIndex]++;
#endif	// LINKIT_COOP
	}

	public void UnPetrifyGem( Gem gem )
	{
		m_aGemCount[gem.GemType]++;
		m_nTotalGemCount++;
		m_Gems[gem.Lane].Add( gem.gameObject );
		m_StonedGems.Remove( gem.gameObject );
		gem.GetComponent<SpriteRenderer>().sprite = gem.GetComponent<GemSpriteContainer>().m_Sprites[0];
		gem.Petrified = false;

#if LINKIT_COOP
		if ( !NetworkManager.IsConnected() || NetworkManager.IsPlayerOne() )
			m_lFailedSequenceCount[gem.GetComponent<Gem>().SequenceIndex]--;
#else	// !LINKIT_COOP
		m_lFailedSequenceCount[gem.GetComponent<Gem>().SequenceIndex]--;
#endif	// LINKIT_COOP
	}

	public void DestroyGem( Gem gem )
	{
		m_StonedGems.Remove( gem.gameObject );

#if LINKIT_COOP
		if ( NetworkManager.IsConnected() )
		{
			GemNetworkInfo info = gem.GetComponent<GemNetworkInfo>();
			if ( info != null && info.OtherLinked )
			{
				m_NetworkGemsToBeDestroyed.Add( gem.gameObject );
				return;
			}
		}
#endif	// LINKIT_COOP

		DestroyGemImpl( gem );
	}

	public void UnlinkGem( Gem gem, bool destroy )
	{
		if ( destroy )
		{ 
			m_Gems[gem.Lane].Remove( gem.gameObject );
			m_aGemCount[gem.GemType]--;
			m_nTotalGemCount--;

			DestroyGemImpl( gem );
		}
		else
		{
			UnsetLinkGemEffect( gem );
		}
	}

	public void UnlinkGold( bool destroy )
	{
		if ( m_GoldObject == null )
			return;

		GoldDrop gd = m_GoldObject.GetComponent<GoldDrop>();

		if ( !gd.GetLink() )
			return;

		if ( destroy )
		{
			// particle effects
			AddGainGoldEffect( m_GoldObject.transform.position );

			DestroyGoldDrop();
			m_PlayerStats.m_nCoinsGain += GOLD_DROP_AMOUNT;
		}
		else
		{
			gd.LinkGold( false );
			//UnscaleLinkEffect( m_GoldObject.transform );
            m_GoldObject.transform.localScale = m_DefaultGoldScale;
        }
	}

	public bool IsGemStoned( Gem gem )
	{
		Transform t = gem.transform;
		return IsInUnlinkableZone( t );
	}

	public bool IsInUnlinkableZone( Transform t )
	{
		Renderer r = t.GetComponent<SpriteRenderer>();
        //BoxCollider2D c = t.GetComponentInChildren<BoxCollider2D>();
        //return ( t.position.y + ( c.size.y * 0.5f ) <= m_LineLine.transform.position.y ) ||
		//	   ( t.position.y <= ( ( m_fGameoverTimer / GAMEOVER_ANIMATION ) * m_HalfDimension.y * 2.0f ) + -m_HalfDimension.y );
        CircleCollider2D c = t.GetComponentInChildren<CircleCollider2D>();
        return ( t.position.y + ( c.radius * 0.5f ) <= m_LineLine.transform.position.y ) ||
			   ( t.position.y <= ( ( m_fGameoverTimer / GAMEOVER_ANIMATION ) * m_HalfDimension.y * 2.0f ) + -m_HalfDimension.y );
	}

	void AnimatePoints()
	{
		m_fPointTimer += Time.deltaTime;
		if ( m_fPointTimer < TIME_TO_ACTUAL_POINTS + Time.deltaTime )
		{
			if(!m_bScoreText_IsAnim)
			{
				m_bScoreText_IsAnim = true;
				AudioManager.Instance.PlaySoundEvent(SOUNDID.SCORE_TICK);
			}

			m_fPointTimer = m_fPointTimer > TIME_TO_ACTUAL_POINTS ? TIME_TO_ACTUAL_POINTS : m_fPointTimer;
			m_nShowingPoints = ( int )( ( m_fPointTimer / TIME_TO_ACTUAL_POINTS ) * ( m_nPoints - m_nPrevPoints ) ) + m_nPrevPoints;
			m_ScoreText.GetComponent<Text>().text = m_nShowingPoints.ToString();
		}
		else
		{
			if(m_bScoreText_IsAnim)
			{
				m_bScoreText_IsAnim = false;
				AudioManager.Instance.PlaySoundEvent(SOUNDID.SCORE_TICK_STOP);
			}
		}
	}

	void AnimateCombo()
	{
		m_fComboTimer += Time.deltaTime;

		if ( m_fComboTimer < TIME_TO_ACTUAL_COMBO + Time.deltaTime )
		{
			if(!m_bComboText_IsAnim)
			{
				m_bComboText_IsAnim = true;
				AudioManager.Instance.PlaySoundEvent(SOUNDID.COMBO_TICK);
			}
			m_fComboTimer = m_fComboTimer > TIME_TO_ACTUAL_COMBO ? TIME_TO_ACTUAL_COMBO : m_fComboTimer;
			m_nShowingCombo = ( int )( ( m_fComboTimer / TIME_TO_ACTUAL_COMBO ) * ( m_nCurrentCombo - m_nPrevCombo ) ) + m_nPrevCombo;
			m_ComboText.GetComponent<Text>().text = "Combo\n" + m_nShowingCombo.ToString();
		}
		else
		{
			if(m_bComboText_IsAnim)
			{
				m_bComboText_IsAnim = false;
				AudioManager.Instance.PlaySoundEvent(SOUNDID.COMBO_TICK_STOP);
			}
		}

		//m_nComboOpacity -= COMBO_FADE_TIME_RECIPROCAL * Time.deltaTime;
	}

	void AnimatePraise()
	{
		if ( m_fPraiseTimer >= TIME_TO_PRAISE_FADE )
			return;

		m_fPraiseTimer += Time.deltaTime;
		m_fPraiseTimer = Math.Min( TIME_TO_PRAISE_FADE, m_fPraiseTimer );

		float factor = m_fPraiseTimer / TIME_TO_PRAISE_FADE;
		Color c = m_PraiseText.GetComponent<Text>().color;
		c.a = 1.0f - ( float )Math.Pow( factor, 2.0 );
		m_PraiseText.GetComponent<Text>().color = c;

		Vector3 pos = m_PraiseText.transform.position;
		pos.y = m_PraisePos.y + factor * PRAISE_MOVE_DISTANCE;
		m_PraiseText.transform.position = pos;

		Color tmp = m_PraiseText.GetComponent<Text>().color;
		if(isPraiseUp)
		{
			tmp.g += 0.03f;
			tmp.b -= 0.03f;
		}
		else
		{
			tmp.g -= 0.03f;
			tmp.b += 0.03f;
		}

		if(isPraiseUp && tmp.b <= 0.0f)
			isPraiseUp = !isPraiseUp;
		if(!isPraiseUp && tmp.b >= 0.5f)
			isPraiseUp = !isPraiseUp;
		m_PraiseText.GetComponent<Text>().color = tmp;
	}

	void AnimateShowMultiplier()
	{
		if ( m_fShowMultiplierTimer >= TIME_TO_PRAISE_FADE )
			return;

		m_fShowMultiplierTimer += Time.deltaTime;
		m_fShowMultiplierTimer = Math.Min( TIME_TO_PRAISE_FADE, m_fShowMultiplierTimer );

		float factor = m_fShowMultiplierTimer / TIME_TO_PRAISE_FADE;
		Color c = m_MultiplierText.GetComponent<Text>().color;
		c.a = 1.0f - (float)Math.Pow( factor, 2.0 );
		m_MultiplierText.GetComponent<Text>().color = c;

		Vector3 pos = m_MultiplierText.transform.position;
		pos.y = m_ShowMultiplierPos.y + factor * PRAISE_MOVE_DISTANCE;
		m_MultiplierText.transform.position = pos;
	}

	void AnimateOverlays()
	{
		// Low Health
		{
			if ( m_nHealth <= LOW_HEALTH )
			{
				m_HealthLowTimer += Time.deltaTime;
			}
			else
			{
				m_HealthLowTimer -= Time.deltaTime;
			}

			Pulsing pulsing = m_HealthLowOverlay.GetComponent<Pulsing>();
			if ( !pulsing.IsPulsing() && m_HealthLowTimer >= HEALTH_LOW_OVERLAY_FADE_TIME )
			{
				pulsing.StartFadeOut();
			}
			else if ( pulsing.IsPulsing() && m_nHealth > LOW_HEALTH )
			{
				pulsing.StopPulsing();
				m_HealthLowTimer = ( m_HealthLowOverlay.GetComponent<SpriteRenderer>().color.a / pulsing.m_fMaxAlpha ) * HEALTH_LOW_OVERLAY_FADE_TIME;
			}

			m_HealthLowTimer = Mathf.Clamp( m_HealthLowTimer, 0.0f, HEALTH_LOW_OVERLAY_FADE_TIME );

			float factor = Mathf.Pow( m_HealthLowTimer / HEALTH_LOW_OVERLAY_FADE_TIME, 2.0f );

			SpriteRenderer sr = m_HealthLowOverlay.GetComponent<SpriteRenderer>();
			Color c = sr.color;
			c.a = factor * pulsing.m_fMaxAlpha;
			sr.color = c;
		}

		// High combo
		{
			if ( m_nCurrentCombo >= HIGH_COMBO )
			{
				m_HighComboZoneTimer += Time.deltaTime;
			}
			else
			{
				m_HighComboZoneTimer -= Time.deltaTime;
			}

			Pulsing pulsing = m_HighComboZone.GetComponent<Pulsing>();
			if ( !pulsing.IsPulsing() && m_HighComboZoneTimer >= HIGH_COMBO_OVERLAY_FADE_TIME )
			{
				pulsing.StartFadeOut();
			}

			m_HighComboZoneTimer = Mathf.Clamp( m_HighComboZoneTimer, 0.0f, HIGH_COMBO_OVERLAY_FADE_TIME );

			float factor = Mathf.Pow( m_HighComboZoneTimer / HIGH_COMBO_OVERLAY_FADE_TIME, 2.0f );

			SpriteRenderer sr = m_HighComboZone.GetComponent<SpriteRenderer>();
			Color c = sr.color;
			c.a = factor * pulsing.m_fMaxAlpha;
			sr.color = c;

			if ( m_nCurrentCombo >= HIGH_COMBO )
			{
				m_HighComboStripSpecularTimer += Time.deltaTime;
				if ( m_HighComboStripSpecularTimer >= HIGH_COMBO_SPECULAR_INTERVAL )
				{
					m_HighComboStripSpecularTimer -= HIGH_COMBO_SPECULAR_INTERVAL;
					CreateComboSpecular();
				}
			}
		}
	}

	bool DidGemCollide( Gem lhs, Gem rhs )
	{
		Collider2D lc = lhs.GetComponentInChildren<CircleCollider2D>();
		Collider2D rc = rhs.GetComponentInChildren<CircleCollider2D>();

		return lc.bounds.Intersects( rc.bounds );
	}

	void CreateRepel( Gem g )
	{
		// Repel effect
#if USE_SINGLE_REPEL
		GameObject repel = ( GameObject )Instantiate( m_Repel, g.transform.position + FRONT_OFFSET, Quaternion.identity );
		repel.transform.SetParent( g.transform );
		repel.GetComponent<SpriteRenderer>().color = m_LinkColours[g.GetComponent<Gem>().m_nGemType];
		Destroy( repel, Repel.REPEL_ANIM_TIME );
#else
		GameObject repel = ( GameObject )Instantiate( m_aRepels[g.GemType], g.transform.position + FRONT_OFFSET, Quaternion.identity );
		repel.transform.SetParent( g.transform );
		Destroy( repel, RepelAnimator.LIFETIME );
#endif
	}

	Color GetLifeLineColour()
	{
		int health = Mathf.Clamp( m_nHealth, 0, MAX_HEALTH );
		int factor = MAX_HEALTH / ( m_LineLineColors.Length - 1 );
		//int maxIndex = m_LineLineColors.Length - 1;
		int index = health / factor;
		int nextIndex = ( index + 1 ) % m_LineLineColors.Length;
		int interpolateFactor = health % factor;
		float interpolate = (float)interpolateFactor / factor;

		Color c1 = m_LineLineColors[index];
		Color c2 = m_LineLineColors[nextIndex];

		Color c = c1 + interpolate * ( c2 - c1 );

		return c;
	}

	public void SetGemSpriteContainer( GemSpriteContainer gemSpriteContainer, int gemType )
	{
		if( m_GemDetails != null )
		{
			m_GemDetails.SetGemSpriteContainer( gemSpriteContainer, gemType );
		}
	}

	void CheckGameOver()
	{
		if ( !m_bGameover && m_nHealth <= 0 )
		{
			m_bGameover = true;
			m_Link.BreakLink();
			BreakCombo();
			m_Link.CheckForDestroy = false;
			VibManager.Instance.StartVib(VIBSTR.BIG);
			foreach ( Gem g in m_LinkedGem )
			{
				UnlinkGem( g, false );
			}
		}
	}

	public void SetGameOver()
	{
		m_nHealth = 0;
		Analytics.CustomEvent("ManualExitGame", new Dictionary<string, object>
		{
			{"Exit", 1}
		});
	}

	public void PauseGame()
	{
		m_bIsPaused = true;
		AudioManager.Instance.PlaySoundEvent(SOUNDID.MENU_CLICK);
	}

	public void UnpauseGame()
	{
		m_bIsPaused = false;
		AudioManager.Instance.PlaySoundEvent(SOUNDID.MENU_CLICK);
	}

	void UpdateGameover()
	{
		if ( !m_bGameover )
			return;

		m_fGameoverTimer += Time.deltaTime;

		if ( m_fGameoverTimer >= GAMEOVER_ANIMATION + Time.deltaTime )
		{
			GameObject.FindGameObjectWithTag( "Transition" ).GetComponent<Transition>().StartFadeOut( GoToScore );
		}
	}

	void CreateLink()
	{
		if ( m_bGameover )
			return;

#if LINKIT_COOP
		if ( NetworkManager.IsConnected() )
		{
			m_LinkObject = PhotonNetwork.Instantiate( "Link", Vector3.zero, Quaternion.identity, 0 );
		}
		else
		{
			m_LinkObject = ( GameObject )Instantiate( m_LinkPrefab, Vector3.zero, Quaternion.identity );
			Destroy( m_LinkObject.GetComponent<PhotonView>() );
			Destroy( m_LinkObject.GetComponent<NetworkLink>() );
		}
#else	// !LINKIT_COOP
		m_LinkObject = ( GameObject )Instantiate( m_LinkPrefab, Vector3.zero, Quaternion.identity );
		Destroy( m_LinkObject.GetComponent<PhotonView>() );
		Destroy( m_LinkObject.GetComponent<NetworkLink>() );
#endif	// LINKIT_COOP
	}

	void DestroyGemImpl( Gem gem )
	{
#if LINKIT_COOP
		if ( NetworkManager.IsConnected() && NetworkManager.IsPlayerOne() )
		{
			m_UnusedGemIDList.Enqueue( gem.GetComponent<GemNetworkInfo>().ID );
		}
#endif	// LINKIT_COOP

		Destroy( gem.gameObject );
	}

	void BreakCombo()
	{
		// Check if current combo more than 1, shake screen?

		if ( m_CurrentHighComboStrip != null )
		{
			AudioManager.Instance.PlaySoundEvent(SOUNDID.FEVER_EXIT);
			AudioManager.Instance.PlaySoundEvent(SOUNDID.FEVER_SUSTAIN_STOP);
			DestroyComboStrip();
		}

		{
			Pulsing pulsing = m_HighComboZone.GetComponent<Pulsing>();
			pulsing.StopPulsing();
			m_HighComboZoneTimer = ( m_HighComboZone.GetComponent<SpriteRenderer>().color.a / pulsing.m_fMaxAlpha ) * HIGH_COMBO_OVERLAY_FADE_TIME;
		}

		if(m_nCurrentCombo > 0)
			AudioManager.Instance.PlaySoundEvent(SOUNDID.COMBO_LOST);

		m_nCurrentCombo = 0;
		m_ComboText.GetComponent<Text>().text = "Combo\n" + m_nCurrentCombo.ToString();
		m_bIsBreakingCombo = true;
		m_bIsStartingCombo = false;
		m_ComboText.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		//m_nComboOpacity = 0.0f;

		m_nHighComboMultiplierIndex = 0;
	}

	void CreateComboStrip()
	{
		if ( m_CurrentHighComboStrip != null )
			return;

		m_CurrentHighComboStrip = (GameObject)Instantiate( m_HighComboStrip, Vector3.zero, Quaternion.identity );
		m_CurrentHighComboStrip.transform.SetParent( m_GameCanvas.transform );
		m_CurrentHighComboStrip.transform.localPosition = m_HighComboStripPos;
		m_CurrentHighComboStrip.transform.localScale = m_HighComboStripScale;
		m_CurrentHighComboStrip.GetComponent<ComboMove>().StartEnter();
		m_CurrentHighComboStrip.SetActive( true );

		m_HighComboStripSpecularTimer = 0.0f;

		m_HighComboEffectLeft.GetComponent<ParticleSystem>().Play();
		m_HighComboEffectRight.GetComponent<ParticleSystem>().Play();
        m_HighComboStripEffectLeft.GetComponent<ParticleSystem>().Play();
        m_HighComboStripEffectRight.GetComponent<ParticleSystem>().Play();
    }

	void DestroyComboStrip()
	{
		if ( m_CurrentHighComboStrip == null )
			return;

		m_CurrentHighComboStrip.GetComponent<ComboMove>().StartExit( -m_HighComboStripPos.y );
		m_CurrentHighComboStrip = null;

		m_HighComboEffectLeft.GetComponent<ParticleSystem>().Stop();
		m_HighComboEffectRight.GetComponent<ParticleSystem>().Stop();
        m_HighComboStripEffectLeft.GetComponent<ParticleSystem>().Stop();
        m_HighComboStripEffectRight.GetComponent<ParticleSystem>().Stop();
    }

	void CreateComboSpecular()
	{
		GameObject currentHighComboStrip = (GameObject)Instantiate( m_HighComboSpecular, Vector3.zero, Quaternion.identity );
		currentHighComboStrip.transform.SetParent( m_GameCanvas.transform );
		currentHighComboStrip.transform.localPosition = m_HighComboSpecularPos;
		currentHighComboStrip.transform.localScale = m_HighComboSpecularScale;
		currentHighComboStrip.GetComponent<ComboMove>().StartAllTheWay( -m_HighComboSpecularPos.y );
		currentHighComboStrip.SetActive( true );
	}

	void UpdateGoldDrop()
	{
		if ( m_GoldObject == null )
		{
			m_GoldIntervalTimer += Time.deltaTime;
		}
		else
		{
			GoldDrop gd = m_GoldObject.GetComponent<GoldDrop>();

			// Check for linking
			if ( !gd.GetLink() && !m_bGameover && m_Link.Linking && CheckLinkLinked( m_GoldObject.GetComponentInChildren<BoxCollider2D>() ) )
			{
				gd.LinkGold( true );
                //ScaleLinkEffect( m_GoldObject.transform );
                m_GoldObject.transform.localScale = m_DefaultGoldScale;
                AudioManager.Instance.PlaySoundEvent(SOUNDID.GEM_LINK);
            }

			// Move gem
			// Linked gems don't move
			if ( !gd.GetLink() )
			{
				Vector3 pos = m_GoldObject.transform.position;
				pos.y -= m_fBaseGemDropSpeed * Time.deltaTime;
				m_GoldObject.transform.position = pos;

				// Check for gold in unlinkable zone
				if ( !gd.GetPetrify() && IsInUnlinkableZone( m_GoldObject.transform ) )
				{
					gd.PetrifyGold();
                    AudioManager.Instance.PlaySoundEvent(SOUNDID.GEM_DROPPED);
                }

				// Check out of screen
				if( pos.y < -m_HalfDimension.y )
				{
					DestroyGoldDrop();
				}
			}
		}
	}

	static void GoToScore()
	{
		SceneManager.LoadScene( "Score" );
	}

#if LINKIT_COOP
	public void OnNetworkDisconnect()
	{
		if ( !NetworkManager.IsConnected() )
			return;

		if ( m_nHealth <= LOW_HEALTH )
		{
			m_nHealth = 0;
			CheckGameOver();
			return;
		}

		m_nHealth = 0;
		CheckGameOver();
		m_ComboText.SetActive( false );
		m_PraiseText.SetActive( false );
		m_MultiplierText.SetActive( false );

		m_DisconnectText.SetActive( true );

		/*
		if ( !m_bOriginalPlayerOne )
		{
			for ( int i = 0; i < LANE_NUM; ++i )
			{
				m_Gems[i].Clear();
			}
			m_StonedGems.Clear();
			m_GemsToBeRemoved.Clear();
			m_GemsToBeDestroyed.Clear();
			m_NetworkGemsToBeDestroyed.Clear();
		}
		*/
	}
#endif	// LINKIT_COOP
}
