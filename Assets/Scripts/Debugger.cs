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
		GameData.Instance.m_Coin += 100;
		SaveLoad.Save();
	}
}
