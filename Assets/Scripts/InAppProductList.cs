using UnityEngine;
using System;
using System.Collections.Generic;

public class InAppProductList : Singleton<InAppProductList>
{
	// Info constants
	public const string COMPANY_NAME = "whywool";
	public const string PROJECT_NAME = "linkit";

	public const string PRODUCT_PREFIX = "com." + COMPANY_NAME + "." + PROJECT_NAME + ".";

	public const string PRODUCT_COIN = "coin.";
	public const string PRODUCT_AVATAR = "avatar.";

	[Flags]
	public enum Store
	{
		GOOGLE = 1 << 0,
		APPLE = 1 << 1,

		ALL = GOOGLE | APPLE,
	}

	public class ProductInfo
	{
		public string m_sProductIdentifier;
		public Store m_nStoreFlag;

		public ProductInfo ( string productIdentifier, Store storeFlag )
		{
			m_sProductIdentifier = productIdentifier;
			m_nStoreFlag = storeFlag;
		}
	}

	private List<ProductInfo> m_ConsumableList;
	private List<ProductInfo> m_NonConsumableList;
	private List<ProductInfo> m_SubscriptionList;

	public void InitialiseProductList()
	{
		if ( m_ConsumableList.Count != 0 || m_NonConsumableList.Count != 0 || m_SubscriptionList.Count != 0 )
			return;

		// Consumables
		m_ConsumableList.Add( new ProductInfo( PRODUCT_PREFIX + PRODUCT_COIN + "100", Store.ALL ) );

		// Non-consumables
		GemLibrary gemLibrary = GameObject.Find( "Gem Library" ).GetComponent<GemLibrary>();
		foreach ( GemContainerSet gemSet in gemLibrary.m_GemsSetList )
		{
			m_NonConsumableList.Add( new ProductInfo( PRODUCT_PREFIX + PRODUCT_AVATAR + gemSet.m_sGemContainerSetName, Store.ALL ) );
		}

		// Subscriptions
	}

	protected InAppProductList()
	{
		m_ConsumableList = new List<ProductInfo>();
		m_NonConsumableList = new List<ProductInfo>();
		m_SubscriptionList = new List<ProductInfo>();
	}

	public List<ProductInfo> ConsumableList
	{
		get
		{
			return m_ConsumableList;
		}
	}

	public List<ProductInfo> NonConsumableList
	{
		get
		{
			return m_NonConsumableList;
		}
	}

	public List<ProductInfo> SubscriptionList
	{
		get
		{
			return m_SubscriptionList;
		}
	}
}
