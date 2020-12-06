namespace AppCentral
{
    using System;
    using UnityEngine;
    using UnityEngine.Purchasing;

    // TODO: Doesn't seem like this should be MonoBehaviour
    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class Purchaser : MonoBehaviour, IStoreListener
    {
        // TODO: Eventually loading from a localisation database?
        public static string localizedPriceString = "Price";
        public static string localizedTitle = "Full Game";
        public static string localizedDescription = "Access all levels";

        private static IStoreController storeController; // The Unity Purchasing system.
        private static IExtensionProvider storeExtensionProvider; // The store-specific Purchasing subsystems.

        // Product identifiers for all products capable of being purchased:
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        // TODO: Should be read from some file or database?
        private static readonly string kProductIDConsumable = "consumable";
        private static readonly string kProductIDNonConsumable = "nonconsumable";
        public static string kProductIDSubscription;

        // Apple App Store-specific product identifier for the subscription product.
        public static string kProductNameAppleSubscription;

        // Google Play Store-specific product identifier subscription product.
        private static readonly string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

        private static bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return Purchaser.storeController != null && Purchaser.storeExtensionProvider != null;
        }

        private static void BuyProductID(string productId)
        {
            // TODO: Reverse the if and fail first?
            // If Purchasing has been initialized
            if (Purchaser.IsInitialized())
            {
                // look up the Product reference with the general product identifier and the Purchasing system's products collection.
                Product product = Purchaser.storeController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log("Purchasing product asychronously: " + product.definition.id);

                    // buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    Purchaser.storeController.InitiatePurchase(product);
                }
                else
                {
                    // report the product look-up failure situation
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                // report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        // TODO: Can there be a better way to allow for many products?
        public void BuyConsumable()
        {
            // Buy the consumable product using its general identifier. Expect a response either
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            Purchaser.BuyProductID(Purchaser.kProductIDConsumable);
        }

        public void BuyNonConsumable()
        {
            // Buy the non-consumable product using its general identifier. Expect a response either
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            Purchaser.BuyProductID(Purchaser.kProductIDNonConsumable);
        }

        public static void BuySubscription()
        {
            // Buy the subscription product using its the general identifier. Expect a response either
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            // Notice how we use the general product identifier in spite of this ID being mapped to
            // custom store-specific identifiers above.
            Purchaser.BuyProductID(Purchaser.kProductIDSubscription);
        }

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google.
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public static void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!Purchaser.IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ...
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                
                IAppleExtensions apple = Purchaser.storeExtensionProvider.GetExtension<IAppleExtensions>();

                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions(result =>
                                          {
                                              // The first phase of restoration. If no more responses are received on ProcessPurchase then
                                              // no purchases are available to be restored.
                                              Debug.Log("RestorePurchases continuing: " + result +
                                                        ". If no further messages, no purchases available to restore.");
                                          });
            }
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " +
                          Application.platform);
            }
        }

        //  
        // --- IStoreListener
        //
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            Purchaser.storeController = controller;

            // Store specific subsystem, for accessing device-specific store features.
            Purchaser.storeExtensionProvider = extensions;

            Debug.Log("Product: " + Purchaser.kProductNameAppleSubscription);
            Product appleSubscription = Purchaser.storeController.products.WithID(Purchaser.kProductNameAppleSubscription);

            Debug.Log("isoCurrencyCode: " + appleSubscription.metadata.isoCurrencyCode);
            Debug.Log("localizedDescription: " + appleSubscription.metadata.localizedDescription);
            Debug.Log("localizedPrice: " + appleSubscription.metadata.localizedPrice);
            Debug.Log("localizedPriceString: " + appleSubscription.metadata.localizedPriceString);
            Debug.Log("localizedTitle: " + appleSubscription.metadata.localizedTitle);

            if (appleSubscription.metadata.localizedTitle.Length > 0)
            { Purchaser.localizedTitle = appleSubscription.metadata.localizedTitle; }

            if (appleSubscription.metadata.localizedDescription.Length > 0)
            { Purchaser.localizedDescription = appleSubscription.metadata.localizedDescription; }

            Purchaser.localizedPriceString = appleSubscription.metadata.localizedPriceString;
            AppCentral.ShowPaywall(); // TODO: child calling parent
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // TODO: Hard coded responses to dynamic products
            // A consumable product has been purchased by this user.
            if (string.Equals(args.purchasedProduct.definition.id, Purchaser.kProductIDConsumable, StringComparison.Ordinal))
            {
                Debug.Log("ProcessPurchase: PASS. Product: " + args.purchasedProduct.definition.id);
                // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            }
            // Or ... a non-consumable product has been purchased by this user.
            else if (string.Equals(args.purchasedProduct.definition.id, Purchaser.kProductIDNonConsumable, StringComparison.Ordinal))
            {
                Debug.Log("ProcessPurchase: PASS. Product: " + args.purchasedProduct.definition.id);
                // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
            }
            // Or ... a subscription product has been purchased by this user.
            else if (string.Equals(args.purchasedProduct.definition.id, Purchaser.kProductIDSubscription, StringComparison.Ordinal))
            {
                Debug.Log("ProcessPurchase: PASS. Product: " + args.purchasedProduct.definition.id);
                //The subscription item has been successfully purchased, grant this to the player.
                // TODO: Shouldn't be saved in Player Prefs
                PlayerPrefs.SetInt("purchased_subscription", 1);
            }
            // Or ... an unknown product has been purchased by this user. Fill in additional products here....
            else { Debug.Log("ProcessPurchase: FAIL. Unrecognized product: " + args.purchasedProduct.definition.id); }

            // Return a flag indicating whether this product has completely been received, or if the application needs
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still
            // saving purchased products to the cloud, and when that save is delayed.
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log("OnPurchaseFailed: FAIL. Product: " + product.definition.storeSpecificId
                                                          + ", PurchaseFailureReason: " + failureReason);
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (Purchaser.IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var purchasingConfiguration = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // TODO: Dynamic product addition?
            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
            purchasingConfiguration.AddProduct(Purchaser.kProductIDConsumable, ProductType.Consumable);

            // Continue adding the non-consumable product.
            purchasingConfiguration.AddProduct(Purchaser.kProductIDNonConsumable, ProductType.NonConsumable);

            // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            // if the Product ID was configured differently between Apple and Google stores. Also note that
            // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs
            // must only be referenced here.
            purchasingConfiguration.AddProduct(Purchaser.kProductIDSubscription, ProductType.Subscription, new IDs
                                                {
                                                    {Purchaser.kProductNameAppleSubscription, AppleAppStore.Name},
                                                    {Purchaser.kProductNameGooglePlaySubscription, GooglePlay.Name},
                                                });

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, purchasingConfiguration);
        }

        private void Start()
        {
            // TODO: Should be done in constructor instead?
            // If we haven't set up the Unity Purchasing reference
            if (Purchaser.storeController == null)
            {
                // Begin to configure our connection to Purchasing
                this.InitializePurchasing();
            }
        }
    }
}