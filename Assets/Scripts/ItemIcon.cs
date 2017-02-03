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
		if ( m_bLocked )
		{
			// @todo In App Purchase
		}
		else
		{
			GameObject gemDetails = GameObject.Find( "Gem Details" );
			GemDetails gd = gemDetails.GetComponent<GemDetails>();
			gd.EquipGemSet( m_ItemType );

			GameObject shopManager = GameObject.Find( "Shop Manager" );
			ShopManager sm = shopManager.GetComponent<ShopManager>();
			sm.ChangeEquippedSprite( m_ItemType );
		}
	}
}
