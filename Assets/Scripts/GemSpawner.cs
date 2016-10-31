using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GemSpawner : MonoBehaviour
{
	public static Vector3 FRONT_OFFSET = Vector3.back;

	// Spawning constants
	public const float GAME_START_DELAY = 3.0f;			//!< In seconds
	public const int LANE_NUM = 5;
	public const int LOOKBACK_NUM = 3;
	public const float BASE_SPAWN_RATE = 1.0f;			//!< In seconds
	public const float BASE_GEM_DROP_TIME = 5.0f;       //!< In seconds
	public const float SPAWN_RATE_GROWTH = 0.06f;       //!< In seconds
	public const float GEM_DROP_TIME_GROWTH = 0.1f;     //!< In seconds

	// Type constants
	public const int INVALID_LANE = -1;
	public const int INVALID_GEM = -1;

	// Animation constants
	public const float ANIMATION_INTERVAL = 2.0f;		//!< In seconds
	public const float ANIMATION_RATE = 0.125f;         //!< In seconds

	public const float TIME_TO_ACTUAL_POINTS = 0.5f;    //!< In seconds

	public const float GAMEOVER_ANIMATION = 2.0f;		//!< In seconds

	// Game constants
	public const float UNLINKABLE_ZONE = 0.1f;          //!< Percentage from bottom

	public const float LINKED_SCALE_FACTOR = 1.25f;

	public const int PER_GEM_POINTS = 100;

	public const int MAX_LEVEL = ( ( int )SpawnPattern.DifficultyLevel.Total - 1 ) * 3;

	public const int HEALTH_LOST_PER_GEM = 10;
	public const int HEALTH_GAIN_PER_LINK = 1;
	public const int MAX_HEALTH = 100;

	bool m_bGameStart = false;

	// Link
	public GameObject m_LinkPrefab;
	private GameObject m_LinkObject;
	public Color[] m_LinkColours;
	public Color m_NeutralColor;
	private Link m_Link;
	private List<Gem> m_LinkedGem;

	// Spawning information
	public GameObject[] m_aGemList;                     //!< Prefab to create from
	public GameObject[] m_aRepels;						//!< Prefab for repel
	private int m_nGemTypeNum = 0;						//!< Number of gem types
	private Vector3 m_HalfDimension;
	private float m_fLaneWidth;
	private float m_fBaseGemDropSpeed;

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

	// Spawning interval
	private float m_fSpawnRate = BASE_SPAWN_RATE;
	private float m_fSpawnTimer = 0.0f;

	// Spawning live information (updating...)
	private int[] m_aGemCount;											//!< Keep track of gem count sorted by color
	private int m_nTotalGemCount = 0;
	private List< List<GameObject> > m_Gems;							//!< Gems in game
	private List<GameObject> m_StonedGems;								//!< Unlinkable gems
	private List<GameObject> m_GemsToBeRemoved;							//!< Gems to be removed after a loop
	private List<GameObject> m_GemsToBeDestroyed;						//!< Gems to be destroyed after a loop
	private List<GameObject> m_NetworkGemsToBeDestroyed;				//!< Delayed destroy because the other player could be linking it

	struct DestroyedNetworkInfo
	{
		public Vector2 m_Pos;
		public int m_GemType;

		public DestroyedNetworkInfo( Vector2 pos, int gemType ) { m_Pos = pos; m_GemType = gemType; }
	}
	private Dictionary<int, DestroyedNetworkInfo> m_DestroyedNetworkInfo;//!< Network gem destroyed pos

	// Life line variables
	public Sprite[] m_StoneSprites;
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

	// Player stats
	private int m_nLevel = 0;
	private int m_nPoints = 0;
	private int m_nShowingPoints = 0;
	private int m_nPrevPoints = 0;
	private float m_nPointTimer = 0.0f;
	private int m_nHealth = MAX_HEALTH;
	private PlayerStatistics m_PlayerStats;
	private bool m_bGameover = false;

	// Gameover
	private float m_fGameoverTimer = 0.0f;

	// Texts
	public GameObject m_GameCanvas;
	public GameObject m_ScoreText;
	public GameObject m_LevelText;		//!< Debug
	public GameObject m_HealthText;		//!< Debug
	public GameObject m_PointsGain;

	NetworkGameLogic m_Network = null;

	// Use this for initialization
	void Start ()
	{
		m_bGameStart = false;

		// Initialise link
		CreateLink();
		m_Link = m_LinkObject.GetComponent<Link>();
		m_LinkedGem = new List<Gem>();

		// Get spawning information
		m_HalfDimension = Camera.main.ScreenToWorldPoint( new Vector3( ( Screen.width ), ( Screen.height ) ) );
		m_fLaneWidth = m_HalfDimension.x * 2.0f / LANE_NUM;
		//m_fBaseGemDropSpeed = m_HalfDimension.y * 2.0f / BASE_GEM_DROP_TIME;

		m_nGemTypeNum = m_aGemList.Length;

		// Initialise spawning history variables
		m_PreviousLanes = Enumerable.Repeat( INVALID_LANE, LOOKBACK_NUM ).ToList();
		//m_PreviousGems = new List<int>();
		m_SpawnPattern = GetComponent<SpawnPattern>();
		m_lSequence = new List<int>();
		m_mSequenceGemTypeMap = new List<int>();
		m_nSequenceIndex = 0;
		m_nSequenceIndexFromList = -1;

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
		if ( NetworkManager.IsConnected() && NetworkManager.IsPlayerOne() )
			m_NetworkGemsToBeDestroyed = new List<GameObject>();
		if ( NetworkManager.IsConnected() && !NetworkManager.IsPlayerOne() )
			m_DestroyedNetworkInfo = new Dictionary<int, DestroyedNetworkInfo>();

		// Initialise life line
		if ( m_LineLine != null )
		{
			m_LineLine.transform.position = new Vector3( 0.0f, ( UNLINKABLE_ZONE * m_HalfDimension.y * 2.0f ) + -m_HalfDimension.y, 0.0f ) + FRONT_OFFSET;
		}

		// Initialising animation timer
		m_nFrameNum = m_StoneSprites.Length;
		for ( int i = 0; i < m_aGemList.Length; ++i )
		{
			int num = m_aGemList[i].GetComponent<GemSpriteContainer>().m_Sprites.Length;
			m_nFrameNum = m_nFrameNum > num ? num : m_nFrameNum;
		}
		m_fAnimationIntervalTimer = 0.0f;
		m_fAnimationTimer = 0.0f;
		m_bAnimating = false;
		m_nAnimatingFrame = -1;

		// Initialise player's stats
		m_nLevel = 0;
		m_nPoints = 0;
		m_nShowingPoints = 0;
		m_nPrevPoints = 0;
		m_nPointTimer = 0.0f;
		m_nHealth = MAX_HEALTH;
		m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
		m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();
		m_fSpawnRate = BASE_SPAWN_RATE - m_nLevel * SPAWN_RATE_GROWTH;
		m_fBaseGemDropSpeed = m_HalfDimension.y * 2.0f / (BASE_GEM_DROP_TIME - (m_nLevel / 2) * GEM_DROP_TIME_GROWTH);

		m_PlayerStats = GameObject.FindGameObjectWithTag( "Player Statistics" ).GetComponent<PlayerStatistics>();
		m_PlayerStats.m_aGems = m_aGemList;
		m_PlayerStats.m_StoneSprites = m_StoneSprites;
		m_PlayerStats.m_aDestroyCount = new int [m_aGemList.Length];
		for (int i = 0; i < m_aGemList.Length; ++i)
		{
			m_PlayerStats.m_aDestroyCount[i] = 0;
		}

		// Gameover
		m_bGameover = false;
		m_fGameoverTimer = 0.0f;

		// Transition Overlay
		GameObject transition = GameObject.FindGameObjectWithTag( "Transition" );
		transition.transform.position += 5.0f * FRONT_OFFSET;

		if ( NetworkManager.IsConnected() )
		{
			m_Network = GameObject.Find( "Network Manager" ).GetComponent<NetworkGameLogic>();
		}
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
		Spawn();

		if ( Input.GetMouseButtonDown( 0 ) )
			m_Link.StartLink();

		if ( m_Link.Linking )
			m_Link.DrawLink();

		UnlinkGems();
		UpdateGems();
		AnimateGems();
		AnimatePoints();

		CheckGameOver();
		UpdateGameover();
	}

	// -------------------------------- Updating functions --------------------------------------------
	void Spawn()
	{
		if ( NetworkManager.IsConnected() && !NetworkManager.IsPlayerOne() )
			return;

		m_fSpawnTimer += Time.deltaTime;

		if ( !m_bGameStart )
		{
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
			CreateGem( GetRandomLane(), GetRandomGem() );
		}
	}

	void UpdateGems()
	{
		bool connected = NetworkManager.IsConnected();

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

				// Other player's linked gem
				if ( connected && g.GetComponent<NetworkGem>().OtherLinked )
				{
					continue;
				}

				// Check for linking
				if ( m_Link.Linking && LinkGem( m_Gems[i][j] ) )
				{
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

		// Let player 1 destroy it and minus health
		if ( !NetworkManager.IsConnected() || NetworkManager.IsPlayerOne() )
		{
			int healthReduce = m_GemsToBeDestroyed.Count * HEALTH_LOST_PER_GEM;
			// RPC health
			if ( m_GemsToBeDestroyed.Count > 0 )
			{
				// Ensure both side die together
				m_Network.UpdateHealth( m_nHealth <= healthReduce ? -MAX_HEALTH : -healthReduce );
			}
				

			m_nHealth -= healthReduce;
			m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
			m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();
			for ( int k = 0; k < m_GemsToBeDestroyed.Count; ++k )
			{
				DestroyGem( m_GemsToBeDestroyed[k].GetComponent<Gem>() );
			}
			m_GemsToBeDestroyed.Clear();
		}
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

				int frame = m_nAnimatingFrame % m_nFrameNum;

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
							gem.GetComponent<SpriteRenderer>().sprite = m_StoneSprites[frame];
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

		Vector3 dir = m_Link.CurrentPoint - m_Link.PreviousPoint;
		float length = dir.magnitude;
		bool linked = false;

		if ( length >= float.Epsilon )
		{
			dir.Normalize();

			Ray ray = new Ray( m_Link.PreviousPoint, dir );
			float distance = 0.0f;
			bool intersected = linkCollider.bounds.IntersectRay( ray, out distance );
			linked = intersected ? distance <= length : false;
		}
		else
		{
			// Test for point inside bound
			linked = linkCollider.bounds.Contains( m_Link.CurrentPoint );
		}

		// Link logic
		if ( linked )
		{
			// Linkable
			if ( want )
			{
				SetLinkGemEffect( g );

				m_LinkedGem.Add( g );

				m_Link.ChangeLinkColor( g.GemType );

				if ( NetworkManager.IsConnected() && !NetworkManager.IsPlayerOne() )
					m_Network.LinkNetworkGem( g, true ); //, ( g.transform.position.y - -m_HalfDimension.y ) / ( m_HalfDimension.y * 2.0f ) );
			}
			// Not linkable
			else
			{
				m_Link.BreakLink();

				// Repel effect
				CreateRepel( g );

				// RPC create repel
				if ( NetworkManager.IsConnected() )
				{
					m_Network.CreateRepel( g );
				}
			}
		}

		return linked;
	}

	public void SetLinkGemEffect( Gem gem )
	{
		gem.Linked = true;
		gem.GetComponent<SpriteRenderer>().sprite = gem.GetComponent<GemSpriteContainer>().m_GlowSprites[0];
		gem.transform.localScale = new Vector3( LINKED_SCALE_FACTOR, LINKED_SCALE_FACTOR, 1.0f );
	}

	public void UnsetLinkGemEffect( Gem gem )
	{
		gem.Linked = false;
		gem.GetComponent<SpriteRenderer>().sprite = gem.GetComponent<GemSpriteContainer>().m_Sprites[0];
		gem.transform.localScale = Vector3.one;
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
	}

	int GetPointsGain( int num )
	{
		int pointsGain = num * PER_GEM_POINTS + num * ( ( num - 3 ) * ( PER_GEM_POINTS / 10 ) );
		return pointsGain;
	}

	void StartRollingPoints( int pointsGain )
	{
		m_nPointTimer = 0.0f;
		m_nPrevPoints = m_nPoints;
		m_nPoints += pointsGain;
	}

	bool UnlinkGems()
	{
		if ( !m_Link.CheckForDestroy )
			return false;

		bool destroy = m_LinkedGem.Count >= 3;
		if ( destroy )
		{
			// Points
			int pointsGain = GetPointsGain( m_LinkedGem.Count );
			int eachGain = pointsGain / m_LinkedGem.Count;
			StartRollingPoints( pointsGain );
			m_PlayerStats.m_nScore = m_nPoints;

			m_nHealth += HEALTH_GAIN_PER_LINK + ( m_LinkedGem.Count - 3 ) * HEALTH_GAIN_PER_LINK;
			m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
			m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();

			// Level
			if ( GetLevelUpRequiredPoints( m_nLevel ) <= m_nPoints )
			{
				m_nLevel++;
				m_LevelText.GetComponent<Text>().text = m_nLevel.ToString();

				if ( m_nLevel <= MAX_LEVEL )
				{
					//m_fSpawnRate -= SPAWN_RATE_GROWTH;
					m_fSpawnRate = BASE_SPAWN_RATE - m_nLevel * SPAWN_RATE_GROWTH;
					m_fBaseGemDropSpeed = m_HalfDimension.y * 2.0f / ( BASE_GEM_DROP_TIME - ( m_nLevel / 2 ) * GEM_DROP_TIME_GROWTH );

					Debug.Log("Spawn rate: " + m_fSpawnRate);
					Debug.Log("Drop rate: " + m_fBaseGemDropSpeed);
				}

				// level up animation
				GameObject levelup = ( GameObject )Instantiate( m_LevelUpOverlayPrefab, Vector3.zero, Quaternion.identity );
				ParticleSystem ps = levelup.GetComponentInChildren<ParticleSystem>();
				Destroy( levelup, LevelUpAnimation.ANIM_TIME + ps.startLifetime + Time.deltaTime );
			}

			foreach ( Gem g in m_LinkedGem )
			{
				m_PlayerStats.m_aDestroyCount[g.GemType]++;

				AddGainPointsEffect( g.transform.position, g.GemType, eachGain );
			}
		}

		// Multiplayer logic
		if ( NetworkManager.IsConnected() )
		{
			if ( destroy )
			{
				if ( NetworkManager.IsPlayerOne() )
				{
					// RPC particle effect
					m_Network.InformGemsDestroyed( m_LinkedGem );
				}
				else
				{
					// RPC destroy gem
					m_Network.DestroyNetworkGems( m_LinkedGem );
				}
			}
			else
			{
				if ( !NetworkManager.IsPlayerOne() && m_LinkedGem.Count > 0 )
				{
					// RPC unlink gem
					m_Network.UnlinkNetworkGems( m_LinkedGem );
				}
			}
		}

		foreach ( Gem g in m_LinkedGem )
		{
			UnlinkGem( g, destroy );
		}

		m_LinkedGem.Clear();
		m_Link.CheckForDestroy = false;

		return true;
	}

	int GetLevelUpRequiredPoints( int nLevel )
	{
		//return ( nLevel + 1 ) * 2000 + ( nLevel * nLevel * 1000 );
		return ( nLevel + 1 ) * 250 + ( nLevel * nLevel * 100 );
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
		// Get the next sequence
		if ( m_lSequence.Count <= m_nSequenceIndex )
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
		GameObject gem = null;
		if ( NetworkManager.IsConnected() )
		{
			object [] data = new object[3];
			data[0] = lane;
			data[1] = gemType;
			data[2] = m_nSequenceIndexFromList;
			gem = PhotonNetwork.Instantiate( m_aGemList[gemType].gameObject.name, new Vector3( GetGemX( lane ), m_HalfDimension.y ), Quaternion.identity, 0, data );
		}
		else
		{
			gem = ( GameObject )Instantiate( m_aGemList[gemType], new Vector3( GetGemX( lane ), m_HalfDimension.y ), Quaternion.identity );
			Destroy ( gem.GetComponent<PhotonView>() );
			Destroy ( gem.GetComponent<NetworkGem>() );
		}
		gem.GetComponent<Gem>().Lane = lane;
		gem.GetComponent<Gem>().GemType = gemType;
		gem.GetComponent<Gem>().SequenceIndex = m_nSequenceIndexFromList;

		// Updating live info
		m_aGemCount[gemType]++;
		m_nTotalGemCount++;
		m_Gems[lane].Add( gem );
	}

	public void AddNetworkGem( NetworkGem ng )
	{
		Gem gem = ng.GetComponent<Gem>();
		int lane = gem.Lane;
		//int gemType = gem.GemType;

		// Updating live info
		//m_aGemCount[gemType]++;
		//m_nTotalGemCount++;
		m_Gems[lane].Add( gem.gameObject );
	}

	public void RemoveNetworkGem( Gem gem, bool byMyself )
	{
		if ( gem.Petrified )
		{
			m_StonedGems.Remove( gem.gameObject );
		}
		else
		{
			bool contains = m_Gems[gem.Lane].Contains( gem.gameObject );

			if( contains )
			{
				m_Gems[gem.Lane].Remove( gem.gameObject );

				// For lookup
				if ( !byMyself )
				{
					int id = gem.GetComponent<PhotonView>().instantiationId;
					if ( m_DestroyedNetworkInfo.ContainsKey( id ) )
					{
						m_DestroyedNetworkInfo.Remove( id );
					}

					Vector2 pos = new Vector2( gem.transform.position.x, gem.transform.position.y );
					m_DestroyedNetworkInfo.Add( id, new DestroyedNetworkInfo( pos, gem.GetComponent<Gem>().GemType ) );
				}
			}
		}
	}

	public void UnlinkNetworkGems( string[] ids )
	{
		foreach ( string id in ids )
		{
			LinkNetworkGem( Convert.ToInt32( id ), false );
		}
	}

	public void LinkNetworkGem( int id, bool link )
	{
		for ( int i = 0; i < LANE_NUM; ++i )
		{
			for ( int j = 0; j < m_Gems[i].Count; ++j )
			{
				PhotonView p = m_Gems[i][j].GetComponent<PhotonView>();
				if ( p && p.instantiationId == id )
				{
					m_Gems[i][j].GetComponent<NetworkGem>().LinkNetworkGem( link );
					return;
				}
			}
		}

		for ( int j = 0; j < m_StonedGems.Count; ++j )
		{
			PhotonView p = m_StonedGems[j].GetComponent<PhotonView>();
			if ( p && p.instantiationId == id )
			{
				m_StonedGems[j].GetComponent<NetworkGem>().LinkNetworkGem( link );
				return;
			}
		}

		if ( !link )
		{
			GameObject dead = null;
			for ( int j = 0; j < m_NetworkGemsToBeDestroyed.Count; ++j )
			{
				PhotonView p = m_NetworkGemsToBeDestroyed[j].GetComponent<PhotonView>();
				if ( p && p.instantiationId == id )
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

	public void DestroyNetworkGems( string[] ids )
	{
		// Sychron point
		int pointsGain = GetPointsGain( ids.Length );
		int eachGain = pointsGain / ids.Length;
		StartRollingPoints( pointsGain );
		m_PlayerStats.m_nScore = m_nPoints;

		// Sychron health
		m_nHealth += HEALTH_GAIN_PER_LINK + ( ids.Length - 3 ) * HEALTH_GAIN_PER_LINK;
		m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
		m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();

		foreach ( string id in ids )
		{
			DestroyNetworkGem( Convert.ToInt32( id ), eachGain );
		}
	}

	public void DestroyNetworkGem( int id, int eachGain )
	{
		Gem g = null;
		for ( int i = 0; i < LANE_NUM; ++i )
		{
			for ( int j = 0; j < m_Gems[i].Count; ++j )
			{
				PhotonView p = m_Gems[i][j].GetComponent<PhotonView>();
				if ( p && p.instantiationId == id )
				{
					g = m_Gems[i][j].GetComponent<Gem>();
					break;
				}
			}
		}

		if ( g == null )
		{
			for ( int j = 0; j < m_StonedGems.Count; ++j )
			{
				PhotonView p = m_StonedGems[j].GetComponent<PhotonView>();
				if ( p && p.instantiationId == id )
				{
					g = m_StonedGems[j].GetComponent<Gem>();
					break;
				}
			}

			if ( g != null )
			{
				m_StonedGems.Remove( g.gameObject );
				m_Gems[g.Lane].Add( g.gameObject );
			}
		}

		if ( g == null )
		{
			for ( int j = 0; j < m_NetworkGemsToBeDestroyed.Count; ++j )
			{
				PhotonView p = m_NetworkGemsToBeDestroyed[j].GetComponent<PhotonView>();
				if ( p && p.instantiationId == id )
				{
					g = m_NetworkGemsToBeDestroyed[j].GetComponent<Gem>();
					break;
				}
			}

			if ( g != null )
			{
				m_NetworkGemsToBeDestroyed.Remove( g.gameObject );
				m_Gems[g.Lane].Add( g.gameObject );
			}
		}

		if ( g != null )
		{
			UnlinkGem( g, true );

			// Particle effect
			AddGainPointsEffect( g.transform.position, g.GemType, eachGain );
		}
	}

	public void NetworkAddGainPointsEffects( string[] ids )
	{
		// Sychron point
		int pointsGain = GetPointsGain( ids.Length );
		int eachGain = pointsGain / ids.Length;
		StartRollingPoints( pointsGain );
		m_PlayerStats.m_nScore = m_nPoints;

		// Sychron health
		m_nHealth += HEALTH_GAIN_PER_LINK + ( ids.Length - 3 ) * HEALTH_GAIN_PER_LINK;
		m_HealthText.GetComponent<Text>().text = m_nHealth.ToString();
		m_LineLine.GetComponent<SpriteRenderer>().color = GetLifeLineColour();

		foreach ( string id in ids )
		{
			NetworkAddGainPointsEffect( Convert.ToInt32( id ), eachGain );
		}
	}

	public void NetworkAddGainPointsEffect( int id, int eachGain )
	{
		if ( m_DestroyedNetworkInfo.ContainsKey( id ) )
		{
			DestroyedNetworkInfo info = m_DestroyedNetworkInfo[id];

			// Particle effect
			AddGainPointsEffect( info.m_Pos, info.m_GemType, eachGain );

			m_DestroyedNetworkInfo.Remove( id );
		}
		else
		{
			Gem g = null;
			for ( int i = 0; i < LANE_NUM; ++i )
			{
				for ( int j = 0; j < m_Gems[i].Count; ++j )
				{
					PhotonView p = m_Gems[i][j].GetComponent<PhotonView>();
					if ( p && p.instantiationId == id )
					{
						g = m_Gems[i][j].GetComponent<Gem>();
						break;
					}
				}
			}

			if ( g == null )
			{
				for ( int j = 0; j < m_StonedGems.Count; ++j )
				{
					PhotonView p = m_StonedGems[j].GetComponent<PhotonView>();
					if ( p && p.instantiationId == id )
					{
						g = m_StonedGems[j].GetComponent<Gem>();
						break;
					}
				}

				if ( g != null )
				{
					m_StonedGems.Remove( g.gameObject );
					m_Gems[g.Lane].Add( g.gameObject );
				}
			}

			if ( g != null )
			{
				UnlinkGem( g, true );

				// Particle effect
				AddGainPointsEffect( g.transform.position, g.GemType, eachGain );
			}
		}
	}

	public void CreateNetworkRepel( int id )
	{
		for ( int i = 0; i < LANE_NUM; ++i )
		{
			for ( int j = 0; j < m_Gems[i].Count; ++j )
			{
				PhotonView p = m_Gems[i][j].GetComponent<PhotonView>();
				if ( p && p.instantiationId == id )
				{
					CreateRepel( m_Gems[i][j].GetComponent<Gem>() );
					return;
				}
			}
		}

		for ( int j = 0; j < m_StonedGems.Count; ++j )
		{
			PhotonView p = m_StonedGems[j].GetComponent<PhotonView>();
			if ( p && p.instantiationId == id )
			{
				CreateRepel( m_StonedGems[j].GetComponent<Gem>() );
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
	}

	public void PetrifyGem( Gem gem )
	{
		m_aGemCount[gem.GemType]--;
		m_nTotalGemCount--;
		m_Gems[gem.Lane].Remove( gem.gameObject );
		m_StonedGems.Add( gem.gameObject );
		gem.GetComponent<SpriteRenderer>().sprite = m_StoneSprites[0];
		gem.Petrified = true;

		m_lFailedSequenceCount[gem.GetComponent<Gem>().SequenceIndex]++;
	}

	public void DestroyGem( Gem gem )
	{
		m_StonedGems.Remove( gem.gameObject );

		if ( NetworkManager.IsConnected() && NetworkManager.IsPlayerOne() )
		{
			NetworkGem ng = gem.GetComponent<NetworkGem>();
			if ( ng != null && ng.OtherLinked )
			{
				m_NetworkGemsToBeDestroyed.Add( gem.gameObject );
				return;
			}
		}

		DestroyGemImpl( gem );
	}

	public void UnlinkGem( Gem gem, bool destroy )
	{
		if ( destroy )
		{ 
			m_Gems[gem.Lane].Remove( gem.gameObject );

			DestroyGemImpl( gem );
		}
		else
		{
			UnsetLinkGemEffect( gem );
		}
	}

	public bool IsGemStoned( Gem gem )
	{
		Transform t = gem.transform;
		Renderer r = gem.GetComponent<SpriteRenderer>();
		return ( t.position.y + r.bounds.size.y <= m_LineLine.transform.position.y ) ||
			   ( t.position.y <= ( ( m_fGameoverTimer / GAMEOVER_ANIMATION ) * m_HalfDimension.y * 2.0f ) + -m_HalfDimension.y );
	}

	void AnimatePoints()
	{
		m_nPointTimer += Time.deltaTime;

		if ( m_nPointTimer < TIME_TO_ACTUAL_POINTS + Time.deltaTime )
		{
			m_nPointTimer = m_nPointTimer > TIME_TO_ACTUAL_POINTS ? TIME_TO_ACTUAL_POINTS : m_nPointTimer;
			m_nShowingPoints = ( int )( ( m_nPointTimer / TIME_TO_ACTUAL_POINTS ) * ( m_nPoints - m_nPrevPoints ) ) + m_nPrevPoints;
			m_ScoreText.GetComponent<Text>().text = m_nShowingPoints.ToString();
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
		GameObject repel = ( GameObject )Instantiate( m_aRepels[g.GemType], g.transform.position + FRONT_OFFSET, Quaternion.identity );
		repel.transform.SetParent( g.transform );
		Destroy( repel, RepelAnimator.LIFETIME );
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

	void CheckGameOver()
	{
		if ( !m_bGameover && m_nHealth <= 0 )
		{
			m_bGameover = true;
			m_Link.BreakLink();
			m_Link.CheckForDestroy = false;
			foreach ( Gem g in m_LinkedGem )
			{
				UnlinkGem( g, false );
			}
		}
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
	}

	void DestroyGemImpl( Gem gem )
	{
		if ( NetworkManager.IsConnected() )
		{
			// Check for player 1
			if ( NetworkManager.IsPlayerOne() )
			{
				PhotonNetwork.Destroy( gem.gameObject );
			}
			else
			{
				RemoveNetworkGem( gem, true );
			}
		}
		else
		{
			Destroy( gem.gameObject );
		}
	}

	static void GoToScore()
	{
		SceneManager.LoadScene( "Score" );
	}
}
