using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayService : MonoBehaviour
{
	private static bool s_Initialised = false;
	private static bool s_Auth = false;

	private bool m_bShowingAchievement = false;

	// Use this for initialization
	void Start ()
	{
		Initialise();
		
		m_bShowingAchievement = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ( s_Auth )
		{
			Debug.Log("Social.ShowAchievementsUI()");
			// show achievements UI
			Social.ShowAchievementsUI();
		}
	}

	public void ShowAchievementUI()
	{
		if ( !s_Auth )
		{
			Debug.Log( "Social.localUser.Authenticate" );
			Authenticate();
		}

		m_bShowingAchievement = true;
	}

	static void Initialise()
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
			.RequestServerAuthCode( false )
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
		}
	}

	static void Authenticate()
	{
		// authenticate user:
		Social.localUser.Authenticate( ( bool success ) => 
		{
			// handle success or failure
			Debug.Log( "Social.localUser.Authenticate success - " + success );
			if ( success )
			{
				SetAuthenticated();
			}
		} );
	}

	static void SetAuthenticated()
	{
		s_Auth = true;
	}
}
