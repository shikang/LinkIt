using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class InAppPurchaser : MonoBehaviour, IStoreListener
{
	private static IStoreController m_StoreController;          // The Unity Purchasing system.
	private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

	void Start()
	{
		// If we haven't set up the Unity Purchasing reference
		if ( m_StoreController == null )
		{
			// Begin to configure our connection to Purchasing
			InitializePurchasing();
		}
	}

	public void InitializePurchasing()
	{
		// If we have already connected to Purchasing ...
		if ( IsInitialized() )
		{
			// ... we are done here.
			return;
		}

		// Create a builder, first passing in a suite of Unity provided stores.
		var builder = ConfigurationBuilder.Instance( StandardPurchasingModule.Instance() );

		InAppProductList.Instance.InitialiseProductList();

		// Add Consumable
		foreach ( InAppProductList.ProductInfo product in InAppProductList.Instance.ConsumableList )
		{
			AddProduct( builder, product, ProductType.Consumable );
		}

		// Continue adding the non-consumable product.
		foreach ( InAppProductList.ProductInfo product in InAppProductList.Instance.NonConsumableList )
		{
			AddProduct( builder, product, ProductType.NonConsumable );
		}

		// Adding subscription
		foreach ( InAppProductList.ProductInfo product in InAppProductList.Instance.SubscriptionList )
		{
			AddProduct( builder, product, ProductType.Subscription );
		}

		// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
		// and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
		UnityPurchasing.Initialize( this, builder );
	}


	private bool IsInitialized()
	{
		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	private void AddProduct( ConfigurationBuilder builder, InAppProductList.ProductInfo product, ProductType productType )
	{
		// Add a product to sell / restore by way of its identifier, associating the general identifier
		// with its store-specific identifiers.

		if ( product.m_nStoreFlag == InAppProductList.Store.ALL )
		{
			builder.AddProduct( product.m_sProductIdentifier, productType );
		}
		else
		{
			// @todo: Not tested yet
			if ( ( product.m_nStoreFlag & InAppProductList.Store.GOOGLE ) != 0 && Application.platform == RuntimePlatform.Android )
			{
				builder.AddProduct( product.m_sProductIdentifier, productType );
			}
			else if ( ( product.m_nStoreFlag & InAppProductList.Store.APPLE ) != 0 && Application.platform == RuntimePlatform.IPhonePlayer )
			{
				builder.AddProduct( product.m_sProductIdentifier, productType );
			}
		}

		// @todo: Handle different id logic
		/*
		builder.AddProduct( product.m_sProductIdentifier, productType, new IDs(){
				{ product.m_sProductAppleIdentifier, AppleAppStore.Name },
				{ product.m_sProductGoogleIdentifier, GooglePlay.Name },
			});
		*/
	}

	public string GetProductLocalisePrice( InAppProductList.ProductType productType, int productParam )
	{
		return GetProductLocalisePrice( InAppProductList.GetProductIdentifier( productType, productParam ) );
	}

	string GetProductLocalisePrice( string productId )
	{
		return m_StoreController.products.WithID( productId ).metadata.localizedPriceString;
	}

	public void BuyProduct( InAppProductList.ProductType productType, int productParam )
	{
		BuyProductID( InAppProductList.GetProductIdentifier( productType, productParam ) );
	}

	void BuyProductID( string productId )
	{
		// If Purchasing has been initialized ...
		if ( IsInitialized() )
		{
			// ... look up the Product reference with the general product identifier and the Purchasing 
			// system's products collection.
			Product product = m_StoreController.products.WithID( productId );

			// If the look up found a product for this device's store and that product is ready to be sold ... 
			if ( product != null && product.availableToPurchase )
			{
				Debug.Log( string.Format( "Purchasing product asychronously: '{0}'", product.definition.id ) );
				// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
				// asynchronously.
				m_StoreController.InitiatePurchase( product );
			}
			// Otherwise ...
			else
			{
				// ... report the product look-up failure situation  
				Debug.Log( "InAppPurchaser::BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase" );
			}
		}
		// Otherwise ...
		else
		{
			// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
			// retrying initiailization.
			Debug.Log( "InAppPurchaser::BuyProductID FAIL. Not initialized." );
		}
	}


	// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
	// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
	public void RestorePurchases()
	{
		// If Purchasing has not yet been set up ...
		if ( !IsInitialized() )
		{
			// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
			Debug.Log( "InAppPurchaser::RestorePurchases FAIL. Not initialized." );
			return;
		}

		// If we are running on an Apple device ... 
		if ( Application.platform == RuntimePlatform.IPhonePlayer ||
			 Application.platform == RuntimePlatform.OSXPlayer )
		{
			// ... begin restoring purchases
			Debug.Log( "InAppPurchaser::RestorePurchases started ..." );

			// Fetch the Apple store-specific subsystem.
			var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
			// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
			// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
			apple.RestoreTransactions( (result) => {
				// The first phase of restoration. If no more responses are received on ProcessPurchase then 
				// no purchases are available to be restored.
				Debug.Log( "InAppPurchaser::RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore." );
			} );
		}
		// Otherwise ...
		else
		{
			// We are not running on an Apple device. No work is necessary to restore purchases.
			Debug.Log( "InAppPurchaser::RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform );
		}
	}


	//  
	// --- IStoreListener
	//

	public void OnInitialized( IStoreController controller, IExtensionProvider extensions )
	{
		// Purchasing has succeeded initializing. Collect our Purchasing references.
		Debug.Log( "InAppPurchaser::OnInitialized: PASS" );

		// Overall Purchasing system, configured with products for this application.
		m_StoreController = controller;
		// Store specific subsystem, for accessing device-specific store features.
		m_StoreExtensionProvider = extensions;

		// Update product price
		// Add Consumable
		foreach ( InAppProductList.ProductInfo product in InAppProductList.Instance.ConsumableList )
		{
			product.m_fPrice = GetProductLocalisePrice( product.m_sProductIdentifier );
			Debug.Log( "InAppPurchaser::OnInitialized: Update price[" + product.m_sProductIdentifier + "," + product.m_fPrice + "]" );
		}

		// Continue adding the non-consumable product.
		foreach ( InAppProductList.ProductInfo product in InAppProductList.Instance.NonConsumableList )
		{
			product.m_fPrice = GetProductLocalisePrice( product.m_sProductIdentifier );
			Debug.Log( "InAppPurchaser::OnInitialized: Update price[" + product.m_sProductIdentifier + "," + product.m_fPrice + "]" );
		}

		// Adding subscription
		foreach ( InAppProductList.ProductInfo product in InAppProductList.Instance.SubscriptionList )
		{
			product.m_fPrice = GetProductLocalisePrice( product.m_sProductIdentifier );
			Debug.Log( "InAppPurchaser::OnInitialized: Update price[" + product.m_sProductIdentifier + "," + product.m_fPrice + "]" );
		}
	}


	public void OnInitializeFailed( InitializationFailureReason error )
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log( "InAppPurchaser::OnInitializeFailed InitializationFailureReason:" + error );
	}


	public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs args )
	{
		InAppProcessor.Instance.ProcessPurchase( args.purchasedProduct.definition.id );

		// Return a flag indicating whether this product has completely been received, or if the application needs 
		// to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
		// saving purchased products to the cloud, and when that save is delayed. 
		return PurchaseProcessingResult.Complete;
	}


	public void OnPurchaseFailed( Product product, PurchaseFailureReason failureReason )
	{
		// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
		// this reason with the user to guide their troubleshooting actions.
		Debug.Log( string.Format( "InAppPurchaser::OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason ) );
	}
}
