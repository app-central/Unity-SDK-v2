
namespace AppCentral
{
    #region using
    using System;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Purchasing;
    using UnityEngine.Serialization;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;
    #endregion

    [Serializable] public struct ProductIDs
    {
        public string subscriptionProductID;
    }

    /// <summary>This is the graphical interface, the view, of the shop.
    /// It should pull a developer uploaded assets to display a custom made shop interface for the user.</summary>
    public class SubscriptionWindow : MonoBehaviour
    {
        #region inspector fields
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image foregroundImage;
        [SerializeField] private TextMeshProUGUI titleTMP;
        [SerializeField] private TextMeshProUGUI descriptionTMP;
        [SerializeField] private TextMeshProUGUI termsLinkTMP;
        [SerializeField] private TextMeshProUGUI restoreLinkTMP;
        [SerializeField] private TextMeshProUGUI encouragementTitleTMP;
        [SerializeField] private TextMeshProUGUI priceTMP;
        [SerializeField] private Image subscribeButtonImage;
        [SerializeField] private TextMeshProUGUI subscribeButtonTMP;
        #endregion // inspector fields

        private AppCentralStoreListener storeListener;

        // It must be unique, always available and always accessible. It's the interface's presence in the game.
        public static SubscriptionWindow Instance { get; private set; }

        public void OpenAppCentralTerms()
        {
            // TODO: Should be a const field or from a configuration file
            Application.OpenURL("https://www.app-central.com/terms");
        }

        public void RestorePurchases()
        {
            AppCentralStoreListener.RestorePurchases();
        }

        public void BuySubscription()
        {
            AppCentralStoreListener.BuyProduct(AppCentralStoreListener.ProductType.Subscription);
        }

        /// <summary>Initialises the Subscription Window with all required parameters.</summary>
        /// <param name="productIDs">The list of product IDs available for this application.</param>
        /// <param name="backgroundSprite">The sprite to set for the background image. It must be a 16:9 image.</param>
        /// <param name="foregroundSprite">The sprite to set for the foreground image. It must be a 16:9 image with transparency (Or it will hide the background.)</param>
        /// <param name="termsLinkText">The text to put on the Terms & Conditions link.</param>
        /// <param name="restoreLinkText">The text to put on the Restore Purchases link.</param>
        /// <param name="encouragementText">The text to use to encourage the user to subscribe.</param>
        /// <param name="priceTextFormat">The format string to use for the price. Must contain %s where the actual price string will be placed.</param>
        /// <param name="subscribeButtonSprite">The sprite to set for the subscription button image.</param>
        /// <param name="subscribeButtonText">The text to use for the subscription button text.</param>\
        /// <remarks>The window title, description and actual price is derived from the product IDs.</remarks>
        public void Initialise(ProductIDs productIDs, Sprite backgroundSprite = null, Sprite foregroundSprite = null,
                               string termsLinkText = "Terms & Conditions", string restoreLinkText = "Restore Purchases",
                               string encouragementText = "Buy Now!", string priceTextFormat = "Only %s/month",
                               Sprite subscribeButtonSprite = null, string subscribeButtonText = "Subscribe Now")
        {
            void OpenWindow()
            {
                SubscriptionWindow.Instance.titleTMP.text = AppCentralStoreListener.LocalizedTitle;
                SubscriptionWindow.Instance.descriptionTMP.text = AppCentralStoreListener.LocalizedDescription;
                SubscriptionWindow.Instance.priceTMP.text = string.Format(priceTextFormat, AppCentralStoreListener.LocalizedPriceString);

                SubscriptionWindow.Instance.gameObject.SetActive(true);
            }

            if (AppCentralStoreListener.IsUserSubscribed())
            {
                Debug.Log("User already subscribed, not showing paywall");
                return;
            }

			AnalyticsCommunicator.SendApplicationStartRequest();

            Color transparent = new Color(1f, 1f, 1f, 0f);
			this.backgroundImage.sprite = backgroundSprite;
            if (backgroundSprite == null) { this.backgroundImage.color = transparent;}
            this.foregroundImage.sprite = foregroundSprite;
            if (foregroundSprite == null) { this.foregroundImage.color = transparent; }
            this.termsLinkTMP.text = termsLinkText;
            this.restoreLinkTMP.text = restoreLinkText;
            this.encouragementTitleTMP.text = encouragementText;
            this.subscribeButtonImage.sprite = subscribeButtonSprite;
            if (subscribeButtonSprite == null) { this.subscribeButtonImage.color = transparent; }
            this.subscribeButtonTMP.text = subscribeButtonText;

            SubscriptionWindow.Instance.storeListener = new AppCentralStoreListener(productIDs, OpenWindow);
        }

        public static void HidePanel()
        {
            SubscriptionWindow.Instance.gameObject.SetActive(false);
        }

        /// <summary>Find a component in a child that corresponds to the given matches.</summary>
        /// <param name="match1">The first string to match the name to.</param>
        /// <param name="match2">The second string to match the name to.</param>
        /// <typeparam name="T">The type of the component to search for.</typeparam>
        /// <returns>The component if found, null otherwise.</returns>
        private T GetComponent<T>(string match1, string match2 = null) where T : Component
        {
            return this.GetComponentsInChildren<T>().FirstOrDefault(
                    t => t.gameObject.name.ToLower().Contains(match1)
                         && (match2 == null || t.gameObject.name.ToLower().Contains(match2)));
        }

        private void CheckAssignments()
        {
            this.backgroundImage ??= this.GetComponent<Image>("background");
            this.foregroundImage ??= this.GetComponent<Image>("foreground");

            this.titleTMP ??= this.GetComponent<TextMeshProUGUI>("title");
            this.descriptionTMP ??= this.GetComponent<TextMeshProUGUI>("description");
            this.termsLinkTMP ??= this.GetComponent<TextMeshProUGUI>("terms", "button");

            this.restoreLinkTMP ??= this.GetComponent<TextMeshProUGUI>("restore", "button");
            this.encouragementTitleTMP ??= this.GetComponent<TextMeshProUGUI>("encouragement");
            this.priceTMP ??= this.GetComponent<TextMeshProUGUI>("price");

            this.subscribeButtonImage ??= this.GetComponent<Image>("subscribe", "button");
            this.subscribeButtonTMP ??= this.GetComponent<TextMeshProUGUI>("subscribe", "tmp");
        }

        private void OnValidate()
        {
            this.CheckAssignments();
        }

        private void Awake()
        {
            if (SubscriptionWindow.Instance != null && SubscriptionWindow.Instance != this)
            {
                Object.DestroyImmediate(this.gameObject);
                return;
            }
            SubscriptionWindow.Instance = this;
        }
    }
}