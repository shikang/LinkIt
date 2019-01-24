using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AchievementViewer : MonoBehaviour
{
	public GameObject m_pAchEntryPrefab;
	public GameObject m_gScrollRect;

	List<GameObject> m_lAchViewer = new List<GameObject>();
	float m_fFirstPosX = 0.0f;
	float m_fFirstPosY = 485.0f;
	float m_fHorizontalGap = 90.0f;

	void Start ()
	{
		GameObject tmp;
		for(int i = 0; i < AchievementManager.Instance.m_noofAchTypes; ++i)
		{
			tmp = GameObject.Instantiate(m_pAchEntryPrefab);
			tmp.transform.parent = m_gScrollRect.transform;
			tmp.transform.localScale = new Vector3(1.0f ,1.0f, 1.0f);
			tmp.GetComponent<Transform>().localPosition = new Vector3(m_fFirstPosX, m_fFirstPosY - i*m_fHorizontalGap, 0.0f);
			m_lAchViewer.Add(tmp);
		}
	}

	public void UpdateEntryBar(int aID, ACHIEVEMENTSET tmp, int currPoints)
	{
		m_lAchViewer[aID].GetComponent<AchievementEntry>().SetEntry(tmp.title, tmp.desc, currPoints, tmp.count);
	}

	void Update ()
	{
		UpdateEntryBar(0, AchievementManager.Instance.m_array_TotalGamesPlayed[GameData.Instance.m_next_TotalGamesPlayed], GameData.Instance.m_curr_TotalGamesPlayed);
		UpdateEntryBar(1, AchievementManager.Instance.m_array_TotalCoinsEarned[GameData.Instance.m_next_TotalCoinsEarned], GameData.Instance.m_curr_TotalCoinsEarned);
		UpdateEntryBar(2, AchievementManager.Instance.m_array_TotalScoreEarned[GameData.Instance.m_next_TotalScoreEarned], GameData.Instance.m_curr_TotalScoreEarned);

		UpdateEntryBar(3, AchievementManager.Instance.m_array_TotalLinkedGems_R[GameData.Instance.m_next_TotalLinkedGems_R], GameData.Instance.m_curr_TotalLinkedGems_R);
		UpdateEntryBar(4, AchievementManager.Instance.m_array_TotalLinkedGems_G[GameData.Instance.m_next_TotalLinkedGems_G], GameData.Instance.m_curr_TotalLinkedGems_G);
		UpdateEntryBar(5, AchievementManager.Instance.m_array_TotalLinkedGems_B[GameData.Instance.m_next_TotalLinkedGems_B], GameData.Instance.m_curr_TotalLinkedGems_B);
		UpdateEntryBar(6, AchievementManager.Instance.m_array_TotalLinkedGems_Y[GameData.Instance.m_next_TotalLinkedGems_Y], GameData.Instance.m_curr_TotalLinkedGems_Y);

		UpdateEntryBar(7, AchievementManager.Instance.m_array_MaxCombo[GameData.Instance.m_next_MaxCombo], GameData.Instance.m_curr_MaxCombo);
		UpdateEntryBar(8, AchievementManager.Instance.m_array_PerGameCoinsEarned[GameData.Instance.m_next_PerGameCoinsEarned], GameData.Instance.m_curr_PerGameCoinsEarned);
		UpdateEntryBar(9, AchievementManager.Instance.m_array_PerGameScoreEarned[GameData.Instance.m_next_PerGameScoreEarned], GameData.Instance.m_curr_PerGameScoreEarned);

		UpdateEntryBar(10, AchievementManager.Instance.m_RecoverToPerfectFromRed, GameData.Instance.m_RecoverToPerfectFromRed);
		UpdateEntryBar(11, AchievementManager.Instance.m_array_LinkGemsInOneChain[GameData.Instance.m_next_LinkGemsInOneChain], GameData.Instance.m_curr_TotalGamesPlayed);
	}
}