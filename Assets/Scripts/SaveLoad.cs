using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveLoad
{
	// Save at score, buying, equipping
	public static void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create( Application.persistentDataPath + "/save.gd" );
		bf.Serialize( file, GameData.Instance );
		file.Close();
	}

	public static void Load()
	{
		if ( File.Exists( Application.persistentDataPath + "/save.gd" ) )
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open( Application.persistentDataPath + "/save.gd", FileMode.Open );
			GameData.Instance = ( GameData )bf.Deserialize( file );
			file.Close();

			if ( GameData.Instance.m_Sets == null )
			{
				GameData.Instance.m_Sets = new HashSet<GemLibrary.GemSet>();
				GameData.Instance.m_EquippedGemSet = GemLibrary.GemSet.GEM;
			}
		}
		else
		{
			GameData.Instance = new GameData();
		}
	}
}
