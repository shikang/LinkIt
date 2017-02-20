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

	[System.NonSerialized]
	public HashSet<GemLibrary.GemSet> m_Sets;

	public GameData()
	{
		m_HighScore = 0;
		m_HighestCombo = 0;
		m_MultiplayerHighScore = 0;
		m_MultiplayerHighestCombo = 0;
		m_Coin = 0;
		m_EquippedGemSet = GemLibrary.GemSet.GEM;
		m_GemList = new List<GemLibrary.GemSet>();
		m_Sets = new HashSet<GemLibrary.GemSet>();
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
