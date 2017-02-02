using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
	const int PER_BUY = 100;
	const int ITEM_PAD = 50;

	public GemLibrary m_GemLibrary;
	public GameObject m_ItemContent;

	public GameObject m_ItemPrefab;

	//float m_fItemContentWidth = 0.0f;
	float m_fItemIconWidth = 0.0f;
	Vector3 m_ItemIconStartPos;

	RectTransform m_ItemContentTransform;
	List<GameObject> m_ItemIcons;

	// Use this for initialization
	void Start ()
	{
		//m_fItemContentWidth = m_ItemContent.GetComponent<RectTransform>().sizeDelta.x;
		m_fItemIconWidth = m_ItemPrefab.GetComponent<RectTransform>().sizeDelta.x;
		m_ItemIconStartPos = m_ItemPrefab.GetComponent<RectTransform>().localPosition;

		m_ItemContentTransform = m_ItemContent.GetComponent<RectTransform>();
		m_ItemIcons = new List<GameObject>();

		InitialiseShopList();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void SetItemIconEnable( Button itemIcon, bool enable )
	{
		float opacity = enable ? 1.0f : 0.5f;
		float scale = enable ? 2.0f : 1.0f;
		itemIcon.enabled = enable;

		// Button color
		{
			ColorBlock cb = itemIcon.colors;
			Color c = cb.normalColor;
			c.a = opacity;
			cb.normalColor = c;
			itemIcon.colors = cb;
		}

		// Image color
		{
			Color c = itemIcon.image.color;
			c.a = opacity;
			itemIcon.image.color = c;
		}

		// Scale
		{
			RectTransform t = itemIcon.GetComponent<RectTransform>();
			t.localScale = scale * Vector3.one;
		}
	}

	void InitialiseShopList()
	{
		// @debug variables
		for ( int i = 0; i < 20; ++i )
		{
			GameObject itemIcon = (GameObject)Instantiate( m_ItemPrefab, m_ItemIconStartPos, Quaternion.identity );
			Button button = itemIcon.GetComponent<Button>();
			button.image.sprite = m_GemLibrary.m_GemsSetList[i % 2].GetGemContainer( GemContainerSet.BLUE_GEM_CONTAINER_INDEX )[0];

			m_ItemIcons.Add( itemIcon );
		}

		// @todo Resize Item Prefab for new art assets
		// Resize base on item count
		Vector2 dimension = m_ItemContentTransform.sizeDelta;
		dimension.x += ( m_fItemIconWidth + ITEM_PAD ) * ( m_ItemIcons.Count - 1 );
		m_ItemContentTransform.sizeDelta = dimension;

		// Add item icons to content
		
		for ( int i = 0; i < m_ItemIcons.Count; ++i )
		{
			GameObject itemIcon = m_ItemIcons[i];
			RectTransform itemIconTransform = itemIcon.GetComponent<RectTransform>();
			itemIconTransform.SetParent( m_ItemContentTransform );

			Vector3 pos = itemIconTransform.localPosition;
			pos.x = m_ItemIconStartPos.x + i * ( ITEM_PAD + m_fItemIconWidth );
			pos.y = m_ItemIconStartPos.y;
			pos.z = m_ItemIconStartPos.z;
			itemIconTransform.localPosition = pos;

			SetItemIconEnable( itemIcon.GetComponent<Button>(), i == 0 );
		}

		m_ItemPrefab.SetActive( false );
	}

	public void OnSwipe( Vector2 dir )
	{
		float x = -m_ItemContentTransform.localPosition.x;
		x -= m_ItemIconStartPos.x;
		x += ( dir.x >= 0 ) ? ITEM_PAD : - 0.5f * ITEM_PAD;

		float fIndex = x / ( m_fItemIconWidth + ITEM_PAD ) + 1.0f;
		int nIndex = Mathf.Clamp( (int)Mathf.Round( fIndex ), 0, m_ItemIcons.Count - 1 );
		//Debug.Log( "Float: " + index );
		Debug.Log( "Int: " + nIndex );

		for ( int i = 0; i < m_ItemIcons.Count; ++i )
		{
			GameObject itemIcon = m_ItemIcons[i];
			SetItemIconEnable( itemIcon.GetComponent<Button>(), i == nIndex );
		}
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
