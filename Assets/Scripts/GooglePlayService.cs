using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayService : MonoBehaviour
{
	private enum GooglePlayState
	{
		NONE,
		START_AUTH,
		AUTHENTICATING,
		CHECK_AUTH,
		FAIL_AUTH,
		SHOWING_ACHIEVEMENT,
	};

	private static bool s_Initialised = false;

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
		if ( !IsAuthenticated() )
		{
			PauseState( GooglePlayState.START_AUTH );
			Authenticate();
		}
		else
		{
			Debug.Log( "Social.ShowAchievementsUI()" );
			Social.ShowAchievementsUI();

			m_State = GooglePlayState.NONE;
		}
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

	bool IsAuthenticated()
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
}
