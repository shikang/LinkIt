using UnityEngine;
using System.Collections;

public class SaveDataLoader : MonoBehaviour
{
	private bool m_startedLoading = false;
	private static bool m_loaded = false;

	// Use this for initialization
	void Start ()
	{
		m_startedLoading = false;
	}

	public static bool IsLoaded()
	{
		return m_loaded;
	}

	public static void LoadGame()
	{
#if UNITY_ANDROID
		//GooglePlayService.LoadGame();
		SaveLoad.Load();
#elif UNITY_IOS
		// @todo: IOS logic here
        SaveLoad.Load();
#endif
    }

    public static void SaveGame()
	{
#if UNITY_ANDROID
		//GooglePlayService.SaveGame();
		SaveLoad.Save();
#elif UNITY_IOS
		// @todo: IOS logic here
        SaveLoad.Save();
#endif
    }

    private static bool IsServiceAuthenticated()
	{
#if UNITY_ANDROID
		return GooglePlayService.IsAuthenticated();
#elif UNITY_IOS
		// @todo: IOS logic here
        return true;
#endif
	}

	private static bool IsServiceLoaded()
	{
#if UNITY_ANDROID
		//return GooglePlayService.IsLoaded()
		return true;
#elif UNITY_IOS
		// @todo: IOS logic here
        return true;
#endif
	}

    private static int ServiceNumRetryNum()
    {
#if UNITY_ANDROID
        //return GooglePlayService.IsLoaded()
        return GooglePlayService.RetryNum();
#elif UNITY_IOS
		// @todo: IOS logic here
        return 1;
#endif
    }

    void Update()
	{
		if ( !m_loaded && !m_startedLoading && ( IsServiceAuthenticated() || ServiceNumRetryNum() > 0 ) )
		{
			LoadGame();

			// For first time use
			if ( GameData.Instance.m_Sets.Count == 0 )
			{
				//GameData.Instance.m_Sets.Add( GemLibrary.GemSet.GEM );
				//GameData.Instance.m_GemList.Add( GemLibrary.GemSet.GEM );
				GameData.Instance.UnlockGem( GemLibrary.GemSet.GEM );
				GameData.Instance.m_EquippedGemSet = GemLibrary.GemSet.GEM;
				SaveGame();
			}

			m_startedLoading = true;
		}

		if ( !m_loaded && m_startedLoading && IsServiceLoaded() )
		{
			m_loaded = true;
			GameObject.Find( "Canvas" ).GetComponent<Splash>().CanFadeOut();
		}
	}
}
