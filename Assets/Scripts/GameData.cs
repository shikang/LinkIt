using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
	public static GameData Instance = new GameData();
	public int m_HighScore;
	public int m_HighestCombo;
	public int m_MultiplayerHighScore;
	public int m_MultiplayerHighestCombo;
	public int m_Coin;
	public GemLibrary.GemSet m_EquippedGemSet;
	public List<GemLibrary.GemSet> m_GemList;
	public bool m_SeenTutorial;

	// Booster Levels
	public bool m_Boost_ScoreMultOnce;
	public bool m_Boost_GoldMultOnce;
	public bool m_Boost_MoreHealthOnce;

	public int m_Boost_ScoreMult;
	public int m_Boost_GoldMult;
	public int m_Boost_Shield;
	public int m_Boost_SlowerGems;
	public int m_Boost_MoreGems;
	public int m_Boost_BiggerGems;

	// Achievements. _curr_ is current count, _next_ is next (target) index
	public int m_curr_TotalGamesPlayed;
	public int m_next_TotalGamesPlayed;
	public int m_curr_TotalCoinsEarned;
	public int m_next_TotalCoinsEarned;
	public int m_curr_TotalScoreEarned;
	public int m_next_TotalScoreEarned;

	public int m_curr_TotalLinkedGems_R;
	public int m_next_TotalLinkedGems_R;
	public int m_curr_TotalLinkedGems_B;
	public int m_next_TotalLinkedGems_B;
	public int m_curr_TotalLinkedGems_G;
	public int m_next_TotalLinkedGems_G;
	public int m_curr_TotalLinkedGems_Y;
	public int m_next_TotalLinkedGems_Y;

	public int m_curr_MaxCombo;
	public int m_next_MaxCombo;
	public int m_curr_PerGameCoinsEarned;
	public int m_next_PerGameCoinsEarned;
	public int m_curr_PerGameScoreEarned;
	public int m_next_PerGameScoreEarned;

	public int m_RecoverToPerfectFromRed;
	public int m_curr_LinkGemsInOneChain;
	public int m_next_LinkGemsInOneChain;

	[System.NonSerialized]
	public HashSet<GemLibrary.GemSet> m_Sets;

	public GameData()
	{
		m_HighScore = 0;
		m_HighestCombo = 0;
		m_MultiplayerHighScore = 0;
		m_MultiplayerHighestCombo = 0;
		m_Coin = 20000;
		m_EquippedGemSet = GemLibrary.GemSet.GEM;
		m_GemList = new List<GemLibrary.GemSet>();
		m_Sets = new HashSet<GemLibrary.GemSet>();
		m_SeenTutorial = false;

		m_Boost_ScoreMultOnce = false;
		m_Boost_GoldMultOnce = false;
		m_Boost_MoreHealthOnce = false;

		m_Boost_ScoreMult = 0;
		m_Boost_GoldMult = 2;
		m_Boost_Shield = 1;
		m_Boost_SlowerGems = 3;
		m_Boost_BiggerGems = 0;

		m_curr_TotalGamesPlayed = 0;
		m_next_TotalGamesPlayed = 0;
		m_curr_TotalCoinsEarned = 0;
		m_next_TotalCoinsEarned = 0;
		m_curr_TotalScoreEarned = 0;
		m_next_TotalScoreEarned = 0;

		m_curr_TotalLinkedGems_R = 0;
		m_next_TotalLinkedGems_R = 0;
		m_curr_TotalLinkedGems_B = 0;
		m_next_TotalLinkedGems_B = 0;
		m_curr_TotalLinkedGems_G = 0;
		m_next_TotalLinkedGems_G = 0;
		m_curr_TotalLinkedGems_Y = 0;
		m_next_TotalLinkedGems_Y = 0;

		m_next_MaxCombo = 0;
		m_next_PerGameCoinsEarned = 0;
		m_next_PerGameScoreEarned = 0;

		m_RecoverToPerfectFromRed = 0;
		m_next_LinkGemsInOneChain = 0;

	}

	public void UnlockGem( GemLibrary.GemSet gemType )
	{
		if ( !m_Sets.Contains( gemType ) )
		{
			m_Sets.Add( gemType );
			m_GemList.Add( gemType );
		}
	}
}
