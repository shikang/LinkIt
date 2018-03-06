using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BoosterViewer : MonoBehaviour
{
	public GameObject m_pEntry;
	public GameObject m_gScrollRect;
	public GameObject m_uPlayerCoins;

	List<GameObject> m_lViewer = new List<GameObject>();
	float m_fFirstPosX = 0.0f;
	float m_fFirstPosY = 300.0f;
	float m_fHorizontalGap = 85.0f;

	void Start ()
	{
		GameObject tmp;
		for(int i = 0; i < (int)BOOSTERTYPE.Count; ++i)
		{
			tmp = GameObject.Instantiate(m_pEntry);
			tmp.transform.parent = m_gScrollRect.transform;
			tmp.transform.localScale = new Vector3(1.0f ,1.0f, 1.0f);
			tmp.GetComponent<Transform>().localPosition = new Vector3(m_fFirstPosX, m_fFirstPosY - i*m_fHorizontalGap, 0.0f);
			m_lViewer.Add(tmp);
		}
	}

	public void UpdateEntryBar(BOOSTERTYPE type_, int currLevel_, bool isOnce_)
	{
		int shiftEntry = 3;	// noof temp boosters
		if(isOnce_)
			shiftEntry = -5;	// no of perm boosters
		m_lViewer[(int)type_+shiftEntry].GetComponent<BoosterEntry>().SetEntry(type_, BoosterManager.Instance.GetBoostData(type_), currLevel_, isOnce_);
	}

	void Update ()
	{
		UpdateEntryBar(BOOSTERTYPE.ScoreMult_Once, 0, true);
		UpdateEntryBar(BOOSTERTYPE.GoldMult_Once, 0, true);
		UpdateEntryBar(BOOSTERTYPE.MoreHealth_Once, 0, true);

		UpdateEntryBar(BOOSTERTYPE.ScoreMult, GameData.Instance.m_Boost_ScoreMult, false);
		UpdateEntryBar(BOOSTERTYPE.GoldMult, GameData.Instance.m_Boost_GoldMult, false);
		UpdateEntryBar(BOOSTERTYPE.Shield, GameData.Instance.m_Boost_Shield, false);
		UpdateEntryBar(BOOSTERTYPE.SlowerGems, GameData.Instance.m_Boost_SlowerGems, false);
		UpdateEntryBar(BOOSTERTYPE.BiggerGems, GameData.Instance.m_Boost_BiggerGems, false);

		m_uPlayerCoins.GetComponent<Text>().text = GameData.Instance.m_Coin.ToString();
	}
}

