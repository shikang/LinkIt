using UnityEngine;
using System.Collections;

public enum SOUNDID
{
	MENU_CLICK = 0,
	MENU_START,
	ACHIEVEMENT_GOT,
	SCORE_TICK,
	SCORE_TICK_STOP,

	GEM_LINK,
	GEM_LINK_FAIL,
	GEM_LINK_SUCCEED,
	GEM_DROPPED,
	GEM_TOUCHED,

	COMBO_TICK,
	COMBO_TICK_STOP,
	COMBO_LOST,

	FEVER_ENTER,
	FEVER_SUSTAIN,
	FEVER_EXIT,
	FEVER_SUSTAIN_STOP,

	BGM,
	BGM_STOP
};

public class AudioManager : MonoBehaviour
{
	int linkCount;

	// Singleton pattern
	static AudioManager instance;
	public static AudioManager Instance
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
		linkCount = 1;
		AkSoundEngine.SetRTPCValue("LinkCount", linkCount);
	}

	void Update()
	{
	}

	public void PlaySoundEvent(SOUNDID sid, GameObject go = null)
	{
		if(!go)
			go = gameObject;

		switch(sid)
		{
		case SOUNDID.MENU_CLICK:
			SoundEventWrapper("Menu_Click", go);
			break;

		case SOUNDID.MENU_START:
			VibrationManager.Vibrate(10);
			SoundEventWrapper("Menu_Start", go);
			UpdateBGM("InGame");
			break;

		case SOUNDID.ACHIEVEMENT_GOT:
			SoundEventWrapper("Achievement_Got", go);
			break;

		case SOUNDID.SCORE_TICK:
			SoundEventWrapper("Score_Tick", go);
			break;

		case SOUNDID.SCORE_TICK_STOP:
			SoundEventWrapper("Score_Tick_Stop", go);
			break;

		case SOUNDID.GEM_LINK:
			SoundEventWrapper ("Gem_Link", go);
			AkSoundEngine.SetRTPCValue("LinkCount", ++linkCount);
			break;

		case SOUNDID.GEM_LINK_FAIL:
			SoundEventWrapper ("Gem_Link_Fail", go);
			linkCount = 1;
			AkSoundEngine.SetRTPCValue ("LinkCount", linkCount);
			break;

		case SOUNDID.GEM_LINK_SUCCEED:
			SoundEventWrapper ("Gem_Link_Succeed", go);
			linkCount = 1;
			AkSoundEngine.SetRTPCValue ("LinkCount", linkCount);
			break;

		case SOUNDID.GEM_DROPPED:
			SoundEventWrapper("Gem_Dropped", go);
			break;

		case SOUNDID.GEM_TOUCHED:
			SoundEventWrapper("Gem_Touched", go);
			break;

		case SOUNDID.COMBO_LOST:
			SoundEventWrapper("Combo_Lost", go);
			break;

		case SOUNDID.COMBO_TICK:
			SoundEventWrapper("Combo_Tick", go);
			break;

		case SOUNDID.COMBO_TICK_STOP:
			SoundEventWrapper("Combo_Tick_Stop", go);
			break;

		case SOUNDID.FEVER_ENTER:
			SoundEventWrapper("Fever_Enter", go);
			break;

		case SOUNDID.FEVER_SUSTAIN:
			SoundEventWrapper("Fever_Sustain", go);
			break;

		case SOUNDID.FEVER_EXIT:
			SoundEventWrapper("Fever_Exit", go);
			break;

		case SOUNDID.FEVER_SUSTAIN_STOP:
			SoundEventWrapper("Fever_Sustain_Stop", go);
			break;

		case SOUNDID.BGM:
			SoundEventWrapper("BGM_Stop", go);
			SoundEventWrapper("BGM", go);
			break;

		case SOUNDID.BGM_STOP:
			SoundEventWrapper("BGM_Stop", go);
			break;

		default:
			Debug.Log("Sound ID " + go + " does not exist");
			break;
		}
	}

	void SoundEventWrapper(string s, GameObject go)
	{
		AkSoundEngine.PostEvent(s, go);
	}

	public void UpdateHighComboIndex(int index)
	{
		AkSoundEngine.SetRTPCValue("HighComboIndex", index);
	}

	public void UpdateBGM(string bgm)
	{
		AkSoundEngine.SetState("BGM_State", bgm);
	}
		
	public void SetBGMVol(float vol)
	{
		AkSoundEngine.SetRTPCValue("BGM_Vol", vol);
	}

	public void SetSFXVol(float vol)
	{
		AkSoundEngine.SetRTPCValue("SFX_Vol", vol);
	}
}

