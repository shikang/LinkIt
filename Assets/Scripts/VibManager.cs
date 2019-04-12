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
		
		VibrationManager.Vibrate(m_vStrengths[(int)v]);
	}
}

