using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
	public const int PER_BUY = 100;
	public const int ITEM_PAD = 50;

	//public GemLibrary m_GemLibrary;
	GemLibrary m_GemLibrary;
	public GameObject m_ItemContent;
	public GameObject m_Equipped;

	public GameObject m_ItemPrefab;

	public HorizontalScrollSnap m_HorizontalScrollSnap;

	//float m_fItemContentWidth = 0.0f;
	float m_fItemIconWidth = 0.0f;
	Vector2 m_ItemIconDim;
	Vector2 m_ItemIconLockDim;
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
		m_GemLibrary = GemLibrary.Instance;

		//m_fItemContentWidth = m_ItemContent.GetComponent<RectTransform>().sizeDelta.x;
		m_fItemIconWidth = m_ItemPrefab.GetComponent<RectTransform>().sizeDelta.x;
		m_ItemIconDim = m_ItemPrefab.GetComponent<RectTransform>().sizeDelta;
		m_ItemIconLockDim = m_ItemPrefab.transform.GetChild( 1 ).GetComponent<RectTransform>().sizeDelta;
		m_ItemIconStartPos = m_ItemPrefab.GetComponent<RectTransform>().localPosition;

		m_ItemContentTransform = m_ItemContent.GetComponent<RectTransform>();
		m_ItemIcons = new List<GameObject>();

		m_PreviousContentPos = m_ItemContentTransform.localPosition;

		ChangeEquippedSprite( GameData.Instance.m_EquippedGemSet );

		InitialiseShopList();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void SetItemIconEnable( GameObject itemIcon, bool enable )
	{
		Button button = itemIcon.GetComponent<Button>();
		ItemIcon ic = itemIcon.GetComponent<ItemIcon>();

		float opacity = enable ? ( ic.m_bLocked ? 0.7f : 1.0f ) : 0.5f;
		float scale = enable ? 2.0f : 1.0f;
		button.enabled = enable;

		// Button color
		{
			ColorBlock cb = button.colors;
			Color c = cb.normalColor;
			c.a = opacity;
			cb.normalColor = c;
			button.colors = cb;
		}

		// Image color
		{
			Color c = button.image.color;
			c.a = opacity;
			button.image.color = c;
		}

		// Scale
		{
			RectTransform t = itemIcon.GetComponent<RectTransform>();
			t.sizeDelta = scale * m_ItemIconDim;
		}

		// Text
		{
			GameObject label = itemIcon.transform.GetChild( 0 ).gameObject;
			label.SetActive( enable );
		}

		// Lock
		{
			GameObject keyhole = itemIcon.transform.GetChild( 1 ).gameObject;
			RectTransform t = keyhole.GetComponent<RectTransform>();
			t.sizeDelta = scale * m_ItemIconLockDim;
		}

		// Price
		{
			itemIcon.transform.GetChild( 2 ).gameObject.SetActive( enable && ic.m_bLocked );
		}
	}

	void InitialiseShopList()
	{
		// @debug variables
		for ( int i = 0; i < 20; ++i )
		{
			// @debug
			int nIndex = i % 2;
			GemLibrary.GemSet gemType = (GemLibrary.GemSet)nIndex;

			GameObject itemIcon = (GameObject)Instantiate( m_ItemPrefab, m_ItemIconStartPos, Quaternion.identity );
			Button button = itemIcon.GetComponent<Button>();
			button.image.sprite = m_GemLibrary.GemsSetList[nIndex].GetGemContainer( GemContainerSet.BLUE_GEM_CONTAINER_INDEX )[0];

			Text label = itemIcon.GetComponentInChildren<Text>();
			label.text = m_GemLibrary.GemsSetList[nIndex].m_sGemContainerSetName;

			ItemIcon ic = itemIcon.GetComponent<ItemIcon>();
			ic.m_ItemType = gemType;

			itemIcon.transform.GetChild( 2 ).gameObject.SetActive( false );

			// Hide lock if unlocked
			if ( GameData.Instance.m_Sets.Contains( gemType ) )
			{
				itemIcon.transform.GetChild( 1 ).gameObject.SetActive( false );
				ic.m_bLocked = false;
			}
			else
			{
				Color c = label.color;
				c.a = 0.5f;
				label.color = c;

				ic.m_bLocked = true;

				string productIdentifier = InAppProductList.GetProductIdentifier( InAppProductList.ProductType.AVATAR, nIndex );
				if ( InAppProductList.Instance.NonConsumableList.ContainsKey( productIdentifier ) )
				{
					InAppProductList.ProductInfo product = InAppProductList.Instance.NonConsumableList[productIdentifier];

					GameObject priceTag = itemIcon.transform.GetChild( 2 ).gameObject;
					Text priceText = priceTag.GetComponent<Text>();
					priceText.text = product.m_sPrice;
				}
				
			}

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
			itemIconTransform.localScale = Vector3.one;

			SetItemIconEnable( itemIcon, i == 0 );
		}

		m_ItemPrefab.SetActive( false );

		// @todo set scroll to equipped
		// @todo back button in item screen will scroll to equipped as well
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
			SetItemIconEnable( itemIcon, i == nIndex );
		}

		if ( dir.sqrMagnitude <= HorizontalScrollSnap.SNAP_SPEED )
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

	public void ChangeEquippedSprite( GemLibrary.GemSet gemType )
	{
		SpriteRenderer sr = m_Equipped.GetComponent<SpriteRenderer>();
		sr.sprite = m_GemLibrary.GemsSetList[ (int)gemType ].GetGemContainer( GemContainerSet.BLUE_GEM_CONTAINER_INDEX )[0];
	}

	public void EnableScrolling( bool enable )
	{
		ScrollRect scroll = m_HorizontalScrollSnap.GetComponent<ScrollRect>();
		scroll.horizontal = enable;
	}

	public void EnableItemScreenControl( bool enable )
	{
		EnableScrolling( enable );

		GameObject mainMenuManager = GameObject.FindGameObjectWithTag( "Main Menu Manager" );
		MainMenuManager mmm = mainMenuManager.GetComponent<MainMenuManager>();
		mmm.EnableBackButton( MainMenuManager.eScreen.ITEM, enable );

		GameObject itemIcon = GetCurrentItemIcon();
		Button b = itemIcon.GetComponent<Button>();
		b.enabled = enable;

		if ( enable )
		{
			ItemIcon ic = itemIcon.GetComponent<ItemIcon>();
			if ( GameData.Instance.m_Sets.Contains( ic.m_ItemType ) )
			{
				itemIcon.transform.GetChild( 1 ).gameObject.SetActive( false );
				ic.m_bLocked = false;

				Text label = itemIcon.GetComponentInChildren<Text>();
				Color c = label.color;
				c.a = 1.0f;
				label.color = c;

				SetItemIconEnable( itemIcon, true );
			}
		}
	}

	public GameObject GetCurrentItemIcon()
	{
		for ( int i = 0; i < m_ItemIcons.Count; ++i )
		{
			GameObject itemIcon = m_ItemIcons[i];
			GameObject label = itemIcon.transform.GetChild( 0 ).gameObject;

			if ( label.GetActive() )
				return itemIcon;
		}

		return null;
	}

	// @debug
	public void AddGold()
	{
		Debugger.AddGold();
	}
}
