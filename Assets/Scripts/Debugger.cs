using UnityEngine;
using System;
using System.Collections.Generic;

public class Debugger : Singleton<Debugger>
{
	protected Debugger()
	{
		
	}

	public static void AddGold()
	{
#if ADD_GOLD_ENABLE
		GameData.Instance.m_Coin += 100;
		//SaveLoad.Save();
		SaveDataLoader.SaveGame();
#endif
	}
}
