using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayService : MonoBehaviour
{
	static bool s_Initialised = false;

	// Use this for initialization
	void Start ()
	{
		if ( !s_Initialised )
		{
			/*
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			// enables saving game progress.
			.EnableSavedGames()
			// registers a callback to handle game invitations received while the game is not running.
			.WithInvitationDelegate(< callback method >)
			// registers a callback for turn based match notifications received while the
			// game is not running.
			.WithMatchDelegate(< callback method >)
			// requests the email address of the player be available.
			// Will bring up a prompt for consent.
			.RequestEmail()
			// requests a server auth code be generated so it can be passed to an
			//  associated back end server application and exchanged for an OAuth token.
			.RequestServerAuthCode(false)
			// requests an ID token be generated.  This OAuth token can be used to
			//  identify the player to other services such as Firebase.
			.RequestIdToken()
			.Build();

			PlayGamesPlatform.InitializeInstance(config);
			*/
			// recommended for debugging:
			PlayGamesPlatform.DebugLogEnabled = true;
			// Activate the Google Play Games platform
			PlayGamesPlatform.Activate();
			Debug.Log( "PlayGamesPlatform.Activate()" );

			// authenticate user:
			Social.localUser.Authenticate((bool success) => {
				// handle success or failure
				Debug.Log( "Social.localUser.Authenticate success");
			});

			s_Initialised = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowAchievementUI()
	{
		Debug.Log( "Social.ShowAchievementsUI()");
		// show achievements UI
		Social.ShowAchievementsUI();
	}
}
