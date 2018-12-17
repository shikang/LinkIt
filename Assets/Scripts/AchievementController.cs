using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void StartShowAchievementUI()
	{
#if UNITY_ANDROID
		GameObject playServiceObj = GameObject.Find( "Google Play Service" );
		if ( playServiceObj != null )
		{
			GooglePlayService service = playServiceObj.GetComponent<GooglePlayService>();
			if ( service != null )
			{
				service.StartShowAchievementUI();
			}
			else
			{
				Debug.Log( "GooglePlayService component can't be found" );
			}
		}
		else
		{
			Debug.Log("Google Play Service game object can't be found");
		}
#elif UNITY_IOS
		// @todo Show Apple achievement
#endif
	}

	public void StartShowLeaderboardUI()
	{
#if UNITY_ANDROID
		GameObject playServiceObj = GameObject.Find( "Google Play Service" );
		if ( playServiceObj != null )
		{
			GooglePlayService service = playServiceObj.GetComponent<GooglePlayService>();
			if ( service != null )
			{
				service.StartShowLeaderboardUI();
			}
			else
			{
				Debug.Log( "GooglePlayService component can't be found" );
			}
		}
		else
		{
			Debug.Log("Google Play Service game object can't be found");
		}
#elif UNITY_IOS
		// @todo Show Apple achievement
#endif
	}
}
