using UnityEngine;
using System.Collections;

public class MainMenuLoader : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		SaveLoad.Load();

		// For first time use
		if( GameData.Instance.m_Sets.Count == 0 )
		{
			//GameData.Instance.m_Sets.Add( GemLibrary.GemSet.GEM );
			//GameData.Instance.m_GemList.Add( GemLibrary.GemSet.GEM );
			GameData.Instance.UnlockGem( GemLibrary.GemSet.GEM );
			GameData.Instance.m_EquippedGemSet = GemLibrary.GemSet.GEM;
			SaveLoad.Save();
		}
	}
}
