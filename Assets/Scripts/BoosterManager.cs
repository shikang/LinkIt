using UnityEngine;
using System.Collections;

public enum BOOSTERTYPE
{
	ScoreMult = 0,
	GoldMult,
	Shield,
	SlowerGems,
	BiggerGems,

	ScoreMult_Once,
	GoldMult_Once,
	MoreHealth_Once,

	//LinkAnyGems,
	//FasterBoosterCharge,
	//LongerBoosterDuration,

	Count
}

public struct BOOSTERDATA
{
	public string title;
	public string desc;
	public int cost;
	public float value;
}

public class BoosterManager : MonoBehaviour
{
	BOOSTERDATA [,] m_uBoosters = new BOOSTERDATA[5, 6];	// 5 types of perma boosters
	BOOSTERDATA [] m_uBoostersOnce = new BOOSTERDATA[3];

	// Singleton pattern
	static BoosterManager instance;
	public static BoosterManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if (instance != null)
			throw new System.Exception("You have more than 1 BoosterManager in the scene.");

		// Initialize the static class variables
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		LoadData();
	}

	public void ResetBoosterOnce()
	{
		GameData.Instance.m_Boost_ScoreMultOnce = false;
		GameData.Instance.m_Boost_GoldMultOnce = false;
		GameData.Instance.m_Boost_MoreHealthOnce = false;
		SaveLoad.Save();
	}

	public float GetScoreMultOnce()
	{
		if(GameData.Instance.m_Boost_ScoreMultOnce)
			return 1.5f;
		return 1.0f;
	}

	public float GetGoldMultOnce()
	{
		if(GameData.Instance.m_Boost_GoldMultOnce)
			return 1.2f;
		return 1.0f;
	}

	public float GetMoreHealthOnce()
	{
		if(GameData.Instance.m_Boost_MoreHealthOnce)
			return 1.25f;
		return 1.0f;
	}

	public float GetBoostValue(BOOSTERTYPE bType_)
	{
		switch(bType_)
		{
		case BOOSTERTYPE.ScoreMult:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_ScoreMult].value;
		case BOOSTERTYPE.GoldMult:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_GoldMult].value;
		case BOOSTERTYPE.Shield:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_Shield].value;
		case BOOSTERTYPE.SlowerGems:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_SlowerGems].value;
		case BOOSTERTYPE.BiggerGems:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_BiggerGems].value;
		default:
			return 1.0f;
		}
	}

	public BOOSTERDATA GetBoostData(BOOSTERTYPE bType_)
	{
		switch(bType_)
		{
		case BOOSTERTYPE.ScoreMult:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_ScoreMult];
		case BOOSTERTYPE.GoldMult:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_GoldMult];
		case BOOSTERTYPE.Shield:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_Shield];
		case BOOSTERTYPE.SlowerGems:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_SlowerGems];
		case BOOSTERTYPE.BiggerGems:
			return m_uBoosters[(int)bType_, GameData.Instance.m_Boost_BiggerGems];
		case BOOSTERTYPE.ScoreMult_Once:
			return m_uBoostersOnce[0];
		case BOOSTERTYPE.GoldMult_Once:
			return m_uBoostersOnce[1];
		case BOOSTERTYPE.MoreHealth_Once:
			return m_uBoostersOnce[2];
		default:
			return m_uBoosters[0, 0];
		}
	}

	void LoadData()
	{
		int maxLevel = 6;

		m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 0].value = 1.0f;	m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 0].cost = 0;
		m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 1].value = 1.2f;	m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 1].cost = 200;
		m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 2].value = 1.4f;	m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 2].cost = 400;
		m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 3].value = 1.6f;	m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 3].cost = 600;
		m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 4].value = 1.8f;	m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 4].cost = 800;
		m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 5].value = 2.0f;	m_uBoosters[(int)BOOSTERTYPE.ScoreMult, 5].cost = 1000;

		for(int i = 0; i < maxLevel; ++i)
		{
			m_uBoosters[(int)BOOSTERTYPE.ScoreMult, i].title = "Score Multiplier";
			m_uBoosters[(int)BOOSTERTYPE.ScoreMult, i].desc = "Get " + m_uBoosters[(int)BOOSTERTYPE.ScoreMult, i].value.ToString("G") + "x more score!";
		}

		m_uBoosters[(int)BOOSTERTYPE.GoldMult, 0].value = 1.0f;	m_uBoosters[(int)BOOSTERTYPE.GoldMult, 0].cost = 0;
		m_uBoosters[(int)BOOSTERTYPE.GoldMult, 1].value = 1.2f;	m_uBoosters[(int)BOOSTERTYPE.GoldMult, 1].cost = 200;
		m_uBoosters[(int)BOOSTERTYPE.GoldMult, 2].value = 1.4f;	m_uBoosters[(int)BOOSTERTYPE.GoldMult, 2].cost = 400;
		m_uBoosters[(int)BOOSTERTYPE.GoldMult, 3].value = 1.6f;	m_uBoosters[(int)BOOSTERTYPE.GoldMult, 3].cost = 600;
		m_uBoosters[(int)BOOSTERTYPE.GoldMult, 4].value = 1.8f;	m_uBoosters[(int)BOOSTERTYPE.GoldMult, 4].cost = 800;
		m_uBoosters[(int)BOOSTERTYPE.GoldMult, 5].value = 2.0f;	m_uBoosters[(int)BOOSTERTYPE.GoldMult, 5].cost = 1000;

		for(int i = 0; i < maxLevel; ++i)
		{
			m_uBoosters[(int)BOOSTERTYPE.GoldMult, i].title = "Gold Multiplier";
			m_uBoosters[(int)BOOSTERTYPE.GoldMult, i].desc = "Get " + m_uBoosters[(int)BOOSTERTYPE.GoldMult, i].value.ToString("G") + "x more gold!";
		}

		m_uBoosters[(int)BOOSTERTYPE.Shield, 0].value = 1.0f;	m_uBoosters[(int)BOOSTERTYPE.Shield, 0].cost = 0;
		m_uBoosters[(int)BOOSTERTYPE.Shield, 1].value = 0.9f;	m_uBoosters[(int)BOOSTERTYPE.Shield, 1].cost = 200;
		m_uBoosters[(int)BOOSTERTYPE.Shield, 2].value = 0.8f;	m_uBoosters[(int)BOOSTERTYPE.Shield, 2].cost = 400;
		m_uBoosters[(int)BOOSTERTYPE.Shield, 3].value = 0.7f;	m_uBoosters[(int)BOOSTERTYPE.Shield, 3].cost = 600;
		m_uBoosters[(int)BOOSTERTYPE.Shield, 4].value = 0.6f;	m_uBoosters[(int)BOOSTERTYPE.Shield, 4].cost = 800;
		m_uBoosters[(int)BOOSTERTYPE.Shield, 5].value = 0.5f;	m_uBoosters[(int)BOOSTERTYPE.Shield, 5].cost = 1000;

		for(int i = 0; i < maxLevel; ++i)
		{
			m_uBoosters[(int)BOOSTERTYPE.Shield, i].title = "Shield";
			m_uBoosters[(int)BOOSTERTYPE.Shield, i].desc = "Take " + m_uBoosters[(int)BOOSTERTYPE.Shield, i].value.ToString("G") + "x less damage!";
		}

		m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 0].value = 1.00f;	m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 0].cost = 0;
		m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 1].value = 0.95f;	m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 1].cost = 200;
		m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 2].value = 0.90f;	m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 2].cost = 400;
		m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 3].value = 0.85f;	m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 3].cost = 600;
		m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 4].value = 0.80f;	m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 4].cost = 800;
		m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 5].value = 0.75f;	m_uBoosters[(int)BOOSTERTYPE.SlowerGems, 5].cost = 1000;

		for(int i = 0; i < maxLevel; ++i)
		{
			m_uBoosters[(int)BOOSTERTYPE.SlowerGems, i].title = "Slower Gem Speed";
			m_uBoosters[(int)BOOSTERTYPE.SlowerGems, i].desc = "Gems move " + m_uBoosters[(int)BOOSTERTYPE.SlowerGems, i].value.ToString("G") + "x slower!";
		}

		m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 0].value = 1.0f;	m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 0].cost = 0;
		m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 1].value = 1.1f;	m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 1].cost = 200;
		m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 2].value = 1.2f;	m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 2].cost = 400;
		m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 3].value = 1.3f;	m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 3].cost = 600;
		m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 4].value = 1.4f;	m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 4].cost = 800;
		m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 5].value = 1.5f;	m_uBoosters[(int)BOOSTERTYPE.BiggerGems, 5].cost = 1000;

		for(int i = 0; i < maxLevel; ++i)
		{
			m_uBoosters[(int)BOOSTERTYPE.BiggerGems, i].title = "Bigger Gems";
			m_uBoosters[(int)BOOSTERTYPE.BiggerGems, i].desc = "Gems become " + m_uBoosters[(int)BOOSTERTYPE.BiggerGems, i].value.ToString("G") + "x bigger!";
		}

		m_uBoostersOnce[0].title = "2x Score (Single Use)";
		m_uBoostersOnce[0].desc = "Get 2x score for your next run!";
		m_uBoostersOnce[0].cost = 300;

		m_uBoostersOnce[1].title = "2x Gold (Single Use)";
		m_uBoostersOnce[1].desc = "Get 2x gold for your next run!";
		m_uBoostersOnce[1].cost = 300;

		m_uBoostersOnce[2].title = "1.5x HP (Single Use)";
		m_uBoostersOnce[2].desc = "Get 1.5x more health for your next run!";
		m_uBoostersOnce[2].cost = 200;
	}
}

