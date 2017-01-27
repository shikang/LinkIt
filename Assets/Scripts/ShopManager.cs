using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
	const int PER_BUY = 100;

	public GemLibrary m_GemLibrary;
	public GameObject m_ItemContent;

	public GameObject m_ItemPrefab;

	// Use this for initialization
	void Start ()
	{
		InitialiseShopList();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void InitialiseShopList()
	{
		// @debug variables
		List<GemContainerSet> gemsSetList = new List<GemContainerSet>();
		for( int i = 0; i < 2; ++i )
		{
			gemsSetList.Add( m_GemLibrary.m_GemsSetList[i % 2] );
		}

		// @todo Resize Item Prefab for new art assets
		// Resize base on item count
		RectTransform transform = m_ItemContent.GetComponent<RectTransform>();
		Vector2 dimension = transform.sizeDelta;
		dimension.x += 20 * ( gemsSetList.Count - 1 ); // @todo add with prefab width and padding left right
		transform.sizeDelta = dimension;
	}

	public bool Buy()
	{
		/*
		if ( GameData.current.coin >= PER_BUY )
		{
			Debug.Log( GameData.current.coin.ToString() + " -> " + (GameData.current.coin - PER_BUY).ToString() );

			Defines.ICONS icon = (Defines.ICONS)Random.Range( (int)Defines.ICONS.EMPTY + 1, (int)Defines.ICONS.TOTAL - 1 );
			GameData.current.coin -= PER_BUY;

			Debug.Log("Bought icon: " + icon.ToString());

			if( GameData.current.icons == null )
			{
				Debug.Log("Icon in save data is null!");
				GameData.current.icons = new System.Collections.Generic.List<Defines.ICONS>();
			}

			if( !GameData.current.icons.Contains( icon ) )
			{
				GameData.current.icons.Add( icon );
			}

			foreach ( Defines.ICONS i in GameData.current.icons )
			{
				Debug.Log( "You have: " + i.ToString() );
			}

			SaveLoad.Save();
			return true;
		}
		else
		{
			Debug.Log( "Not enough money!" );
			return false;
		}
		*/
		return true;
	}
}
