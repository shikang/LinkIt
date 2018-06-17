using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class BoosterEntry : MonoBehaviour
{
	public BOOSTERTYPE m_Type;
	public int m_MaxLevel = 5;	// Assume 5 for now
	public int m_CurrLevel;
	int m_bCost;

	public GameObject m_goBtnFX;
	public GameObject m_Image;
	public GameObject m_Title;
	public GameObject m_Desc;
	public GameObject m_Cost;
	public GameObject m_LevelButton;
	public GameObject m_LevelText;
	public GameObject m_Bar;
	public GameObject m_BarFrame;
	public GameObject m_BarText;
	public GameObject m_Overlay;
	public GameObject m_OverlayText;
	public GameObject m_OverlayLock;

	bool isDimming = true;

	void Start ()
	{
	}

	void Update ()
	{
		if(m_Type == BOOSTERTYPE.ScoreMult_Once)
		{
			m_Overlay.SetActive(!GameData.Instance.m_bUnlock_Games);
			m_OverlayText.SetActive(!GameData.Instance.m_bUnlock_Games);
			m_OverlayLock.SetActive(!GameData.Instance.m_bUnlock_Games);
			m_OverlayText.GetComponent<Text>().text = "PLAY 5 GAMES TO UNLOCK!";
		}
		else if(m_Type == BOOSTERTYPE.GoldMult_Once)
		{
			m_Overlay.SetActive(!GameData.Instance.m_bUnlock_EarnPoints);
			m_OverlayText.SetActive(!GameData.Instance.m_bUnlock_EarnPoints);
			m_OverlayLock.SetActive(!GameData.Instance.m_bUnlock_EarnPoints);
			m_OverlayText.GetComponent<Text>().text = "GET 20,000 POINTS IN ONE GAME TO UNLOCK!";
		}
		else if(m_Type == BOOSTERTYPE.MoreHealth_Once)
		{
			m_Overlay.SetActive(!GameData.Instance.m_bUnlock_Share_FB);
			m_OverlayText.SetActive(!GameData.Instance.m_bUnlock_Share_FB);
			m_OverlayLock.SetActive(!GameData.Instance.m_bUnlock_Share_FB);
			m_OverlayText.GetComponent<Text>().text = "SHARE LINKIT WITH 3 FACEBOOK FRIENDS TO UNLOCK!";
		}

		if(m_OverlayText.GetActive())
		{
			Color tmp = m_OverlayText.GetComponent<Text>().color;
			if(isDimming)
			{
				tmp.a -= 0.01f;
				if(tmp.a <= 0.4f)
					isDimming = false;
			}
			else
			{
				tmp.a += 0.01f;
				if(tmp.a >= 1.0f)
					isDimming = true;
			}
			m_OverlayText.GetComponent<Text>().color = tmp;
		}

		if(GameData.Instance.m_Coin < m_bCost)
		{
			m_Cost.GetComponent<Text>().color = Color.red;
			m_LevelButton.GetComponent<Image> ().color = new Color (0.6f, 0.6f, 0.6f);
		}
		else
		{
			m_Cost.GetComponent<Text>().color = Color.black;
			m_LevelButton.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
		}
	}

	public void SetEntry(BOOSTERTYPE type_, BOOSTERDATA tmp_, int cLevel_, bool isOnce_)
	{
		m_Type = type_;
		m_Title.GetComponent<Text>().text = tmp_.title;
		m_Desc.GetComponent<Text>().text = tmp_.desc;
		m_Cost.GetComponent<Text>().text = tmp_.cost.ToString();
		SetButtonText();
		m_Overlay.SetActive(false);
		m_OverlayText.SetActive(false);
		m_bCost = tmp_.cost;

		if(isOnce_)
		{
			m_Bar.SetActive(false);
			m_BarFrame.SetActive(false);
			m_BarText.SetActive(false);
		}
		else
		{
			m_CurrLevel = cLevel_;
			SetLevelInfo();
		}
	}

	public void PressButton()
	{
		int boosterCost = BoosterManager.Instance.GetBoostData(m_Type).cost;

		if(m_Type == BOOSTERTYPE.ScoreMult_Once)
		{
			if(GameData.Instance.m_Boost_ScoreMultOnce)
			{
				RefundMoney(boosterCost);
				StartBtnClickFX();
			}
			else
			{
				if(BuyBooster(boosterCost))
				{
					GameData.Instance.m_Boost_ScoreMultOnce = !GameData.Instance.m_Boost_ScoreMultOnce;
					//SaveLoad.Save();
					StartBtnClickFX();
					SaveDataLoader.SaveGame();
				}
			}
		}
		else if(m_Type == BOOSTERTYPE.GoldMult_Once)
		{
			if(GameData.Instance.m_Boost_GoldMultOnce)
			{
				RefundMoney(boosterCost);
				StartBtnClickFX();
			}
			else
			{
				if(BuyBooster(boosterCost))
				{
					GameData.Instance.m_Boost_GoldMultOnce = !GameData.Instance.m_Boost_GoldMultOnce;
					//SaveLoad.Save();
					StartBtnClickFX();
					SaveDataLoader.SaveGame();
				}
			}
		}
		else if(m_Type == BOOSTERTYPE.MoreHealth_Once)
		{
			if(GameData.Instance.m_Boost_MoreHealthOnce)
			{
				RefundMoney(boosterCost);
				StartBtnClickFX();
			}
			else
			{
				if(BuyBooster(boosterCost))
				{
					GameData.Instance.m_Boost_MoreHealthOnce = !GameData.Instance.m_Boost_MoreHealthOnce;
					//SaveLoad.Save();
					StartBtnClickFX();
					SaveDataLoader.SaveGame();
				}
			}
		}
		else if(m_Type == BOOSTERTYPE.ScoreMult)
		{
			if(GameData.Instance.m_Boost_ScoreMult < m_MaxLevel)
			{
				if(BuyBooster(boosterCost))
				{
					GameData.Instance.m_Boost_ScoreMult += 1;
					//SaveLoad.Save();
					StartBtnClickFX();
					SaveDataLoader.SaveGame();
					Analytics.CustomEvent("BuyBoosterLvl", new Dictionary<string, object>
					{
						{"ScoreMult", GameData.Instance.m_Boost_ScoreMult}
					});
				}
			}
		}
		else if(m_Type == BOOSTERTYPE.GoldMult)
		{
			if(GameData.Instance.m_Boost_GoldMult < m_MaxLevel)
			{
				if(BuyBooster(boosterCost))
				{
					GameData.Instance.m_Boost_GoldMult += 1;
					//SaveLoad.Save();
					StartBtnClickFX();
					SaveDataLoader.SaveGame();
					Analytics.CustomEvent("BuyBoosterLvl", new Dictionary<string, object>
					{
						{"GoldMult", GameData.Instance.m_Boost_GoldMult}
					});
				}
			}
		}
		else if(m_Type == BOOSTERTYPE.Shield)
		{
			if(GameData.Instance.m_Boost_Shield < m_MaxLevel)
			{
				if(BuyBooster(boosterCost))
				{
					GameData.Instance.m_Boost_Shield += 1;
					//SaveLoad.Save();
					StartBtnClickFX();
					SaveDataLoader.SaveGame();
					Analytics.CustomEvent("BuyBoosterLvl", new Dictionary<string, object>
					{
						{"Shield", GameData.Instance.m_Boost_Shield}
					});
				}
			}
		}
		else if(m_Type == BOOSTERTYPE.SlowerGems)
		{
			if(GameData.Instance.m_Boost_SlowerGems < m_MaxLevel)
			{
				if(BuyBooster(boosterCost))
				{
					GameData.Instance.m_Boost_SlowerGems += 1;
					//SaveLoad.Save();
					StartBtnClickFX();
					SaveDataLoader.SaveGame();
					Analytics.CustomEvent("BuyBoosterLvl", new Dictionary<string, object>
					{
						{"SlowerGems", GameData.Instance.m_Boost_SlowerGems}
					});
				}
			}
		}
		else if(m_Type == BOOSTERTYPE.BiggerGems)
		{
			if(GameData.Instance.m_Boost_BiggerGems < m_MaxLevel)
			{
				if(BuyBooster(boosterCost))
				{
					GameData.Instance.m_Boost_BiggerGems += 1;
					//SaveLoad.Save();
					StartBtnClickFX();
					SaveDataLoader.SaveGame();
					Analytics.CustomEvent("BuyBoosterLvl", new Dictionary<string, object>
					{
						{"BiggerGems", GameData.Instance.m_Boost_BiggerGems}
					});
				}
			}
		}

		SetButtonText();
	}

	void SetButtonText()
	{
		m_LevelText.GetComponent<Text>().text = "GET!";

		m_LevelButton.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
		if(m_Type == BOOSTERTYPE.ScoreMult_Once)
		{
			if(GameData.Instance.m_Boost_ScoreMultOnce)
			{
				m_LevelText.GetComponent<Text>().text = "In Use";
				m_LevelButton.GetComponent<Image> ().color = new Color (1.0f, 0.87f, 0.0f); // gold
			}
		}
		else if(m_Type == BOOSTERTYPE.GoldMult_Once)
		{
			if(GameData.Instance.m_Boost_GoldMultOnce)
			{
				m_LevelText.GetComponent<Text>().text = "In Use";
				m_LevelButton.GetComponent<Image> ().color = new Color (1.0f, 0.87f, 0.0f); // gold
			}
		}
		else if(m_Type == BOOSTERTYPE.MoreHealth_Once)
		{
			if(GameData.Instance.m_Boost_MoreHealthOnce)
			{
				m_LevelText.GetComponent<Text>().text = "In Use";
				m_LevelButton.GetComponent<Image> ().color = new Color (1.0f, 0.87f, 0.0f); // gold
			}
		}
		else
		{
			if(m_CurrLevel < m_MaxLevel)
			{
				m_LevelText.GetComponent<Text>().text = "LEVEL UP";
			}
			else
			{
				m_LevelText.GetComponent<Text>().text = "MAXED!";
				m_LevelButton.GetComponent<Image> ().color = new Color (1.0f, 0.87f, 0.0f); // gold
			}
		}
	}

	void SetLevelInfo()
	{
		m_Bar.GetComponent<RectTransform>().localScale = new Vector3((float)m_CurrLevel/(float)m_MaxLevel, 1.0f, 1.0f);
		m_BarText.GetComponent<Text>().text = m_CurrLevel.ToString() + " / " + m_MaxLevel.ToString();
	}

	bool BuyBooster(int cost_)
	{
		if(GameData.Instance.m_Coin >= cost_)
		{
			GameData.Instance.m_Coin -= cost_;
			return true;
		}
		return false;
	}

	void RefundMoney(int cost_)
	{
		GameData.Instance.m_Coin += cost_;
	}

	void StartBtnClickFX()
	{
		GameObject go = (GameObject)GameObject.Instantiate(m_goBtnFX);
		go.transform.parent = m_LevelButton.transform;
		go.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		go.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
	}
}

