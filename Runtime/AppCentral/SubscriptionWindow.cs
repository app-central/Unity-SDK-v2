
namespace AppCentral
{
    #region using
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Purchasing;
    using UnityEngine.Serialization;
    using UnityEngine.UI;
    #endregion

    /// <summary>This is the graphical interface, the view, of the shop.
    /// It should pull a developer uploaded assets to display a custom made shop interface for the user.</summary>
    public class SubscriptionWindow : MonoBehaviour
    {
        #region inspector fields
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image foregroundImage;
        [SerializeField] private TextMeshProUGUI titleTMP;
        [FormerlySerializedAs("topSubtitleTMP"),SerializeField] private TextMeshProUGUI descriptionTMP;
        [SerializeField] private TextMeshProUGUI termsLinkTMP;
        [SerializeField] private TextMeshProUGUI restoreLinkTMP;
        [SerializeField] private TextMeshProUGUI bottomTitleTMP;
        [SerializeField] private TextMeshProUGUI priceTMP;
        [SerializeField] private Image subscriptionButtonImage;
        [SerializeField] private TextMeshProUGUI subscriptionButtonTMP;
        [ContextMenuItem("Run Configuration", "ReadConfiguration")]
        [SerializeField] private SubscriptionWindowConfiguration subscriptionConfiguration;
        #endregion // inspector fields

        AppCentralStoreListener storeListener;

        // It must be unique, always available and always accessible. It's the interface's presence in the game.
        public static SubscriptionWindow Instance { get; private set; }
        public static bool WindowOpen { get; private set; }

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

        /// <summary>Read configuration file into window parameters. Called by field context menu.</summary>
        private void ReadConfiguration()
        {
            if (this.backgroundImage != null)
            { this.backgroundImage.sprite = this.subscriptionConfiguration.backgroundImage; }
            if (this.foregroundImage != null)
            { this.foregroundImage.sprite = this.subscriptionConfiguration.foregroundImage; }
            if (this.foregroundImage != null)
            {
                Color foregroundImageColor = this.foregroundImage.color;
                foregroundImageColor.a = this.subscriptionConfiguration.foregroundOpacity;
                this.foregroundImage.color = foregroundImageColor;
            }
            if (this.subscriptionButtonImage != null)
            { this.subscriptionButtonImage.sprite = this.subscriptionConfiguration.subscriptionButtonImage; }

            if (this.titleTMP != null)
            {this.titleTMP.text = this.subscriptionConfiguration.topTitleText;}
            if (this.descriptionTMP != null)
            {this.descriptionTMP.text = this.subscriptionConfiguration.topSubtitleText;}
            if (this.termsLinkTMP != null)
            {this.termsLinkTMP.text = this.subscriptionConfiguration.topLeftLinkText;}
            if (this.restoreLinkTMP != null)
            {this.restoreLinkTMP.text = this.subscriptionConfiguration.topRightLinkText;}
            if (this.bottomTitleTMP != null)
            {this.bottomTitleTMP.text = this.subscriptionConfiguration.bottomTitleText;}
            if (this.priceTMP != null)
            {this.priceTMP.text = this.subscriptionConfiguration.bottomSubtitleText;}
            if (this.subscriptionButtonTMP != null)
            {this.subscriptionButtonTMP.text = this.subscriptionConfiguration.subscriptionButtonText;}
        }

        public static void ShowPanel()
        {
            static void OpenWindow()
            {
                SubscriptionWindow.Instance.titleTMP.text = AppCentralStoreListener.LocalizedTitle;
                SubscriptionWindow.Instance.descriptionTMP.text = AppCentralStoreListener.LocalizedDescription;
                SubscriptionWindow.Instance.priceTMP.text = "Just " + AppCentralStoreListener.LocalizedPriceString + "/month";

                SubscriptionWindow.Instance.gameObject.SetActive(true);

                SubscriptionWindow.WindowOpen = true;
            }

            if (AppCentralStoreListener.IsUserSubscribed())
            {
                Debug.Log("User already subscribed, not showing paywall");
                return;
            }

            SubscriptionWindow.Instance.ReadConfiguration();

            // TODO: What is this for?
            UnityWebRequest.Get("https://vnc412s287.execute-api.us-east-1.amazonaws.com/default/unity-tracker?v=1&action=start&appid=" + Application.identifier).SendWebRequest();

            SubscriptionWindow.Instance.storeListener
                = new AppCentralStoreListener(SubscriptionWindow.Instance.subscriptionConfiguration.productIDs, OpenWindow);
        }

        public static void HidePanel()
        {
            SubscriptionWindow.WindowOpen = false;

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

            this.titleTMP ??= this.GetComponent<TextMeshProUGUI>("top", "title");
            this.descriptionTMP ??= this.GetComponent<TextMeshProUGUI>("top", "subtitle");
            this.termsLinkTMP ??= this.GetComponent<TextMeshProUGUI>("terms", "button");

            this.restoreLinkTMP ??= this.GetComponent<TextMeshProUGUI>("restore", "button");
            this.bottomTitleTMP ??= this.GetComponent<TextMeshProUGUI>("bottom", "title");
            this.priceTMP ??= this.GetComponent<TextMeshProUGUI>("bottom", "subtitle");

            this.subscriptionButtonImage ??= this.GetComponent<Image>("subscription", "button");
            this.subscriptionButtonTMP ??= this.GetComponent<TextMeshProUGUI>("subscribe", "tmp");
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