using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum ITEMLIST
{
	SLOW_GEMS = 0,
	MULTIPLY_SCORE,
	DESTROY_COLOR,
	LINK_GEMS
};

public class ItemController : MonoBehaviour
{
	public GameObject m_ItemSlot1;
	public GameObject m_ItemSlot2;
	public GameObject m_ItemSlot3;
	public GameObject m_ItemSlot4;

	public GameObject m_PlayerCoins;
	public GameObject m_Spending;

	bool [] m_bItemsActivated;

	void Start()
	{
		m_bItemsActivated = new bool[4];
		SetItems();
	}

	void Update()
	{
	}

	void SetItems ()
	{
		m_ItemSlot1.GetComponent<ItemMainMenu>().SetData("Faster Charge",	350, "Powerup gauge fills up 2x faster");
		m_ItemSlot2.GetComponent<ItemMainMenu>().SetData("Score 2x",		500, "Multiply your score by 2x");
		m_ItemSlot3.GetComponent<ItemMainMenu>().SetData("HP Recovery",		650, "HP recovers 2x faster");
		m_ItemSlot4.GetComponent<ItemMainMenu>().SetData("Longer Powerups",	1000, "Powerups last 2x longer");
		m_PlayerCoins.GetComponent<Text>().text = GameData.Instance.m_Coin.ToString();
		m_Spending.GetComponent<Text>().text = "0";
	}

	public void CalculateSpending ()
	{
		uint spent = 0;
		if(m_ItemSlot1.GetComponent<ItemMainMenu>().IsActive())
		{
			spent += m_ItemSlot1.GetComponent<ItemMainMenu>().GetCost();
		}
		if(m_ItemSlot2.GetComponent<ItemMainMenu>().IsActive())
		{
			spent += m_ItemSlot2.GetComponent<ItemMainMenu>().GetCost();
		}
		if(m_ItemSlot3.GetComponent<ItemMainMenu>().IsActive())
		{
			spent += m_ItemSlot3.GetComponent<ItemMainMenu>().GetCost();
		}
		if(m_ItemSlot4.GetComponent<ItemMainMenu>().IsActive())
		{
			spent += m_ItemSlot4.GetComponent<ItemMainMenu>().GetCost();
		}
		m_Spending.GetComponent<Text>().text = spent.ToString();

		if(spent > GameData.Instance.m_Coin)
		{
			m_Spending.GetComponent<Text>().color = Color.red;
		}
		else
		{
			m_Spending.GetComponent<Text>().color = Color.white;
		}
	}
}
