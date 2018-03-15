using UnityEngine;
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
	}
}
