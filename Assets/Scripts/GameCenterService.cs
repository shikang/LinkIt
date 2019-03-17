using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.SocialPlatforms;
#endif

public class GameCenterService : MonoBehaviour
{
#if UNITY_IOS
    private enum GameCenterState
    {
        NONE,
        START_AUTH,
        AUTHENTICATING,
        CHECK_AUTH,
        FAIL_AUTH,
        SHOWING_LEADERBOARD,
    };

    private const string LEADERBOARD_ID = "HighScoreLeaderboard";

    private static bool s_Initialised = false;
    private static bool s_Loaded = false;
    private static int s_Retry = 0;

    GameCenterState m_State;
    GameCenterState m_QueueState;

    // Use this for initialization
    void Start ()
    {
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case GameCenterState.START_AUTH:
                Authenticate();
                break;
            case GameCenterState.AUTHENTICATING:
                // Nothing
                break;
            case GameCenterState.CHECK_AUTH:
                if (IsAuthenticated())
                {
                    EnableLoadingOverlay(false);
                    ResumeState();
                }
                else
                {
                    // Wait
                }
                break;
            case GameCenterState.FAIL_AUTH:
                if (m_QueueState != GameCenterState.NONE)
                {
                    // Show popup
                    Popup popup = Popup.GetPopup();
                    if (popup)
                    {
                        popup.ShowPopup("Fail to connect to Game Center.");
                    }

                    m_QueueState = GameCenterState.NONE;
                }

                m_State = GameCenterState.NONE;
                break;
            case GameCenterState.SHOWING_LEADERBOARD:
                ShowLeaderboardUI();
                break;
            default:
                // Idle
                break;
        }
    }

    public void StartShowLeaderboardUI()
	{
		m_State = GameCenterState.SHOWING_LEADERBOARD;
	}

	void ShowLeaderboardUI()
	{
		if (!IsAuthenticated())
		{
			PauseState(GameCenterState.START_AUTH);
			Authenticate();
		}
		else
		{
			Debug.Log("Social.ShowLeaderboardUI()");
			MainMenuManager.DisableButtons();
            Social.ShowLeaderboardUI();

			m_State = GameCenterState.NONE;
		}
	}

    public static void PostHighScore(int highscore)
    {
        Social.ReportScore(highscore, LEADERBOARD_ID, (bool success) => {
            // handle success or failure
            Debug.Log("Social.localUser.ReportScore success - " + success);
        });
    }

    public static bool IsAuthenticated()
    {
        return Social.localUser.authenticated;
    }

    void ResumeState()
    {
        m_State = m_QueueState;
        m_QueueState = GameCenterState.NONE;
    }

    void PauseState( GameCenterState goState )
	{
		m_QueueState = m_State;
		m_State = goState;
	}

    void Authenticate()
	{
		if ( IsAuthenticated() )
			return;

		Debug.Log( "Social.localUser.Authenticate" );

		EnableLoadingOverlay( true );
		m_State = GameCenterState.AUTHENTICATING;

		// authenticate user:
		Social.localUser.Authenticate( ( bool success ) => 
		{
			// handle success or failure
			Debug.Log( "Social.localUser.Authenticate success - " + success );
			if ( success )
			{
				m_State = GameCenterState.CHECK_AUTH;
			}
			else
			{
				m_State = GameCenterState.FAIL_AUTH;
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

    void Initialise()
    {
        if (!s_Initialised)
        {
            s_Initialised = true;
            m_State = GameCenterState.START_AUTH;
            m_QueueState = GameCenterState.NONE;
        }
    }
#endif
}
