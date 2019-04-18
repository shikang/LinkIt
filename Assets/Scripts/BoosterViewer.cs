using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BoosterViewer : MonoBehaviour
{
	public GameObject m_pEntry;
    public GameObject m_pSkinEntry;
    public GameObject m_gScrollRect;
	public GameObject m_uPlayerCoins;
	public GameObject m_pHeader;

	public List<GameObject> m_lViewer = new List<GameObject>();
	float m_fFirstPosX = 0.0f;
	float m_fFirstPosY = 593.0f;
	float m_fHorizontalGap = 90.0f;

	void Start ()
	{
		GameObject tmp;

		tmp = GameObject.Instantiate(m_pHeader);
		tmp.GetComponent<BoosterHeader>().SetText("SKINS");
		tmp.transform.parent = m_gScrollRect.transform;
		tmp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		tmp.GetComponent<Transform>().localPosition = new Vector3(m_fFirstPosX, m_fFirstPosY, 0.0f);
		m_lViewer.Add(tmp);
		m_fFirstPosY -= m_fHorizontalGap;

        for (int i = 0; i < (int)GemLibrary.GemSet.TOTAL; ++i)
        {
            tmp = GameObject.Instantiate(m_pSkinEntry);
            tmp.transform.parent = m_gScrollRect.transform;
            tmp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			tmp.GetComponent<Transform>().localPosition = new Vector3(m_fFirstPosX, m_fFirstPosY, 0.0f);
            m_lViewer.Add(tmp);
			m_fFirstPosY -= m_fHorizontalGap;
        }

		tmp = GameObject.Instantiate(m_pHeader);
		tmp.GetComponent<BoosterHeader>().SetText("ONE TIME BOOSTERS");
		tmp.transform.parent = m_gScrollRect.transform;
		tmp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		tmp.GetComponent<Transform>().localPosition = new Vector3(m_fFirstPosX, m_fFirstPosY, 0.0f);
		m_lViewer.Add(tmp);
		m_fFirstPosY -= m_fHorizontalGap;

        for (int i = 5; i <= 7; ++i)
		{
			tmp = GameObject.Instantiate(m_pEntry);
			tmp.transform.parent = m_gScrollRect.transform;
			tmp.transform.localScale = new Vector3(1.0f ,1.0f, 1.0f);
			tmp.GetComponent<Transform>().localPosition = new Vector3(m_fFirstPosX, m_fFirstPosY, 0.0f);
			m_lViewer.Add(tmp);
			m_fFirstPosY -= m_fHorizontalGap;
		}

		tmp = GameObject.Instantiate(m_pHeader);
		tmp.GetComponent<BoosterHeader>().SetText("UPGRADES");
		tmp.transform.parent = m_gScrollRect.transform;
		tmp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		tmp.GetComponent<Transform>().localPosition = new Vector3(m_fFirstPosX, m_fFirstPosY, 0.0f);
		m_lViewer.Add(tmp);
		m_fFirstPosY -= m_fHorizontalGap;

		for (int i = 0; i <= 4; ++i)
		{
			tmp = GameObject.Instantiate(m_pEntry);
			tmp.transform.parent = m_gScrollRect.transform;
			tmp.transform.localScale = new Vector3(1.0f ,1.0f, 1.0f);
			tmp.GetComponent<Transform>().localPosition = new Vector3(m_fFirstPosX, m_fFirstPosY, 0.0f);
			m_lViewer.Add(tmp);
			m_fFirstPosY -= m_fHorizontalGap;
		}
	}

    public void UpdateEntryBarGem(GemLibrary.GemSet type_)
    {
        m_lViewer[(int)type_+1].GetComponent<SkinEntry>().SetEntry(type_);
    }

    public void UpdateEntryBarEquip(BOOSTERTYPE type_, int currLevel_)
	{
		int difference = 9;		// position starts at 9, ID is 0. So, diff is 9.
		m_lViewer[(int)type_+difference].GetComponent<BoosterEntry>().SetEntry(type_, BoosterManager.Instance.GetBoostData(type_), currLevel_, false);
	}

	public void UpdateEntryBarBoost(BOOSTERTYPE type_, int currLevel_)
	{
		int difference = 0;	// position starts at 5, ID is 5. So, diff is 0.
		m_lViewer[(int)type_+difference].GetComponent<BoosterEntry>().SetEntry(type_, BoosterManager.Instance.GetBoostData(type_), currLevel_, true);
	}

	void Update ()
	{
        for (int i = 0; i < (int)GemLibrary.GemSet.TOTAL; ++i)
        {
            UpdateEntryBarGem((GemLibrary.GemSet)i);
        }

		UpdateEntryBarBoost(BOOSTERTYPE.ScoreMult_Once, 0);
		UpdateEntryBarBoost(BOOSTERTYPE.GoldMult_Once, 0);
		UpdateEntryBarBoost(BOOSTERTYPE.MoreHealth_Once, 0);

		UpdateEntryBarEquip(BOOSTERTYPE.ScoreMult, GameData.Instance.m_Boost_ScoreMult);
		UpdateEntryBarEquip(BOOSTERTYPE.GoldMult, GameData.Instance.m_Boost_GoldMult);
		UpdateEntryBarEquip(BOOSTERTYPE.Shield, GameData.Instance.m_Boost_Shield);
		UpdateEntryBarEquip(BOOSTERTYPE.SlowerGems, GameData.Instance.m_Boost_SlowerGems);
		UpdateEntryBarEquip(BOOSTERTYPE.BiggerGems, GameData.Instance.m_Boost_BiggerGems);

		m_uPlayerCoins.GetComponent<Text>().text = GameData.Instance.m_Coin.ToString();
	}
}

