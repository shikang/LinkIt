using UnityEngine;
using System.Collections;

public class PlatformUtilies : Singleton<PlatformUtilies>
{
    const string ANDRIOD_BUNDLE_NAME = "com.WhyWool.LinkItTest";
    const string IOS_BUNDLE_NAME = "com.WhyWool.LinkIt";
    const string FACEBOOK_PAGE_ID = "1825892051004820";
    const string FACEBOOK_PAGE = "https://m.facebook.com/whywool/";

    protected PlatformUtilies()
    {
        m_NaviToFB = false;
        m_Timer = 0.0f;
    }

    bool m_NaviToFB;
    float m_Timer = 0.0f;

    public void DisplayRateUs()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + ANDRIOD_BUNDLE_NAME);
#elif UNITY_IPHONE
		Application.OpenURL("https://itunes.apple.com/sg/app/linkit-game/id1455092714?mt=8");
#endif
    }

    public void Update()
    {
        if (m_NaviToFB)
        {
            m_Timer += Time.deltaTime;

            if (m_Timer >= 1.0f)
            {
                Application.OpenURL(FACEBOOK_PAGE);
                m_NaviToFB = false;
                m_Timer = 0.0f;
            }
        }
    }

    public void DisplayFacebookPage()
    {
        //Application.OpenURL("fb://profile/" + FACEBOOK_PAGE_ID);
        //Application.OpenURL("fb://facewebmodal/f?href=" + FACEBOOK_PAGE);
        Application.OpenURL(FACEBOOK_PAGE);
        m_NaviToFB = true;
        m_Timer = 0.0f;

        if ( !GameData.Instance.m_bUnlock_Share_FB )
        {
            GameData.Instance.m_bUnlock_Share_FB = true;
            //SaveLoad.Save();
            SaveDataLoader.SaveGame();
        }
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        m_NaviToFB = false;
    }
}