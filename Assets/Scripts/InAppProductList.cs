using UnityEngine;
using System;
using System.Collections.Generic;

public class InAppProductList : Singleton<InAppProductList>
{
	// Info constants
	public const string COMPANY_NAME = "whywool";
	public const string PROJECT_NAME = "linkit";

	public const string PRODUCT_PREFIX = "com." + COMPANY_NAME + "." + PROJECT_NAME + ".";

	// Product Type
	public enum ProductType
	{
		COIN,
		AVATAR,
		DISABLE_ADS,
	}

	// Comsumables
	Dictionary<ProductType, List<int>> m_ConsumableMap = new Dictionary<ProductType, List<int>>
														{
															{ ProductType.COIN, new List<int>{ 100, 250, 500, 1000 } },
														};

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
		public string m_sPrice;

		public ProductInfo( string productIdentifier, Store storeFlag )
		{
			m_sProductIdentifier = productIdentifier;
			m_nStoreFlag = storeFlag;
			m_sPrice = "Invalid Price";
		}
	}

	private Dictionary<string, ProductInfo> m_ConsumableList;
	private Dictionary<string, ProductInfo> m_NonConsumableList;
	private Dictionary<string, ProductInfo> m_SubscriptionList;

	public void InitialiseProductList()
	{
		if ( m_ConsumableList.Count != 0 || m_NonConsumableList.Count != 0 || m_SubscriptionList.Count != 0 )
			return;

		// Consumables
		foreach ( KeyValuePair<ProductType, List<int>> consumeInfo in m_ConsumableMap )
		{
			foreach ( int amount in consumeInfo.Value )
			{
				string productIdentifier = GetProductIdentifier( consumeInfo.Key, amount );
				m_ConsumableList.Add( productIdentifier, new ProductInfo( productIdentifier, Store.ALL ) );

				InAppProcessor.Instance.AddProductParam( productIdentifier, consumeInfo.Key, amount );
			}
		}

		// Non-consumables
#if ENABLE_BUY_AVATAR
		//GemLibrary gemLibrary = GameObject.Find( "Gem Library" ).GetComponent<GemLibrary>();
		GemLibrary gemLibrary = GemLibrary.Instance;
		for ( int i = 0; i < gemLibrary.GemsSetList.Count; ++i )
		{
			string productIdentifier = GetProductIdentifier( ProductType.AVATAR, i );
			m_NonConsumableList.Add( productIdentifier, new ProductInfo( productIdentifier, Store.ALL ) );

			InAppProcessor.Instance.AddProductParam( productIdentifier, ProductType.AVATAR, i );
		}
#endif
		// Disable Ads
		{
			string productIdentifier = GetProductIdentifier( ProductType.DISABLE_ADS, 0 );
			m_NonConsumableList.Add( productIdentifier, new ProductInfo( productIdentifier, Store.ALL ) );
			InAppProcessor.Instance.AddProductParam( productIdentifier, ProductType.DISABLE_ADS, 0 );
		}

		// Subscriptions
	}

	// return: com.<company_name>.<project_name>.<product_type>.<product_identifier>
	public static string GetProductIdentifier( ProductType productType, int productParam )
	{
		switch ( productType )
		{
			case InAppProductList.ProductType.COIN:
				return PRODUCT_PREFIX + productType.ToString().ToLower() + "." + productParam.ToString();
			case InAppProductList.ProductType.AVATAR:
				//GemLibrary gemLibrary = GameObject.Find( "Gem Library" ).GetComponent<GemLibrary>();
				GemLibrary gemLibrary = GemLibrary.Instance;
				GemContainerSet gemSet = gemLibrary.GemsSetList[productParam];
				return PRODUCT_PREFIX + ProductType.AVATAR.ToString().ToLower() + "." + gemSet.m_sGemContainerSetName.ToLower();
			case InAppProductList.ProductType.DISABLE_ADS:
				return PRODUCT_PREFIX + ProductType.DISABLE_ADS.ToString().ToLower();
			default:
				Debug.Log( string.Format( "InAppProcessor::GetProductIdentifier: FAIL. Invalid product type: '{0}'", productType.ToString() ) );
				return "";
		}
	}

	protected InAppProductList()
	{
		m_ConsumableList = new Dictionary<string, ProductInfo>();
		m_NonConsumableList = new Dictionary<string, ProductInfo>();
		m_SubscriptionList = new Dictionary<string, ProductInfo>();
	}

	public Dictionary<string, ProductInfo> ConsumableList
	{
		get
		{
			return m_ConsumableList;
		}
	}

	public Dictionary<string, ProductInfo> NonConsumableList
	{
		get
		{
			return m_NonConsumableList;
		}
	}

	public Dictionary<string, ProductInfo> SubscriptionList
	{
		get
		{
			return m_SubscriptionList;
		}
	}
}
