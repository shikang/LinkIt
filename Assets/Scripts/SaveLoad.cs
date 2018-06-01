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

			if ( GameData.Instance.m_GemList == null )
			{
				GameData.Instance.m_GemList = new List<GemLibrary.GemSet>();
				GameData.Instance.m_EquippedGemSet = GemLibrary.GemSet.GEM;
			}

			GameData.Instance.m_Sets = new HashSet<GemLibrary.GemSet>();
			foreach ( GemLibrary.GemSet gemSet in GameData.Instance.m_GemList )
			{
				GameData.Instance.m_Sets.Add( gemSet );
			}
		}
		else
		{
			GameData.Instance = new GameData();
		}
	}

	public static void LoadFromByteArray( byte[] bytes )
	{
		if ( bytes.Length > 0 )	// Save exist
		{
			using ( var memStream = new MemoryStream() )
			{
				BinaryFormatter bf = new BinaryFormatter();
				memStream.Write( bytes, 0, bytes.Length );
				memStream.Seek( 0, SeekOrigin.Begin );
				GameData.Instance = ( GameData )bf.Deserialize( memStream );
			}
		}
		else
		{
			GameData.Instance = new GameData();
		}
	}

	public static byte[] ToByteArray()
	{
		BinaryFormatter bf = new BinaryFormatter();
		using ( var ms = new MemoryStream() )
		{
			bf.Serialize( ms, GameData.Instance );
			return ms.ToArray();
		}
	}
}
