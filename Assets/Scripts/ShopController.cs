using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum SHOPLIST
{
	COIN_SMALL = 0,
	COIN_MEDIUM,
	COIN_LARGE,
	COIN_JUMBO
};

public class ShopController : MonoBehaviour
{
	public GameObject m_ShopSlot1;
	public GameObject m_ShopSlot2;
	public GameObject m_ShopSlot3;
	public GameObject m_ShopSlot4;
	public GameObject m_PlayerCoins;

	void Start()
	{
		SetItems();
	}

	void Update()
	{
	}

	void SetItems ()
	{
		m_ShopSlot1.GetComponent<ShopMainMenu>().SetData("Handful of Coins", 100, 1.99f, "Contains %d coins");
		m_ShopSlot2.GetComponent<ShopMainMenu>().SetData("Bag of Coins",	 250, 3.99f, "Contains %d coins");
		m_ShopSlot3.GetComponent<ShopMainMenu>().SetData("Bucket of Coins",	 500, 6.99f, "Contains %d coins");
		m_ShopSlot4.GetComponent<ShopMainMenu>().SetData("Room of Coins",	1000, 9.99f, "Contains %d coins");
		m_PlayerCoins.GetComponent<Text>().text = GameData.Instance.m_Coin.ToString();
	}

	public void FakeAddCoin(int amt)
	{
		GameData.Instance.m_Coin += amt;
		SaveLoad.Save();
		m_PlayerCoins.GetComponent<Text>().text = GameData.Instance.m_Coin.ToString();
	}
}
