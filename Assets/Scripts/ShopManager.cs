using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
	public const int PER_BUY = 100;
	public const int ITEM_PAD = 50;

	public GemLibrary m_GemLibrary;
	public GameObject m_ItemContent;

	public GameObject m_ItemPrefab;

	public HorizontalScrollSnap m_HorizontalScrollSnap;

	//float m_fItemContentWidth = 0.0f;
	float m_fItemIconWidth = 0.0f;
	Vector3 m_ItemIconStartPos;
	Vector3 m_PreviousContentPos;

	RectTransform m_ItemContentTransform;
	List<GameObject> m_ItemIcons;

	public List<GameObject> ItemIcons
	{
		get
		{
			return m_ItemIcons;
		}
	}

	public float ItemIconWidth
	{
		get
		{
			return m_fItemIconWidth;
		}
	}

	// Use this for initialization
	void Start ()
	{
		//m_fItemContentWidth = m_ItemContent.GetComponent<RectTransform>().sizeDelta.x;
		m_fItemIconWidth = m_ItemPrefab.GetComponent<RectTransform>().sizeDelta.x;
		m_ItemIconStartPos = m_ItemPrefab.GetComponent<RectTransform>().localPosition;

		m_ItemContentTransform = m_ItemContent.GetComponent<RectTransform>();
		m_ItemIcons = new List<GameObject>();

		m_PreviousContentPos = m_ItemContentTransform.localPosition;

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

		// Text
		{
			GameObject label = itemIcon.transform.GetChild( 0 ).gameObject;
			label.SetActive( enable );
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
			itemIcon.GetComponentInChildren<Text>().text = m_GemLibrary.m_GemsSetList[i % 2].m_sGemContainerSetName;

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

	public void OnScroll( Vector2 pos )
	{
		Vector3 dir = m_ItemContentTransform.localPosition - m_PreviousContentPos;
		m_PreviousContentPos = m_ItemContentTransform.localPosition;

		/*
		float x = -m_ItemContentTransform.localPosition.x;
		x -= m_ItemIconStartPos.x;
		x += ITEM_PAD;

		float fIndex = x / ( m_fItemIconWidth + ITEM_PAD ) + 1.0f;
		int nIndex = Mathf.Clamp( (int)Mathf.Round( fIndex ), 0, m_ItemIcons.Count - 1 );
		*/
		int nIndex = m_HorizontalScrollSnap.FindClosestIndex();

		for ( int i = 0; i < m_ItemIcons.Count; ++i )
		{
			GameObject itemIcon = m_ItemIcons[i];
			SetItemIconEnable( itemIcon.GetComponent<Button>(), i == nIndex );
		}

		if ( dir.sqrMagnitude <= 1.0f )
		{
			m_HorizontalScrollSnap.SetScrollStop();
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
