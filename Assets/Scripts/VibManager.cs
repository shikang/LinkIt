using UnityEngine;
using System.Collections;

public enum VIBSTR
{
	STARTGAME = 0,
	SMALL,
	BIG
};

public class VibManager : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
	public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
	public static AndroidJavaClass unityPlayer;
	public static AndroidJavaObject currentActivity;
	public static AndroidJavaObject vibrator;
#endif

	int [] m_vStrengths;

	// Singleton pattern
	static VibManager instance;
	public static VibManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if (instance != null)
		{
			//Destroy (this.gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		m_vStrengths = new int[3];
		m_vStrengths [(int)VIBSTR.STARTGAME] = 150;
		m_vStrengths [(int)VIBSTR.SMALL] = 80;
		m_vStrengths [(int)VIBSTR.BIG] = 300;
	}

	public void StartVib(VIBSTR v)
	{
		if(!GameData.Instance.m_CanVibrate || VibrationManager.HasVibrator ())
			return;
		
		Vibrate(m_vStrengths[(int)v]);
	}

	public static void Vibrate()
	{
		if (isAndroid())
			vibrator.Call("vibrate");
	}

	public static void Vibrate(long milliseconds)
	{
		if (isAndroid())
			vibrator.Call("vibrate", milliseconds);
	}

	public static bool HasVibrator()
	{
		return isAndroid();
	}

	public static void Cancel()
	{
		if (isAndroid())
			vibrator.Call("cancel");
	}

	private static bool isAndroid()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		return true;
#else
		return false;
#endif
	}
}

