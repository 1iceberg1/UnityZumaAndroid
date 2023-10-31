using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class Purchaser : MonoBehaviour, IStoreListener
{
	private static IStoreController m_StoreController;

	private static IExtensionProvider m_StoreExtensionProvider;

	public static string kProductIDConsumable = "consumable";

	public static string kProductIDNonConsumable = "nonconsumable";

	public static string kProductIDSubscription = "subscription";

	public IAPItem[] iapItems;

	private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

	private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

	public static Purchaser instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (m_StoreController == null)
		{
			InitializePurchasing();
		}
	}

	public void InitializePurchasing()
	{
		if (!IsInitialized())
		{
			//ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			//configurationBuilder.AddProduct(kProductIDConsumable, ProductType.Consumable);
			//configurationBuilder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
			//configurationBuilder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs
			//{
			//	{
			//		kProductNameAppleSubscription,
			//		"AppleAppStore"
			//	},
			//	{
			//		kProductNameGooglePlaySubscription,
			//		"GooglePlay"
			//	}
			//});
			IAPItem[] array = iapItems;
			foreach (IAPItem iAPItem in array)
			{
			//	configurationBuilder.AddProduct(iAPItem.productID, iAPItem.productType);
			}
			//UnityPurchasing.Initialize(this, configurationBuilder);
		}
	}

	private bool IsInitialized()
	{
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void BuyProduct(int index)
	{
		BuyProductID(iapItems[index].productID);
	}

	public void BuyConsumable()
	{
		BuyProductID(kProductIDConsumable);
	}

	public void BuyNonConsumable()
	{
		BuyProductID(kProductIDNonConsumable);
	}

	public void BuySubscription()
	{
		BuyProductID(kProductIDSubscription);
	}

	private void BuyProductID(string productId)
	{
		if (IsInitialized())
		{
			Product product = m_StoreController.products.WithID(productId);
			if (product != null && product.availableToPurchase)
			{
				UnityEngine.Debug.Log($"Purchasing product asychronously: '{product.definition.id}'");
				m_StoreController.InitiatePurchase(product);
			}
			else
			{
				UnityEngine.Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
		}
		else
		{
			UnityEngine.Debug.Log("BuyProductID FAIL. Not initialized.");
			Toast.instance.ShowMessage(Language.Get("CHECK_CONNECTION"));
		}
	}

	public void RestorePurchases()
	{
		if (!IsInitialized())
		{
			UnityEngine.Debug.Log("RestorePurchases FAIL. Not initialized.");
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			UnityEngine.Debug.Log("RestorePurchases started ...");
			//IAppleExtensions extension = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
			//extension.RestoreTransactions(delegate(bool result)
			//{
			//	UnityEngine.Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			//});
		}
		else
		{
			UnityEngine.Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		UnityEngine.Debug.Log("OnInitialized: PASS");
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		UnityEngine.Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		IAPItem iAPItem = null;
		IAPItem[] array = iapItems;
		foreach (IAPItem iAPItem2 in array)
		{
			if (string.Equals(args.purchasedProduct.definition.id, iAPItem2.productID, StringComparison.Ordinal))
			{
				UnityEngine.Debug.Log($"ProcessPurchase: PASS. Product: '{args.purchasedProduct.definition.id}'");
				iAPItem = iAPItem2;
			}
		}
		if (iAPItem.productType == ProductType.Consumable)
		{
			CurrencyController.CreditBalance(iAPItem.value);
			Toast.instance.ShowMessage(Language.Get("PURCHASE_SUCCESSFUL"));
			CUtils.SetBuyItem();
		}
		else if (iAPItem.productType != ProductType.NonConsumable && iAPItem.productType != ProductType.Subscription)
		{
		}
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		UnityEngine.Debug.Log($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
	}
}
