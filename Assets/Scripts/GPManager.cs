using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class GPManager : MonoBehaviour
{
	// Singleton pattern
	static GPManager instance;
	public static GPManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if (instance != null)
			throw new System.Exception("You have more than 1 GPManager in the scene.");

		// Initialize the static class variables
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		/*PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();

		LCGoogleLoginBridge.ChangeLoggingLevel(true);
		LCGoogleLoginBridge.InitWithClientID ("166665260343-nlc4ttki8juo6m5igh9cbj3khm5pfeha.apps.googleusercontent.com");
		SignIn();*/
	}

	void SignIn()
    {
    	//Debug.Log("Start Auth");
		//PlayGamesPlatform.Instance.Authenticate((bool success) => { });
		//Debug.Log("End Auth");

		/*Action<bool> logInCallBack = (Action<bool>)((loggedIn)=> {
			if(loggedIn){
				Debug.Log("Google Login Success> " + LCGoogleLoginBridge.GSIUserName()); 
			}

			else{
				Debug.Log("Google Login Failed");
			}	
		});*/

		//LCGoogleLoginBridge.LoginUser (logInCallBack, false);
    }

	public static void ShowAchievementsUI()
    {
		Debug.Log("Show Achievements");
        Social.ShowAchievementsUI();
    }

	public static void ShowLeaderboardsUI()
    {
		Debug.Log("Show Leaderboards");
        Social.ShowLeaderboardUI();
    }

    public static void UnlockAchievement(string id)
    {
        Social.ReportProgress(id, 100, success => { });
    }
 
    public static void IncrementAchievement(string id, int stepsToIncrement)
    {
        //PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
    }
 
    public static void AddScoreToLeaderboard(string leaderboardId, long score)
    {
        Social.ReportScore(score, leaderboardId, success => { });
    }
 
    
}
