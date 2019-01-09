using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.SavedGame;
using System;

public class GooglePlayService : MonoBehaviour
{
#if UNITY_ANDROID
	private enum GooglePlayState
	{
		NONE,
		START_AUTH,
		AUTHENTICATING,
		CHECK_AUTH,
		FAIL_AUTH,
		SHOWING_ACHIEVEMENT,
		SHOWING_LEADERBOARD,
	};

	private static bool s_Initialised = false;
	private static bool s_Loaded = false;
    private static int s_Retry = 0;

	GooglePlayState m_State;
	GooglePlayState m_QueueState;

	// Use this for initialization
	void Start ()
	{
		Initialise();
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch ( m_State )
		{
			case GooglePlayState.START_AUTH:
				Authenticate();
				break;
			case GooglePlayState.AUTHENTICATING:
				// Nothing
				break;
			case GooglePlayState.CHECK_AUTH:
				if ( IsAuthenticated() )
				{
					EnableLoadingOverlay( false );
					ResumeState();
				}
				else
				{
					// Wait
				}
				break;
			case GooglePlayState.FAIL_AUTH:
				if ( m_QueueState != GooglePlayState.NONE )
				{
					// Show popup
					Popup popup = Popup.GetPopup();
					if ( popup )
					{
						popup.ShowPopup( "Fail to connect to Google Play." );
					}

					m_QueueState = GooglePlayState.NONE;
				}

				m_State = GooglePlayState.NONE;
				break;
			case GooglePlayState.SHOWING_ACHIEVEMENT:
				ShowAchievementUI();
				break;
			case GooglePlayState.SHOWING_LEADERBOARD:
				ShowLeaderboardUI();
				break;
			default:
				// Idle
				break;
		}
	}

	public void StartShowAchievementUI()
	{
		m_State = GooglePlayState.SHOWING_ACHIEVEMENT;
	}

	void ShowAchievementUI()
	{
		if (!IsAuthenticated())
		{
			PauseState(GooglePlayState.START_AUTH);
			Authenticate();
		}
		else
		{
			Debug.Log("Social.ShowAchievementsUI()");
			MainMenuManager.DisableButtons();
			PlayGamesPlatform.Instance.ShowAchievementsUI(EnableButtons);

			m_State = GooglePlayState.NONE;
		}
	}

	public static void UnlockAcheivement( string achievementID )
	{
		ProgressAcheivement( achievementID, 0.0f );
	}

	// 0.0f to 100.0f
	public static void ProgressAcheivement( string achievementID, float progress )
	{
		Social.ReportProgress( achievementID, progress, ( bool success ) => 
		{
			// handle success or failure
			Debug.Log( "Social.localUser.ReportProgress success - " + success );
		} );
	}

	public void StartShowLeaderboardUI()
	{
		m_State = GooglePlayState.SHOWING_LEADERBOARD;
	}

	void ShowLeaderboardUI()
	{
		if (!IsAuthenticated())
		{
			PauseState(GooglePlayState.START_AUTH);
			Authenticate();
		}
		else
		{
			Debug.Log("Social.ShowLeaderboardUI()");
			MainMenuManager.DisableButtons();
			PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_high_score, EnableButtons);

			m_State = GooglePlayState.NONE;
		}
	}

	public static void PostHighScore( int highscore )
	{
		Social.ReportScore( highscore, GPGSIds.leaderboard_high_score, ( bool success ) => {
			// handle success or failure
			Debug.Log( "Social.localUser.ReportScore success - " + success );
		} );
	}

	public static void SaveGame()
	{
		OpenSavedGame( "linkit.gd", SaveAfterOpen );

		// Backup save
		SaveLoad.Save();
	}

	public static void LoadGame()
	{
		OpenSavedGame( "linkit.gd", LoadAfterOpen );
	}

	public static void SaveAfterOpen( SavedGameRequestStatus status, ISavedGameMetadata game )
	{
		if ( status == SavedGameRequestStatus.Success )
		{
			// handle reading or writing of saved game.
			Debug.Log( "Saving game after open file" );
			SaveGame( game );
		}
		else
		{
			// handle error
			Debug.Log( "Fail to open file" );
		}
	}

	public static void LoadAfterOpen( SavedGameRequestStatus status, ISavedGameMetadata game )
	{
		if ( status == SavedGameRequestStatus.Success )
		{
			// handle reading or writing of saved game.
			Debug.Log( "Load game after open file" );
			LoadGameData( game );
		}
		else
		{
			// handle error
			Debug.Log( "Fail to open file, loading from backup" );
			SaveLoad.Load();
		}

		s_Loaded = true;
	}

	public static bool IsLoaded()
	{
		return s_Loaded;
	}

    public static int RetryNum()
    {
        return s_Retry;
    }

	public static void OpenSavedGame( string filename, Action<SavedGameRequestStatus, ISavedGameMetadata> callback )
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution( filename, DataSource.ReadCacheOrNetwork,
			ConflictResolutionStrategy.UseLongestPlaytime, callback );
	}

	static void LoadGameData( ISavedGameMetadata game )
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData( game, OnSavedGameDataRead );
	}

	public static void OnSavedGameDataRead( SavedGameRequestStatus status, byte[] data )
	{
		if ( status == SavedGameRequestStatus.Success )
		{
			// handle processing the byte array data
			SaveLoad.LoadFromByteArray( data );
			Debug.Log( "Game data loaded" );
		}
		else
		{
			// Fail to load, read from file instead
			Debug.Log( "Fail to load game data, loading from backup" );
			SaveLoad.Load();
		}
	}

	static void SaveGame( ISavedGameMetadata game )
	{
		byte[] savedData = SaveLoad.ToByteArray();

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

		SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
		builder = builder
			.WithUpdatedDescription( "Saved game at " + DateTime.Now );
		
		SavedGameMetadataUpdate updatedMetadata = builder.Build();
		savedGameClient.CommitUpdate( game, updatedMetadata, savedData, OnSavedGameWritten );
	}

	public static void OnSavedGameWritten( SavedGameRequestStatus status, ISavedGameMetadata game )
	{
		if ( status == SavedGameRequestStatus.Success )
		{
			// handle reading or writing of saved game.
			Debug.Log( "Game data saved!" );
		}
		else
		{
			// handle error
			Debug.Log( "Failed to save game data" );
		}
	}

	static void EnableButtons( UIStatus status )
	{
		Debug.Log( "UIStatus: " + status );
		MainMenuManager.EnableButtons();
	}

	void Initialise()
	{
		if ( !s_Initialised )
		{
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			// enables saving game progress.
			//.EnableSavedGames()
			// registers a callback to handle game invitations received while the game is not running.
			//.WithInvitationDelegate(< callback method >)
			// registers a callback for turn based match notifications received while the
			// game is not running.
			//.WithMatchDelegate(< callback method >)
			// requests the email address of the player be available.
			// Will bring up a prompt for consent.
			//.RequestEmail()
			// requests a server auth code be generated so it can be passed to an
			//  associated back end server application and exchanged for an OAuth token.
			//.RequestServerAuthCode( false )
			// requests an ID token be generated.  This OAuth token can be used to
			//  identify the player to other services such as Firebase.
			//.RequestIdToken()
			.Build();

			PlayGamesPlatform.InitializeInstance( config );
			
			// recommended for debugging:
			PlayGamesPlatform.DebugLogEnabled = true;
			// Activate the Google Play Games platform
			PlayGamesPlatform.Activate();

			s_Initialised = true;
			m_State = GooglePlayState.START_AUTH;
			m_QueueState = GooglePlayState.NONE;
		}
	}

	void Authenticate()
	{
		if ( IsAuthenticated() )
			return;

		Debug.Log( "Social.localUser.Authenticate" );

		EnableLoadingOverlay( true );
		m_State = GooglePlayState.AUTHENTICATING;

		// authenticate user:
		Social.localUser.Authenticate( ( bool success ) => 
		{
			// handle success or failure
			Debug.Log( "Social.localUser.Authenticate success - " + success );
			if ( success )
			{
				m_State = GooglePlayState.CHECK_AUTH;
			}
			else
			{
				m_State = GooglePlayState.FAIL_AUTH;
                ++s_Retry;
                EnableLoadingOverlay( false );
			}
		} );
	}

	GameObject GetLoadingOverlay()
	{
		return GameObject.FindGameObjectWithTag( "Loading Overlay" );
	}

	void EnableLoadingOverlay( bool enable )
	{
		GameObject overlay = GetLoadingOverlay();
		if ( overlay != null )
		{
			overlay.SetActive( enable );
		}
	}

	public static bool IsAuthenticated()
	{
		return Social.localUser.authenticated;
	}

	void ResumeState()
	{
		m_State = m_QueueState;
		m_QueueState = GooglePlayState.NONE;
	}

	void PauseState( GooglePlayState goState )
	{
		m_QueueState = m_State;
		m_State = goState;
	}
#endif
}
