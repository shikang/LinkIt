using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
	public GemLibrary.GemSet m_ItemType;

	public  void Start()
	{
		Button button = GetComponent<Button>();
		button.onClick.AddListener( Equip );
	}

	void Equip()
	{
		GameObject gemDetails = GameObject.Find( "Gem Details" );
		GemDetails gd = gemDetails.GetComponent<GemDetails>();
		gd.EquipGemSet( m_ItemType );
	}
}
