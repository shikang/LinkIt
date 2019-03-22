using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum SHOPLIST
{
	COIN_SMALL = 0,
	COIN_MEDIUM,
	COIN_LARGE,
	COIN_JUMBO
};

public class ShopController : MonoBehaviour
{
	//public GameObject m_ShopSlot1;
	//public GameObject m_ShopSlot2;
	//public GameObject m_ShopSlot3;
	//public GameObject m_ShopSlot4;

	public GameObject []m_ShopSlots;

	public GameObject m_AdsAreDisabled;
	public GameObject m_AdsNotDisabled;

	public GameObject m_AdsCostText;

	public GameObject m_BackButton;

    public GameObject m_ScreenOverlay;
	//float m_fAdsCost;

	void Start()
	{
		//SetItems();

		//m_fAdsCost = 0.99f;
		//m_AdsCostText.GetComponent<Text> ().text = "$" + m_fAdsCost.ToString ();
	}

	void Update()
	{
	}

	public void CheckDisableAds()
	{
		m_AdsAreDisabled.SetActive (GameData.Instance.m_bIsAdsRemoved);
		m_AdsNotDisabled.SetActive (!GameData.Instance.m_bIsAdsRemoved);
	}

	public void EnableAllButtons( bool enable )
	{
		m_BackButton.GetComponent<Button>().interactable = enable;
		if ( !GameData.Instance.m_bIsAdsRemoved ) m_AdsNotDisabled.GetComponentInParent<Button>().interactable = enable;

		for ( int i = 0; i < m_ShopSlots.Length; ++i )
		{
			m_ShopSlots[i].GetComponent<Button>().interactable = enable;
		}

        if (enable)
            m_ScreenOverlay.GetComponent<ShopProcessing>().StopBlocking();
        else
            m_ScreenOverlay.GetComponent<ShopProcessing>().StartBlocking();
    }

	public void SetItems ()
	{
		CheckDisableAds();

		string [] productTitle = { "Handful of Coins", "Bag of Coins", "Bucket of Coins", "Room of Coins" };
		// Should have create shop slot through code. however, I'm lazy
		int i = 0;
		foreach ( KeyValuePair<string, InAppProductList.ProductInfo> p in InAppProductList.Instance.ConsumableList )
		{
			m_ShopSlots[i].GetComponent<ShopMainMenu>().SetData( productTitle[i], (uint)InAppProcessor.Instance.ProductParamMap[p.Key].m_nProductParam * 50, p.Value.m_sPrice, "Contains %d coins" );
			++i;
		}

		m_AdsCostText.GetComponent<Text>().text = InAppProductList.Instance.NonConsumableList[InAppProductList.GetProductIdentifier( InAppProductList.ProductType.DISABLE_ADS, 0 )].m_sPrice;
		//m_ShopSlot1.GetComponent<ShopMainMenu>().SetData("Handful of Coins", 100, 1.99f, "Contains %d coins");
		//m_ShopSlot2.GetComponent<ShopMainMenu>().SetData("Bag of Coins",	 250, 3.99f, "Contains %d coins");
		//m_ShopSlot3.GetComponent<ShopMainMenu>().SetData("Bucket of Coins",	 500, 6.99f, "Contains %d coins");
		//m_ShopSlot4.GetComponent<ShopMainMenu>().SetData("Room of Coins",	1000, 9.99f, "Contains %d coins");
	}

	public void BuyCoin( int index )
	{
		// Disable item page controls
		EnableAllButtons( false );

		// In App Purchase
		GameObject shopManager = GameObject.FindGameObjectWithTag( "Shop Manager" );
		shopManager.GetComponent<InAppPurchaser>().BuyProduct( InAppProductList.ProductType.COIN, (int)m_ShopSlots[index].GetComponent<ShopMainMenu>().GetCoin() );
	}

	public void BuyDisableAds()
	{
		if ( GameData.Instance.m_bIsAdsRemoved ) return;

		// Disable item page controls
		EnableAllButtons( false );

		// In App Purchase
		GameObject shopManager = GameObject.FindGameObjectWithTag( "Shop Manager" );
		shopManager.GetComponent<InAppPurchaser>().BuyProduct( InAppProductList.ProductType.DISABLE_ADS, 0 );
	}
}
