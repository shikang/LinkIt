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

	public GameObject m_AdsAreDisabled;
	public GameObject m_AdsNotDisabled;

	public GameObject m_AdsCostText;
	float m_fAdsCost;

	void Start()
	{
		SetItems();

		m_fAdsCost = 0.99f;
		m_AdsCostText.GetComponent<Text> ().text = "$" + m_fAdsCost.ToString ();
	}

	void Update()
	{
		m_AdsAreDisabled.SetActive (GameData.Instance.m_bIsAdsRemoved);
		m_AdsNotDisabled.SetActive (!GameData.Instance.m_bIsAdsRemoved);
	}

	void SetItems ()
	{
		m_ShopSlot1.GetComponent<ShopMainMenu>().SetData("Handful of Coins", 100, 1.99f, "Contains %d coins");
		m_ShopSlot2.GetComponent<ShopMainMenu>().SetData("Bag of Coins",	 250, 3.99f, "Contains %d coins");
		m_ShopSlot3.GetComponent<ShopMainMenu>().SetData("Bucket of Coins",	 500, 6.99f, "Contains %d coins");
		m_ShopSlot4.GetComponent<ShopMainMenu>().SetData("Room of Coins",	1000, 9.99f, "Contains %d coins");
	}
}
