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
	}

	// Comsumables
	Dictionary<ProductType, List<int>> m_ConsumableMap = new Dictionary<ProductType, List<int>>
														{
															{ ProductType.COIN, new List<int>{ 100, 200 } },
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

		public ProductInfo( string productIdentifier, Store storeFlag )
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
		foreach ( KeyValuePair<ProductType, List<int>> consumeInfo in m_ConsumableMap )
		{
			foreach ( int amount in consumeInfo.Value )
			{
				string productIdentifier = PRODUCT_PREFIX + consumeInfo.Key.ToString().ToLower() + "." + amount.ToString();
				m_ConsumableList.Add( new ProductInfo( productIdentifier, Store.ALL ) );

				InAppProcessor.Instance.AddProductParam( productIdentifier, consumeInfo.Key, amount );
			}
		}
		
		// Non-consumables
		GemLibrary gemLibrary = GameObject.Find( "Gem Library" ).GetComponent<GemLibrary>();
		for ( int i = 0; i < gemLibrary.m_GemsSetList.Count; ++i )
		{
			GemContainerSet gemSet = gemLibrary.m_GemsSetList[i];
			string productIdentifier = PRODUCT_PREFIX + ProductType.AVATAR.ToString().ToLower() + "." + gemSet.m_sGemContainerSetName;
			m_NonConsumableList.Add( new ProductInfo( productIdentifier, Store.ALL ) );

			InAppProcessor.Instance.AddProductParam( productIdentifier, ProductType.AVATAR, i );
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
