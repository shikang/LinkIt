﻿using UnityEngine;
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

	public void AddProductParam( string productIdentifier, InAppProductList.ProductType productType, int productParam )
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
				{ 
					GameData.Instance.m_Coin += productParam.m_nProductParam * 50;
					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: PASS. Product: '{0}'", productIdentifier ) );

					// @todo Some feedback

					// Enable item screen controls
					GameObject shopController = GameObject.FindGameObjectWithTag( "Shop Controller" );
					ShopController sc = shopController.GetComponent<ShopController>();
					sc.EnableAllButtons( true );

				}	break;
				case InAppProductList.ProductType.AVATAR:
				{
					GemLibrary.GemSet gemType = (GemLibrary.GemSet)productParam.m_nProductParam;

					/*
					if ( !GameData.Instance.m_Sets.Contains( gemType ) )
					{
						GameData.Instance.m_Sets.Add( gemType );
						GameData.Instance.m_GemList.Add( gemType );
					}
					*/
					GameData.Instance.UnlockGem( gemType );

					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: PASS. Product: '{0}'", productIdentifier ) );

					// Enable item screen controls
					GameObject shopManager = GameObject.FindGameObjectWithTag( "Shop Manager" );
					ShopManager sm = shopManager.GetComponent<ShopManager>();
					sm.EnableItemScreenControl( true );

					// Some feedback
					//GemLibrary gemLibrary = GameObject.Find( "Gem Library" ).GetComponent<GemLibrary>();
					GemLibrary gemLibrary = GemLibrary.Instance;
					GemContainerSet gemSet = gemLibrary.GemsSetList[productParam.m_nProductParam];

					GameObject explosion = ( GameObject )Instantiate( gemSet.m_Explosion, sm.GetCurrentItemIcon().transform.position, Quaternion.identity );
					explosion.layer = LayerMask.NameToLayer( "UI Particles" );
					ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
					ps.GetComponent<Renderer>().sortingLayerName = "UI Particles";
					ps.GetComponent<Renderer>().sortingOrder = LayerMask.NameToLayer( "UI Particles" );
					Destroy( explosion, ps.duration + ps.startLifetime + Time.deltaTime );

				}	break;
				case InAppProductList.ProductType.DISABLE_ADS:
				{
					GameObject shopManager = GameObject.FindGameObjectWithTag( "Shop Manager" );
					ShopManager sm = shopManager.GetComponent<ShopManager>();
					sm.DisableAds();

					// @todo some feedback

					// Enable item screen controls
					GameObject shopController = GameObject.FindGameObjectWithTag( "Shop Controller" );
					ShopController sc = shopController.GetComponent<ShopController>();
					sc.EnableAllButtons( true );
					sc.CheckDisableAds();

				}	break;
				default:
					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: FAIL. Invalid product type: '{0}'", productParam.m_ProductType.ToString() ) );
					return;
			}

			//SaveLoad.Save();
			SaveDataLoader.SaveGame();
		}
		else
		{
			Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: FAIL. Unrecognized product: '{0}'", productIdentifier ) );
		}
	}

	public void ProcessPurchaseFail( string productIdentifier )
	{
		if( m_ProductParamMap.ContainsKey( productIdentifier ) )
		{
			ProductParam productParam = m_ProductParamMap[productIdentifier];

			switch ( productParam.m_ProductType )
			{
				case InAppProductList.ProductType.COIN:
				{ 
					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: FAIL. Product: '{0}'", productIdentifier ) );

					// Enable item screen controls
					GameObject shopController = GameObject.FindGameObjectWithTag( "Shop Controller" );
					ShopController sc = shopController.GetComponent<ShopController>();
					sc.EnableAllButtons( true );

				}	break;
				case InAppProductList.ProductType.AVATAR:
					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: FAIL. Product: '{0}'", productIdentifier ) );

					// Enable item screen controls
					GameObject shopManager = GameObject.FindGameObjectWithTag( "Shop Manager" );
					ShopManager sm = shopManager.GetComponent<ShopManager>();
					sm.EnableItemScreenControl( true );

					break;
				case InAppProductList.ProductType.DISABLE_ADS:
				{
					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: FAIL. Product: '{0}'", productIdentifier ) );

					// Enable item screen controls
					GameObject shopController = GameObject.FindGameObjectWithTag( "Shop Controller" );
					ShopController sc = shopController.GetComponent<ShopController>();
					sc.EnableAllButtons( true );

				}	break;
				default:
					Debug.Log( string.Format( "InAppProcessor::ProcessPurchase: FAIL. Invalid product type: '{0}'", productParam.m_ProductType.ToString() ) );
					return;
			}

			//SaveLoad.Save();
			SaveDataLoader.SaveGame();
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
