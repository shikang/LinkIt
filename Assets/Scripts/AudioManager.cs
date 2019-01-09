using UnityEngine;
using System.Collections;

public enum SOUNDID
{
	CLICK = 0,
	GACHACOUNT,
	BGM,
	STOPBGM
};

public class AudioManager : MonoBehaviour
{
	int gachaSwitch;
	bool hasSFX;
	bool hasBGM;

	// Singleton pattern
	static AudioManager instance;
	public static AudioManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if (instance != null)
			throw new System.Exception("You have more than 1 AudioManager in the scene.");

		// Initialize the static class variables
		instance = this;
	}

	void Start()
	{
		gachaSwitch = 1;
		//hasBGM = GameData.current.hasBGM;
		//hasSFX = GameData.current.hasSFX;
	}

	void Update()
	{
	}

	public void PlaySoundEvent(SOUNDID sid, GameObject go = null)
	{
		if(!hasSFX)
			return;

		if(!go)
			go = gameObject;

		switch(sid)
		{
		case SOUNDID.CLICK:
			VibrationManager.Vibrate(10);
			SoundEventWrapper("Click", go);
			break;

		case SOUNDID.GACHACOUNT:
			SoundEventWrapper("GachaCount", go);
			//AkSoundEngine.SetSwitch("GachaCount", UpdateGachaSwitch(), go);
			break;

		case SOUNDID.BGM:
			SoundEventWrapper("PlayBGM", go);
			//GlobalScript.Instance.isBGMPlaying = true;
			break;

		case SOUNDID.STOPBGM:
			SoundEventWrapper("StopBGM", go);
			//GlobalScript.Instance.isBGMPlaying = false;
			break;

		default:
			Debug.Log("Sound ID " + go + " does not exist");
			break;
		}
	}

	void SoundEventWrapper(string s, GameObject go)
	{
		//AkSoundEngine.PostEvent(s, go);
	}

	public void ToggleBGM()
	{
	}

	public void ToggleSFX()
	{
		hasSFX = !hasSFX;
		//SaveLoad.Save();
		SaveDataLoader.SaveGame();
	}

	public void SetMasterVol(float vol)
	{
		//AkSoundEngine.SetRTPCValue("MasterVol", vol);
	}

	public void SetBGMVol(float vol)
	{
		//AkSoundEngine.SetRTPCValue("BGMVol", vol);
	}

	public void SetSFXVol(float vol)
	{
		//AkSoundEngine.SetRTPCValue("SFXVol", vol);
	}

	string UpdateGachaSwitch()
	{
		if(gachaSwitch == 3)
			gachaSwitch = 1;
		else
			++gachaSwitch;

		return "Gacha" + gachaSwitch;
	}

	//AkSoundEngine.SetState("GameState", "MainMenu");
}

