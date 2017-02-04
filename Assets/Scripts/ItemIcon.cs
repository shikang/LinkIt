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
		GameObject shopManager = GameObject.FindGameObjectWithTag( "Shop Manager" );
		ShopManager sm = shopManager.GetComponent<ShopManager>();

		if ( m_bLocked )
		{
			// Disable item page controls
			sm.EnableItemScreenControl( false );

			// In App Purchase
			shopManager.GetComponent<InAppPurchaser>().BuyProduct( InAppProductList.ProductType.AVATAR, (int)m_ItemType );
		}
		else
		{
			// Equip
			GameObject gemDetails = GameObject.Find( "Gem Details" );
			GemDetails gd = gemDetails.GetComponent<GemDetails>();
			gd.EquipGemSet( m_ItemType );

			sm.ChangeEquippedSprite( m_ItemType );
		}
	}
}
