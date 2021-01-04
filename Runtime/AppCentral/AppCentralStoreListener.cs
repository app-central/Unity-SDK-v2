#region using
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Debug = UnityEngine.Debug;
#endregion

// Deriving the AppCentralStoreListener class from IStoreListener enables it to receive messages from Unity Purchasing.
namespace AppCentral
{
    public class AppCentralStoreListener : IStoreListener
    {
        private const string PurchasedSubscriptionKey = "purchased_subscription";
        public delegate void OnInitialisedDelegate();
        public event OnInitialisedDelegate OnInitialisedEvent;

        #region public text fields
        // TODO: Eventually loading from a localisation database?
        public static string LocalizedPriceString { get; private set; } = "Price";
        public static string LocalizedTitle { get; private set; } = "Full Game";
        public static string LocalizedDescription { get; private set; } = "Access all levels";
        #endregion // public text fields

        #region private purchasing accessors
        /// <summary>The Unity Purchasing system.</summary>
        private static IStoreController storeController;
        /// <summary>The store-specific Purchasing subsystems.</summary>
        private static IExtensionProvider storeExtensionProvider;
        #endregion // private purchasing accessors

        #region public product information
        // Product identifiers for all products capable of being purchased:
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the
        // ProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        // TODO: Should be read from some file or online database?
        private static readonly string ProductIDConsumable = "consumable";
        private static readonly string ProductIDNonConsumable = "nonconsumable";
        public static string productIDSubscription;

        public enum ProductType { Consumable, NonConsumable, Subscription }

        // Apple App Store-specific product identifier for the subscription product.
        public static string productNameAppleSubscription;

        // Google Play Store-specific product identifier subscription product.
        private static readonly string productNameGoogleSubscription = "com.unity3d.subscription.original";
        #endregion // public product information

        /// <summary>Check if the listener has been initialised.</summary>
        /// <returns>Return true if it has, false otherwise.</returns>
        public static bool IsInitialized() => AppCentralStoreListener.storeController != null
                                              && AppCentralStoreListener.storeExtensionProvider != null;

        public static bool IsUserSubscribed() => 1 == PlayerPrefs.GetInt(AppCentralStoreListener.PurchasedSubscriptionKey);

        #region IStoreListener implementation
        /// <summary>Called by the system when purchasing has been initialised.</summary>
        /// <param name="controller">Contains information about products.</param>
        /// <param name="extensions">Contains specific extensions for platforms.</param>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized: PASS");

            // save necessary interfaces.
            AppCentralStoreListener.storeController = controller;
            AppCentralStoreListener.storeExtensionProvider = extensions;

            Product appleSubscription = AppCentralStoreListener.storeController.products.WithID(AppCentralStoreListener.productNameAppleSubscription);

            Debug.Log("Product: " + AppCentralStoreListener.productNameAppleSubscription);
            Debug.Log("isoCurrencyCode: " + appleSubscription.metadata.isoCurrencyCode);
            Debug.Log("localizedDescription: " + appleSubscription.metadata.localizedDescription);
            Debug.Log("localizedPrice: " + appleSubscription.metadata.localizedPrice);
            Debug.Log("localizedPriceString: " + appleSubscription.metadata.localizedPriceString);
            Debug.Log("localizedTitle: " + appleSubscription.metadata.localizedTitle);

            // Save available data.
            if (appleSubscription.metadata.localizedTitle.Length > 0)
            { AppCentralStoreListener.LocalizedTitle = appleSubscription.metadata.localizedTitle; }
            if (appleSubscription.metadata.localizedDescription.Length > 0)
            { AppCentralStoreListener.LocalizedDescription = appleSubscription.metadata.localizedDescription; }
            AppCentralStoreListener.LocalizedPriceString = appleSubscription.metadata.localizedPriceString;

            this.OnInitialisedEvent?.Invoke(); // notify anyone waiting for initialisation.
        }

        /// <summary>Called by the system when purchasing initialisation fails.</summary>
        /// <param name="error">The reported error.</param>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // TODO: Notify this in some way?
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        private PurchaseProcessingResult ProcessConsumablePurchase(string productID)
        {
            Debug.Log("ProcessPurchase: PASS. Product: " + productID);
            return PurchaseProcessingResult.Complete;
        }

        private PurchaseProcessingResult ProcessNonConsumablePurchase(string productID)
        {
            Debug.Log("ProcessPurchase: PASS. Product: " + productID);
            return PurchaseProcessingResult.Complete;
        }

        private PurchaseProcessingResult ProcessSubscriptionPurchase(string productID)
        {
            Debug.Log("ProcessPurchase: PASS. Product: " + productID);
            PlayerPrefs.SetInt(AppCentralStoreListener.PurchasedSubscriptionKey, 1);
            return PurchaseProcessingResult.Complete;
        }

        /// <summary>Called by the system when a product has been purchased.</summary>
        /// <param name="args">The event arguments.</param>
        /// <returns></returns>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // for individual handling of specific purchases.
            if (args.purchasedProduct.definition.Equals(new ProductDefinition(AppCentralStoreListener.ProductIDConsumable,
                                                                              UnityEngine.Purchasing.ProductType.Consumable)))
            { return this.ProcessConsumablePurchase(AppCentralStoreListener.ProductIDConsumable); }

            if (args.purchasedProduct.definition.Equals(new ProductDefinition(AppCentralStoreListener.ProductIDNonConsumable,
                                                                              UnityEngine.Purchasing.ProductType.NonConsumable)))
            { return this.ProcessNonConsumablePurchase(AppCentralStoreListener.ProductIDNonConsumable); }

            if (args.purchasedProduct.definition.Equals(new ProductDefinition(AppCentralStoreListener.productIDSubscription,
                                                                              UnityEngine.Purchasing.ProductType.Subscription)))
            { return this.ProcessSubscriptionPurchase(AppCentralStoreListener.productIDSubscription); }

            Debug.Log("ProcessPurchase: FAIL. Unrecognized product: " + args.purchasedProduct.definition.id);

            // Return a flag indicating whether this product has completely been received, or if the application needs
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still
            // saving purchased products to the cloud, and when that save is delayed.
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log("OnPurchaseFailed: FAIL. Product: " + product.definition.storeSpecificId + ", PurchaseFailureReason: " + failureReason);
        }

        #region purchasing
        /// <summary>Try to buy a product.</summary>
        /// <param name="productId">The ID of the product to buy.</param>
        private static void BuyProductID(string productId)
        {
            if (!AppCentralStoreListener.IsInitialized())
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
                return;
            }

            // find product with given ID
            Product product = AppCentralStoreListener.storeController.products.WithID(productId);

            if (product == null || !product.availableToPurchase)
            {
                Debug.Log("BuyProductID: FAIL. Product either not found or not available.");
                return;
            }

            Debug.Log("Purchasing product asynchronously: " + product.definition.id);
            // Async response should arrive at ProcessPurchase or OnPurchaseFailed.
            AppCentralStoreListener.storeController.InitiatePurchase(product);
        }

        /// <summary>Try to buy a product based on Enum.</summary>
        /// <param name="productType">The product type to buy.</param>
        /// <exception cref="ArgumentOutOfRangeException">If given wrong type.</exception>
        public static void BuyProduct(ProductType productType)
        {
            switch (productType)
            {
                case ProductType.Consumable: AppCentralStoreListener.BuyProductID(AppCentralStoreListener.ProductIDConsumable); break;
                case ProductType.NonConsumable: AppCentralStoreListener.BuyProductID(AppCentralStoreListener.ProductIDNonConsumable); break;
                case ProductType.Subscription: AppCentralStoreListener.BuyProductID(AppCentralStoreListener.productIDSubscription); break;
                default: throw new ArgumentOutOfRangeException(nameof(productType), productType, null);
            }
        }

        /// <summary>Force restore previous purchases.</summary>
        /// <remarks>Only allow this in iOS platforms. Android doesn't require this.</remarks>
        public static void RestorePurchases()
        {
            if (!AppCentralStoreListener.IsInitialized())
            {
                // TODO: Retry authorisation?
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            #if UNITY_IOS
        IAppleExtensions apple = AppCentralStoreListener.storeExtensionProvider.GetExtension<IAppleExtensions>(); // get apple specific extension

        // Begin the asynchronous process of restoring purchases. Expect a confirmation response in
        // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
        apple.RestoreTransactions(result =>
                                  {
                                      // The first phase of restoration. If no more responses are received on ProcessPurchase then
                                      // no purchases are available to be restored.
                                      Debug.Log("RestorePurchases continuing: " + result +
                                                ". If no further messages, no purchases available to restore.");
                                  });
            #endif
        }
        #endregion // purchasing
        #endregion // IStoreListener implementation

        /// <summary>Entry point: How the whole system starts.</summary>
        /// <param name="productIDs">Product IDs for what can be bought in the game.</param>
        /// <param name="onInitialisedAction">The action to call when this is initialised.</param>
        public AppCentralStoreListener(ProductIDs productIDs, OnInitialisedDelegate onInitialisedAction)
        {
            if (AppCentralStoreListener.IsInitialized())
            { return; }

            Debug.Log("Init Subscription " + productIDs.subscriptionProductID);
            AppCentralStoreListener.productNameAppleSubscription = productIDs.subscriptionProductID;
            AppCentralStoreListener.productIDSubscription = productIDs.subscriptionProductID;
            this.OnInitialisedEvent += onInitialisedAction;

            // Create a builder, first passing in a suite of Unity provided stores.
            ConfigurationBuilder purchasingConfiguration = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            List<ProductDefinition> productDefinitions = new List<ProductDefinition>
                                                             {
                                                                 new ProductDefinition(AppCentralStoreListener.ProductIDConsumable, UnityEngine.Purchasing.ProductType.Consumable),
                                                                 new ProductDefinition(AppCentralStoreListener.ProductIDNonConsumable, UnityEngine.Purchasing.ProductType.NonConsumable),
                                                             };
            purchasingConfiguration.AddProducts(productDefinitions);

            // Adding the subscription product.
            // Notice this uses store-specific IDs, illustrating if the Product ID was configured differently between Apple and Google stores.
            // Also note that one uses the general ProductIDSubscription handle inside the game - the store-specific IDs must only be referenced here.
            purchasingConfiguration.AddProduct(AppCentralStoreListener.productIDSubscription, UnityEngine.Purchasing.ProductType.Subscription, new IDs
                                                   {
                                                       {AppCentralStoreListener.productNameAppleSubscription, AppleAppStore.Name},
                                                       {AppCentralStoreListener.productNameGoogleSubscription, GooglePlay.Name},
                                                   });

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, purchasingConfiguration);
        }
    }
}