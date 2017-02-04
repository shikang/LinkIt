using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
	public GemLibrary.GemSet m_ItemType;
	public bool m_bLocked = true;

	public  void Start()
	{
		Button button = GetComponent<Button>();
		button.onClick.AddListener( Equip );
	}

	void Equip()
	{
		GameObject shopManager = GameObject.Find( "Shop Manager" );

		if ( m_bLocked )
		{
			// In App Purchase
			shopManager.GetComponent<InAppPurchaser>().BuyProduct( InAppProductList.ProductType.AVATAR, (int)m_ItemType );
		}
		else
		{
			// Equip
			GameObject gemDetails = GameObject.Find( "Gem Details" );
			GemDetails gd = gemDetails.GetComponent<GemDetails>();
			gd.EquipGemSet( m_ItemType );

			ShopManager sm = shopManager.GetComponent<ShopManager>();
			sm.ChangeEquippedSprite( m_ItemType );
		}
	}
}
