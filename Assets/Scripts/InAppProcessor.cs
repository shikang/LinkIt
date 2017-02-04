using UnityEngine;
using System;
using System.Collections.Generic;

public class InAppProcessor : Singleton<InAppProcessor>
{
	public class ProductParam
	{
		public InAppProductList.ProductType m_ProductType;
		public int m_nProductParam;

		public ProductParam( InAppProductList.ProductType productType, int productParam )
		{
			m_ProductType = productType;
			m_nProductParam = productParam;
		}
	}

	private Dictionary<string, ProductParam> m_ProductParamMap;

	protected InAppProcessor()
	{
		m_ProductParamMap = new Dictionary<string, ProductParam>();
	}

	public void AddProductParam( string productIdentifier, InAppProductList.ProductType productType, int productParam)
	{
		m_ProductParamMap.Add( productIdentifier, new ProductParam( productType, productParam ) );
	}

	public void ProcessPurchase( string productIdentifier )
	{
		if( m_ProductParamMap.ContainsKey( productIdentifier ) )
		{
			ProductParam productParam = m_ProductParamMap[productIdentifier];

			switch ( productParam.m_ProductType )
			{
				case InAppProductList.ProductType.COIN:
					GameData.Instance.m_Coin += productParam.m_nProductParam;
					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: PASS. Product: '{0}'", productIdentifier ) );

					// @todo Some feedback
					break;
				case InAppProductList.ProductType.AVATAR:
					GemLibrary.GemSet gemType = (GemLibrary.GemSet)productParam.m_nProductParam;

					if ( !GameData.Instance.m_Sets.Contains( gemType ) )
					{
						GameData.Instance.m_Sets.Add( gemType );
						GameData.Instance.m_GemList.Add( gemType );
					}

					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: PASS. Product: '{0}'", productIdentifier ) );

					// @todo Some feedback
					break;
				default:
					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: FAIL. Invalid product type: '{0}'", productParam.m_ProductType.ToString() ) );
					return;
			}

			SaveLoad.Save();
		}
		else
		{
			Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: FAIL. Unrecognized product: '{0}'", productIdentifier ) );
		}
	}

	public Dictionary<string, ProductParam> ProductParamMap
	{
		get
		{
			return m_ProductParamMap;
		}
	}
}
